using System;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Shared.Dto;

namespace BeerDispenser.Application.DTO
{
    public class UsageDto
	{
        public Guid Id { get; set; }
        public Guid DispencerId { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public decimal? FlowVolume { get; set; }
        public decimal? TotalSpent { get; set; }
        public Guid? PaidBy { get; set; }
        public PaymentStatusDto? PaymentStatus { get; set; }
        public string Reason { get; set; }
    }
}

