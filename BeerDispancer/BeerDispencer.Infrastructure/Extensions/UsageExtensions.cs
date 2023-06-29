using System;
using BeerDispancer.Application.DTO;
using BeerDispencer.Infrastructure.Persistence.Entities;
using MongoDB.Bson;

namespace BeerDispencer.Infrastructure.Extensions
{
	public static class UsageExtensions
	{
		public static Usage ToDbEntity(this UsageDto dto)
		{
			return new Usage {
                
				ClosedAt = dto.ClosedAt,
				FlowVolume = dto.FlowVolume,
				DispencerId = new ObjectId(dto.DispencerId),
				OpenAt = dto.OpenAt,
				TotalSpent = dto.TotalSpent };
        }

        public static UsageDto ToDto(this Usage dto)
        {
            return new UsageDto
            {
                Id = dto.Id.ToString(),
                ClosedAt = dto.ClosedAt,
                FlowVolume = dto.FlowVolume,
                DispencerId = dto.DispencerId.ToString(),
                OpenAt = dto.OpenAt,
                TotalSpent = dto.TotalSpent
            };

        }
    }
}

