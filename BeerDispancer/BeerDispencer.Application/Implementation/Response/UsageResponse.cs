using System;


namespace BeerDispancer.Application.Implementation.Response
{
    public class UsageResponse
    {
        public double Amount { get; set; } 
        public UsageEntry[] Usages { get; set; }
    }

	public class UsageEntry
    {
        public DateTime? OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public decimal? FlowVolume { get; set; }
        public decimal? TotalSpent { get; set; }

    }
}