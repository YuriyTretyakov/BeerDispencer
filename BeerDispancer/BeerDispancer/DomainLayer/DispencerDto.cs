using System;
namespace BeerDispancer.DomainLayer
{
	public class DispencerDto
	{
        public Guid Id { get; set; }
        public double Volume { get; set; }
        public DispencerStatusDto Status { get; set; }
    }
}

