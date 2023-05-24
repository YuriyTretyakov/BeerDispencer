using System;
using BeerDispancer.DataLayer.Models;
using BeerDispancer.DomainLayer;
using BeerDispancer.PresentationLayer;

namespace BeerDispancer.Extensions
{
	public static class DispencerExtensions
	{
		public static DispencerDto ToDto(this DispencerCreate model)
		{
			return new DispencerDto { Volume = model.FlowVolume, Status = DispencerStatusDto.Close };
		}

		public static DispencerCreateResponse ToViewModel(this DispencerDto dispencerDto)
		{
			return new DispencerCreateResponse { Id = dispencerDto.Id, FlowVolume = dispencerDto.Volume};
		}

		public static DispencerUpdateDto ToDto(this DispenserUpdate dispencderUpdate, Guid id)
		{
			return new DispencerUpdateDto { Id = id, Status = dispencderUpdate.Status, UpdatedAt = dispencderUpdate.UpdatedAt };
		}

        public static Dispencer ToDbModel(this DispencerDto dispencderUpdate)
        {
            return new Dispencer { Id = dispencderUpdate.Id, Status = dispencderUpdate.Status, Volume = dispencderUpdate.Volume };
        }
    }
}

