using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerDispencer.Infrastructure.Persistence.Entities
{
    [Table("Usage")]
    public class Usage
    {
        [Key]
        public int? Id { get; set; }
        [ForeignKey("Dispencer.Id")]
        public Guid DispencerId { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public double? FlowVolume { get; set; }
        public double? TotalSpent { get; set; }
    }
}

