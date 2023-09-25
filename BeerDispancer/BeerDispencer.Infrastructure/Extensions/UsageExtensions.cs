using System;
using BeerDispenser.Application.DTO;
using BeerDispenser.Infrastructure.Persistence.Entities;

namespace BeerDispenser.Infrastructure.Extensions
{
	public static class UsageExtensions
	{
		public static Usage ToDbEntity(this UsageDto dto)
		{
			return new Usage {
                Id = dto.Id,
				ClosedAt = dto.ClosedAt,
				FlowVolume = dto.FlowVolume,
				DispencerId = dto.DispencerId,
				OpenAt = dto.OpenAt,
				TotalSpent = dto.TotalSpent };

        }

        public static UsageDto ToDto(this Usage dto)
        {
            return new UsageDto
            {
                Id = dto.Id,
                ClosedAt = dto.ClosedAt,
                FlowVolume = dto.FlowVolume,
                DispencerId = dto.DispencerId,
                OpenAt = dto.OpenAt,
                TotalSpent = dto.TotalSpent
            };

        }
    }
}

