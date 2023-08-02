using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.DTO;
using BeerDispancer.Application.Implementation.Commands;
using BeerDispencer.Application.Implementation.Response;
using BeerDispencer.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BeerDispancer.Application.Implementation.Handlers
{
    public class DispencerUpdateHandler: IRequestHandler<DispencerUpdateCommand, DispencerUpdateResponse>
	{
        private readonly IDispencerUOW _dispencerUof;
        private readonly IBeerFlowCalculator _calculator;
        private readonly ILogger<DispencerUpdateHandler> _logger;

        public DispencerUpdateHandler(
            IDispencerUOW dispencerUof,
        IBeerFlowCalculator calculator,
        ILogger<DispencerUpdateHandler> logger)
		{
            _dispencerUof = dispencerUof;
            _calculator = calculator;
            _logger = logger;
        }

        public async Task<DispencerUpdateResponse> Handle(DispencerUpdateCommand request, CancellationToken cancellationToken)
        {
            var updateCommandResult =  new DispencerUpdateResponse { Result = false };

            using (var transaction = _dispencerUof.StartTransaction())
            {
                var dispencerDto = await _dispencerUof.DispencerRepo.GetByIdAsync(request.Id);

                if (dispencerDto == null || dispencerDto.Status == request.Status)
                {
                    return updateCommandResult;
                }

                dispencerDto.Status = request.Status;
                
                await _dispencerUof.DispencerRepo.UpdateAsync(dispencerDto);


                if (dispencerDto.Status == DispencerStatusDto.Open)
                {
                    await _dispencerUof.UsageRepo.AddAsync(new UsageDto { DispencerId = dispencerDto.Id, OpenAt = request.UpdatedAt });
                }

                else if (dispencerDto.Status == DispencerStatusDto.Close)
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

