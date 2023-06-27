using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.DTO;
using BeerDispancer.Application.Implementation.Commands;
using BeerDispencer.Application.DTO;
using BeerDispencer.Application.Implementation.Response;
using BeerDispencer.Domain.Abstractions;
using MediatR;

namespace BeerDispancer.Application.Implementation.Handlers
{
    public class DispencerUpdateHandler: IRequestHandler<DispencerUpdateCommand, DispencerUpdateResponse>
	{
        private readonly IDispencerUof _dispencerUof;
        private readonly IBeerFlowCalculator _calculator;

        public DispencerUpdateHandler(IDispencerUof dispencerUof, IBeerFlowCalculator calculator)
		{
            _dispencerUof = dispencerUof;
            _calculator = calculator;
        }

        public async Task<DispencerUpdateResponse> Handle(DispencerUpdateCommand request, CancellationToken cancellationToken)
        {
            var dispencerUpdate = new DispencerUpdateDto { Id = request.Id, Status = request.Status, UpdatedAt = request.UpdatedAt };
            var updateCommandResult = new DispencerUpdateResponse { Result = false };

            var dispencerDto = await _dispencerUof.DispencerRepo.GetByIdAsync(request.Id);


            if (dispencerDto is null || dispencerDto.Status == request.Status)
            {
                return updateCommandResult;
            }

            await _dispencerUof.DispencerRepo.UpdateAsync(dispencerUpdate);


            if (dispencerUpdate.Status == DispencerStatusDto.Open)
            {
                await _dispencerUof.UsageRepo.AddAsync(new UsageDto { DispencerId = dispencerUpdate.Id, OpenAt = dispencerUpdate.UpdatedAt });
                updateCommandResult.Result = true;
            }

            else if (dispencerUpdate.Status == DispencerStatusDto.Close)
            {
                var usagesFound = await _dispencerUof.UsageRepo.GetByDispencerIdAsync(dispencerUpdate.Id);

                var activeUsage = usagesFound.SingleOrDefault(x => x.ClosedAt == null);

                activeUsage.ClosedAt = dispencerUpdate.UpdatedAt;
                activeUsage.FlowVolume = _calculator.GetFlowVolume(activeUsage.ClosedAt, activeUsage.OpenAt);
                activeUsage.TotalSpent = _calculator.GetTotalSpent(activeUsage.FlowVolume);

                await _dispencerUof.UsageRepo.UpdateAsync(activeUsage);

                updateCommandResult.Result = true;
            }

            return updateCommandResult;
        }
    }
}

