using System;
using System.Net;
using BeerDispancer.Application.Abstractions;
using BeerDispencer.Infrastructure.Implementations;
using BeerDispencer.WebApi.Commands;
using MediatR;

namespace BeerDispencer.WebApi.Handlers
{
	public class DispencerUpdateHandler: IRequestHandler<DispencerUpdateCommand, bool>
	{
        private readonly IDispencerUof _dispencerUof;
        private readonly IBeerFlowSettings _beerFlowSettings;

        public DispencerUpdateHandler(IDispencerUof dispencerUof, IBeerFlowSettings beerFlowSettings)
		{
            _dispencerUof = dispencerUof;
            _beerFlowSettings = beerFlowSettings;
        }

        public async Task<bool> Handle(DispencerUpdateCommand request, CancellationToken cancellationToken)
        {
            var result = await _dispencerUof.UpdateDispencerStateAsync(request, _beerFlowSettings);
            return result;
        }
    }
}

