using System;
using BeerDispencer.Infrastructure.Persistence.Entities;
using BeerDispenser.Application.DTO;

namespace BeerDispencer.Infrastructure.Extensions
{
	public static class OutboxExtensions
	{
		public static Outbox ToDb(this OutboxDto dto)
		{
			return new Outbox
			{
				Id = dto.Id,
				Payload = dto.Payload,
				CreatedAt = dto.CreatedAt,
				EventState = dto.EventState,
				UpdatedAt = dto.UpdatedAt,
				EventType = dto.EventType
			};
		}

        public static OutboxDto ToDto(this Outbox dbOutbox)
        {
            return new OutboxDto
            {
                Id = dbOutbox.Id,
                Payload = dbOutbox.Payload,
                CreatedAt = dbOutbox.CreatedAt,
                EventState = dbOutbox.EventState,
                UpdatedAt = dbOutbox.UpdatedAt,
                EventType = dbOutbox.EventType
            };
        }
    }
}

