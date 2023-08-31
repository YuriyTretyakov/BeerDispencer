using System;

namespace BeerDispancer.Application.DTO
{
	public class UsageDto
	{
        public Guid? Id { get; set; }
        public Guid DispencerId { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public decimal? FlowVolume { get; set; }
        public decimal? TotalSpent { get; set; }
    }
}

