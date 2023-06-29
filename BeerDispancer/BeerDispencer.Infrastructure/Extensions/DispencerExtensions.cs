using System;
using BeerDispancer.Application.DTO;
using BeerDispencer.Infrastructure.Persistence.Entities;
using MongoDB.Bson;

namespace BeerDispencer.Infrastructure.Extensions
{
	public static class DispencerExtensions
	{
		public static Dispencer ToDbEntity(this DispencerDto dto)
		{
			return new Dispencer {  Status = ToDbEntity(dto.Status), Volume = dto.Volume };
		}

		public static DispencerStatus? ToDbEntity(DispencerStatusDto? dto)
		{
			if (dto == null)
				return null;

			return (DispencerStatus)Enum.Parse(typeof(DispencerStatus), dto.ToString());
		}

		public static DispencerDto ToDto(this Dispencer entity)
		{
			return new DispencerDto { Id = entity.Id.ToString(), Status = ToDto(entity.Status), Volume = entity.Volume };
		}

        public static DispencerStatusDto? ToDto(DispencerStatus? entity)
        {
            if (entity == null)
                return null;

            return (DispencerStatusDto)Enum.Parse(typeof(DispencerStatusDto), entity.ToString());
        }

    }
}

