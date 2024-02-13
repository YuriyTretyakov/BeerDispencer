using BeerDispenser.Shared.Dto;

namespace BeerDispenser.Application.DTO
{
    public class DispenserDto
	{
        public Guid Id { get; set; }
        public decimal? Volume { get; set; }
        public DispenserStatusDto? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}

