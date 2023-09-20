using System;
using BeerDispancer.Application.DTO;
using BeerDispencer.Infrastructure.Persistence.Entities;

namespace BeerDispencer.Infrastructure.Extensions
{
	public static class DispenserExtensions
	{
		public static Dispenser ToDbEntity(this DispenserDto dto)
		{
			return new Dispenser { Id = dto.Id, Status = dto.Status, Volume = dto.Volume };
		}

	

		public static DispenserDto ToDto(this Dispenser entity)
		{
			return new DispenserDto { Id = entity.Id, Status = entity.Status, Volume = entity.Volume };
		}

        

    }
}

