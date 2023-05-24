using System;
namespace BeerDispancer.DomainLayer
{
	public class DispencerUpdateDto
	{
        public Guid Id { get; set; }
        public DispencerStatusDto Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

