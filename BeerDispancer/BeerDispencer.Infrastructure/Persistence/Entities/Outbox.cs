using System;
using BeerDispenser.Application.DTO;

namespace BeerDispencer.Infrastructure.Persistence.Entities
{
	public class Outbox
	{
		public Guid Id { get; set; }
		public string EventType { get; set; }
		public  string Payload { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public EventStateDto EventState { get; set; }
	}
}

