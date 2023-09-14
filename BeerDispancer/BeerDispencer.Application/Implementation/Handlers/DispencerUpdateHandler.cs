using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.DTO;
using BeerDispancer.Application.Implementation.Commands;
using BeerDispencer.Application.Implementation.Response;
using BeerDispencer.Domain.Abstractions;
using BeerDispencer.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BeerDispancer.Application.Implementation.Handlers
{
    public class DispenserUpdateHandler: IRequestHandler<DispenserUpdateCommand, DispenserUpdateResponse>
	{
        private readonly IDispencerUof _dispencerUof;
        private readonly IBeerFlowCalculator _calculator;
        private readonly ILogger<DispenserUpdateHandler> _logger;

        public DispenserUpdateHandler(
            IDispencerUof dispencerUof,
        IBeerFlowCalculator calculator,
        ILogger<DispenserUpdateHandler> logger)
		{
            _dispencerUof = dispencerUof;
            _calculator = calculator;
            _logger = logger;
        }

        public async Task<DispenserUpdateResponse> Handle(DispenserUpdateCommand request, CancellationToken cancellationToken)
        {
            var updateCommandResult =  new DispenserUpdateResponse { Result = false };

            using (var transaction = _dispencerUof.StartTransaction())
            {
                var dispencerDto = await _dispencerUof.DispencerRepo.GetByIdAsync(request.Id);

                if (dispencerDto == null || dispencerDto.Status == request.Status)
                {
                    return updateCommandResult;
                }

                dispencerDto.Status = request.Status;
                
                await _dispencerUof.DispencerRepo.UpdateAsync(dispencerDto);


                if (dispencerDto.Status == DispenserStatus.Opened)
                {
                    await _dispencerUof.UsageRepo.AddAsync(new UsageDto { DispencerId = dispencerDto.Id, OpenAt = request.UpdatedAt });
                }

                else if (dispencerDto.Status == DispenserStatus.Closed)
                {
                    var usagesFound = await _dispencerUof.UsageRepo.GetByDispencerIdAsync(dispencerDto.Id);

                    var activeUsage = usagesFound.SingleOrDefault(x => x.ClosedAt == null);

                    if (activeUsage == null)
                    {
                        _logger.LogError($"No usage records with {nameof(UsageDto.ClosedAt)} ==null found for dispencer id:{dispencerDto.Id}");
                        updateCommandResult.Result = false;
                        return updateCommandResult;
                    }

                    activeUsage.ClosedAt = request.UpdatedAt;
                    activeUsage.FlowVolume = _calculator.GetFlowVolume(activeUsage.ClosedAt, activeUsage.OpenAt);
                    activeUsage.TotalSpent = _calculator.GetTotalSpent(activeUsage.FlowVolume);

                    await _dispencerUof.UsageRepo.UpdateAsync(activeUsage);

                }

                await _dispencerUof.Complete();
                _dispencerUof.CommitTransaction();
            }
            updateCommandResult.Result = true;
            return updateCommandResult;
        }
    }
}

