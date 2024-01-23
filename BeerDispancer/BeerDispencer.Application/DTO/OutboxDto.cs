using System;
using BeerDispenser.Shared.Dto;

namespace BeerDispenser.Application.DTO
{
    public class OutboxDto
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string Payload { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public EventStateDto EventState { get; set; }
    }
}

