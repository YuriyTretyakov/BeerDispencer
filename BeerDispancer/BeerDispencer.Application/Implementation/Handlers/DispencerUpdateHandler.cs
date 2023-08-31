using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.Implementation.Commands;
using BeerDispencer.Application;
using BeerDispencer.Application.Implementation.Response;
using BeerDispencer.Domain.Abstractions;
using BeerDispencer.Domain.Entity;
using BeerDispencer.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BeerDispancer.Application.Implementation.Handlers
{
    public class DispencerUpdateHandler: IRequestHandler<DispencerUpdateCommand, DispencerUpdateResponse>
	{
        private readonly IDispencerUof _dispencerUof;
        private readonly IBeerFlowCalculator _calculator;

        public DispencerUpdateHandler(
            IDispencerUof dispencerUof,
        IBeerFlowCalculator calculator,
        ILogger<DispencerUpdateHandler> logger)
		{
            _dispencerUof = dispencerUof;
            _calculator = calculator;
        }

        public async Task<DispencerUpdateResponse> Handle(DispencerUpdateCommand request, CancellationToken cancellationToken)
        {
            var updateCommandResult =  new DispencerUpdateResponse { Result = false };

            using (var transaction = _dispencerUof.StartTransaction())
            {
                var dispencerDto = await _dispencerUof
                    .DispencerRepo
                    .GetByIdAsync(request.Id);

                if (dispencerDto is null)
                {
                    return updateCommandResult;
                }

                var usagesDto = await _dispencerUof
                    .UsageRepo
                    .GetByDispencerIdAsync(dispencerDto.Id.Value);

                var usages = usagesDto.ToDomain();

                var dispencer = Dispencer.Create(
                    dispencerDto.Id.Value,
                    dispencerDto.Volume.Value,
                    dispencerDto.Status.Value,
                    usages.ToList());


                if (request.Status == DispencerStatus.Open)
                {
                    var usageDto = dispencer.Open().ToDto();
                    await _dispencerUof.UsageRepo.AddAsync(usageDto);
                }

                else if (request.Status == DispencerStatus.Close)
                {
                    var usageDto = dispencer.Close(_calculator).ToDto();
                    await _dispencerUof.UsageRepo.UpdateAsync(usageDto);
                }
                
                await _dispencerUof.DispencerRepo.UpdateAsync(dispencer.ToDto());

                await _dispencerUof.Complete();
                _dispencerUof.CommitTransaction();
            }
            updateCommandResult.Result = true;
            return updateCommandResult;
        }
    }
}

