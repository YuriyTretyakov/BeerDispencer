using System;
using BeerDispancer.Application.DTO;
using BeerDispencer.Infrastructure.Persistence.Entities;

namespace BeerDispencer.Infrastructure.Extensions
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

