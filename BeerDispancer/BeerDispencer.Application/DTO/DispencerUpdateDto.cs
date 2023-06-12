using System;
using BeerDispancer.Application.DTO;

namespace BeerDispencer.Application.DTO
{
	public class DispencerUpdateDto:DispencerDto
	{
        public DateTime UpdatedAt { get; set; }
    }
}

