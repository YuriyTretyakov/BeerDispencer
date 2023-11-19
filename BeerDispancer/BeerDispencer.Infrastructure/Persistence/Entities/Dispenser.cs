using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BeerDispenser.Shared;

namespace BeerDispenser.Infrastructure.Persistence.Entities
{
    [Table("Dispenser")]
    public class Dispenser
    {
        [Key]
        public Guid Id { get; set; }
        public decimal Volume { get; set; }
        public DispenserStatusDto Status { get; set; }
        public string ReservedFor { get; set; }
        public bool IsActive { get; set; }
    }
}

