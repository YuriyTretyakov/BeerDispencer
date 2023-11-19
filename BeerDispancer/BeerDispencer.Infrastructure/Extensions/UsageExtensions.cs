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
				TotalSpent = dto.TotalSpent,
                PaidBy = dto.PaidBy,
                Reason = dto.Reason,
                PaymentStatus = dto.PaymentStatus
            };

        }

        public static UsageDto ToDto(this Usage db)
        {
            return new UsageDto
            {
                Id = db.Id,
                ClosedAt = db.ClosedAt,
                FlowVolume = db.FlowVolume,
                DispencerId = db.DispencerId,
                OpenAt = db.OpenAt,
                TotalSpent = db.TotalSpent,
                PaidBy = db.PaidBy,
                Reason = db.Reason,
                PaymentStatus = db.PaymentStatus
            };

        }
    }
}

