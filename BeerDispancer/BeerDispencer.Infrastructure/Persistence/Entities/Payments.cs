using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerDispencer.Infrastructure.Persistence.Entities
{
	[Table("Payments")]
	public class Payments
	{
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Dispencer.Id))]
        public Guid DispencerId { get; set; }
        public double Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

