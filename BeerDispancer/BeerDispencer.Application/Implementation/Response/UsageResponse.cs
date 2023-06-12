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
        public double? FlowVolume { get; set; }
        public double? TotalSpent { get; set; }

    }
}