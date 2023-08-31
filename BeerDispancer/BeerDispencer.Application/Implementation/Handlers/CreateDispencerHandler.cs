using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.DTO;
using BeerDispancer.Application.Implementation.Commands;
using BeerDispencer.Application;
using BeerDispencer.Domain.Entity;
using MediatR;

namespace BeerDispancer.Application.Implementation.Handlers
{
    public class CreateDispencerHandler: IRequestHandler<DispencerCreateCommand, DispencerDto>
    {
        private readonly IDispencerUof _dispencerUof;
       
        public CreateDispencerHandler(IDispencerUof dispencerUof)
		{
            _dispencerUof = dispencerUof;
        }

        public async Task<DispencerDto> Handle(DispencerCreateCommand request, CancellationToken cancellationToken)
        {

            var dispencer = Dispencer
                .CreateNewDispencer(request.FlowVolume);

            var dispencerDto = dispencer.ToDto();

            dispencerDto = await _dispencerUof.DispencerRepo.AddAsync(dispencerDto);
            await _dispencerUof.Complete();
            return dispencerDto;
        }
    }
}

