
using System;
using Beerdispancer.Domain.Entities;

using BeerDispencer.WebApi.Commands;
using BeerDispencer.WebApi.Responses;

namespace BeerDispencer.WebApi.Extensions
{
	public static class DataObjectExtensions
	{
        public static DispencerResponse ToViewModel(this DispencerDto dispencerDto)
        {
            return new DispencerResponse { Id = dispencerDto.Id, FlowVolume = dispencerDto.Volume };
        }

        public static DispencerDto ToDto(this DispencerCreateCommand dispencerCommand)
        {
            return new DispencerDto { Volume = dispencerCommand.FlowVolume };
        }

    }
}

