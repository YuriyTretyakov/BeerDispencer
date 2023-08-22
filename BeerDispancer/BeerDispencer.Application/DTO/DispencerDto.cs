using System;
using BeerDispencer.Shared;

namespace BeerDispancer.Application.DTO
{
	public class DispencerDto
	{
        public Guid Id { get; set; }
        public double? Volume { get; set; }
        public DispencerStatus? Status { get; set; }
    }
}

