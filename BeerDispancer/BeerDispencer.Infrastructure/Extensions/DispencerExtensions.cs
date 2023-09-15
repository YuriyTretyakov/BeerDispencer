using System;
using BeerDispancer.Application.DTO;
using BeerDispencer.Infrastructure.Persistence.Entities;

namespace BeerDispencer.Infrastructure.Extensions
{
	public static class DispencerExtensions
	{
		public static Dispencer ToDbEntity(this DispencerDto dto)
		{
			return new Dispencer { Id = dto.Id, Status = dto.Status, Volume = dto.Volume,ReservedFor =dto.ReservedFor };
		}

	

		public static DispencerDto ToDto(this Dispencer entity)
		{
			return new DispencerDto { Id = entity.Id, Status = entity.Status, Volume = entity.Volume, ReservedFor = entity.ReservedFor };
		}

        

    }
}

