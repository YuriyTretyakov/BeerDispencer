using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BeerDispencer.Shared;

namespace BeerDispencer.Infrastructure.Persistence.Entities
{
    [Table("Dispencer")]
    public class Dispenser
    {
        [Key]
        public Guid? Id { get; set; }
        public decimal? Volume { get; set; }
        public DispenserStatus? Status { get; set; }
    }
}

