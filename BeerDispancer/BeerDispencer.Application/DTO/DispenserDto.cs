using BeerDispenser.Shared;

namespace BeerDispenser.Application.DTO
{
    public class DispenserDto
	{
        public Guid Id { get; set; }
        public decimal? Volume { get; set; }
        public DispenserStatus? Status { get; set; }
        public string ReservedFor { get; set; }
    }
}

