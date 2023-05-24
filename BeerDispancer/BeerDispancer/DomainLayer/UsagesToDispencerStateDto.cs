using System;
using BeerDispancer.DomainLayer;

namespace BeerDispencer.DomainLayer
{
	public class UsagesToDispencerStateDto
	{
		public DispencerStatusDto DispencerState { get; set; }
        public Guid DispencerId { get; set; }
        public DateTime? OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public double? FlowVolume { get; set; }
        public double? TotalSpent { get; set; }

    }
}

