using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerDispencer.Infrastructure.Persistence.Entities
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
    }
}

