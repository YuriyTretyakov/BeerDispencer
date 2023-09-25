using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Commands;
using BeerDispenser.Application;
using BeerDispenser.Shared;
using BeerDispenser.Domain.Entity;
using MediatR;
using BeerDispenser.Domain.Abstractions;
using BeerDispenser.Application.Implementation.Response;

namespace BeerDispenser.Application.Implementation.Handlers
{
    public class DispenserUpdateHandler: IRequestHandler<DispenserUpdateCommand, DispenserUpdateResponse>
	{
        private readonly IDispencerUof _dispencerUof;
        private readonly IBeerFlowSettings _beerFlowSettings;

        public DispenserUpdateHandler(
            IDispencerUof dispencerUof,
        IBeerFlowSettings beerFlowSettings)
		{
            _dispencerUof = dispencerUof;
            _beerFlowSettings = beerFlowSettings;
        }

        public async Task<DispenserUpdateResponse> Handle(DispenserUpdateCommand request, CancellationToken cancellationToken)
        {
            var updateCommandResult =  new DispenserUpdateResponse { Result = false };

            using (var transaction = _dispencerUof.StartTransaction())
            {
                var dispenserDto = await _dispencerUof
                    .DispencerRepo
                    .GetByIdAsync(request.Id);

                if (dispenserDto is null)
                {
                    return updateCommandResult;
                }

                var usagesDto = await _dispencerUof
                    .UsageRepo
                    .GetByDispencerIdAsync(dispenserDto.Id);

                var usages = usagesDto.ToDomain(_beerFlowSettings);

                var dispenser = Dispenser.CreateDispenser(
                    dispenserDto.Id,
                    dispenserDto.Volume.Value,
                    dispenserDto.Status.Value,
                    usages.ToList(),
                    _beerFlowSettings);


                if (request.Status == DispenserStatus.Opened)
                {
                    var usageDto = dispenser.Open().ToDto();
                    await _dispencerUof.UsageRepo.AddAsync(usageDto);
                }

                else if (request.Status == DispenserStatus.Closed)
                {
                    var usageDto = dispenser.Close().ToDto();
                    await _dispencerUof.UsageRepo.UpdateAsync(usageDto);
                }
                
                await _dispencerUof.DispencerRepo.UpdateAsync(dispenser.ToDto());

                await _dispencerUof.Complete();
                _dispencerUof.CommitTransaction();
            }
            updateCommandResult.Result = true;
            return updateCommandResult;
        }
    }
}

