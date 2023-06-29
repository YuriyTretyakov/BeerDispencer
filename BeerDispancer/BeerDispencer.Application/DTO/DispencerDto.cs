using System;
namespace BeerDispancer.Application.DTO
{
	public class DispencerDto
	{
        public string Id { get; set; }
        public double? Volume { get; set; }
        public DispencerStatusDto? Status { get; set; }
    }
}

