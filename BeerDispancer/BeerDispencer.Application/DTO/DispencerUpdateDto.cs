using System;
using BeerDispancer.Application.DTO;

namespace BeerDispencer.Application.DTO
{
	public class DispenserUpdateDto:DispenserDto
	{
        public DateTime UpdatedAt { get; set; }
    }
}

