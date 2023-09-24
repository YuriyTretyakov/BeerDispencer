using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.DTO;
using BeerDispancer.Application.Implementation.Commands;
using BeerDispencer.Application;
using BeerDispencer.Domain.Abstractions;
using BeerDispenser.Domain.Entity;
using MediatR;

namespace BeerDispancer.Application.Implementation.Handlers
{
    public class CreateDispenserHandler: IRequestHandler<DispenserCreateCommand, DispenserDto>
    {
        private readonly IDispencerUof _dispencerUof;
        private readonly IBeerFlowSettings _beerFlowSettings;

        public CreateDispenserHandler(
            IDispencerUof dispencerUof,
            IBeerFlowSettings beerFlowSettings)
		{
            _dispencerUof = dispencerUof;
            _beerFlowSettings = beerFlowSettings;
        }

        public async Task<DispenserDto> Handle(DispenserCreateCommand request, CancellationToken cancellationToken)
        {

            var dispenser = Dispenser
                .CreateNewDispenser(request.FlowVolume, _beerFlowSettings);

            var dispenserDto = dispenser.ToDto();

            dispenserDto = await _dispencerUof.DispencerRepo.AddAsync(dispenserDto);
            await _dispencerUof.Complete();
            return dispenserDto;
        }
    }
}

