namespace BeerDispencer.Shared
{
    public class UsageResponse
    {
        public decimal Amount { get; set; }
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