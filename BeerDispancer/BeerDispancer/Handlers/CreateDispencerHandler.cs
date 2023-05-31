using System;
using BeerDispancer.Application.Abstractions;
using BeerDispencer.Infrastructure.Implementations;
using BeerDispencer.WebApi.Commands;
using BeerDispencer.WebApi.Extensions;
using BeerDispencer.WebApi.Responses;
using MediatR;

namespace BeerDispencer.WebApi.Handlers
{
	public class CreateDispencerHandler: IRequestHandler<DispencerCreateCommand, DispencerResponse>
    {
        private readonly IDispencerUof _dispencerUof;
       
        public CreateDispencerHandler(IDispencerUof dispencerUof)
		{
            _dispencerUof = dispencerUof;
        }

        public async Task<DispencerResponse> Handle(DispencerCreateCommand request, CancellationToken cancellationToken)
        {
            var dispencerDto = request.ToDto();
            var dispencer = await _dispencerUof.DispencerRepo.CreateAsync(dispencerDto);
            await _dispencerUof.Complete();
            dispencer.Id = dispencer.Id;
            return dispencer.ToViewModel();
        }
    }
}

