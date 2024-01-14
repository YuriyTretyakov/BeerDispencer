using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BeerDispenser.Application.Implementation.Messaging.Events;
using BeerDispenser.Shared.Dto;

namespace BeerDispenser.Infrastructure.Persistence.Entities
{
    [Table("Usage")]
    public class Usage
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Dispencer.Id")]
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

