using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.DTO;
using BeerDispancer.Application.Implementation.Commands;
using BeerDispencer.Shared;
using MediatR;

namespace BeerDispancer.Application.Implementation.Handlers
{
    public class CreateDispenserHandler: IRequestHandler<DispenserCreateCommand, DispenserDto>
    {
        private readonly IDispencerUof _dispencerUof;
       
        public CreateDispenserHandler(IDispencerUof dispencerUof)
		{
            _dispencerUof = dispencerUof;
        }

        public async Task<DispenserDto> Handle(DispenserCreateCommand request, CancellationToken cancellationToken)
        {
            var dispencerDto = new DispenserDto { Volume = request.FlowVolume, Status = DispenserStatus.Closed };
            var dispencer = await _dispencerUof.DispencerRepo.AddAsync(dispencerDto);
            await _dispencerUof.Complete();
            return dispencer;
        }
    }
}

