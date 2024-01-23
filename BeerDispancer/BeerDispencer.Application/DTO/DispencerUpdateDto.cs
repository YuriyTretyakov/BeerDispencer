using System;
using BeerDispenser.Application.DTO;

namespace BeerDispenser.Application.DTO
{
	public class DispenserUpdateDto:DispenserDto
	{
        public DateTime UpdatedAt { get; set; }
    }
}

