using System;
using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.DTO;
using BeerDispancer.Application.Implementation.Commands;
using BeerDispancer.Application.Implementation.Response;

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
            var dispencerDto = new DispencerDto { Volume = request.FlowVolume, Status = DispencerStatusDto.Close };
            var dispencer = await _dispencerUof.DispencerRepo.AddAsync(dispencerDto);
            await _dispencerUof.Complete();
            return dispencer;
        }
    }
}

