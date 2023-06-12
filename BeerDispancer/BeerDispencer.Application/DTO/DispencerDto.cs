using System;
namespace BeerDispancer.Application.DTO
{
	public class DispencerDto
	{
        public Guid Id { get; set; }
        public double? Volume { get; set; }
        public DispencerStatusDto? Status { get; set; }
    }
}

