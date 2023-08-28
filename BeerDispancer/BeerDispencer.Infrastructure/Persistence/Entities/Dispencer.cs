using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BeerDispencer.Shared;

namespace BeerDispencer.Infrastructure.Persistence.Entities
{
    [Table("Dispencer")]
    public class Dispencer
    {
        [Key]
        public Guid? Id { get; set; }
        public double? Volume { get; set; }
        public DispencerStatus? Status { get; set; }
    }
}

