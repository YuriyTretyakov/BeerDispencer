using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerDispencer.Infrastructure.Persistence.Entities
{
    [Table("Outbox")]
	public class Outbox
	{
        [Key]
        public int Id { get; set; }
        public string Data { get; set; }
        public OperationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

