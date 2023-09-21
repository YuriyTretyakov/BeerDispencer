using System;
using BeerDispencer.Shared;

namespace BeerDispancer.Application.DTO
{
	public class DispenserDto
	{
        public Guid? Id { get; set; }
        public decimal? Volume { get; set; }
        public DispenserStatus? Status { get; set; }
    }
}

