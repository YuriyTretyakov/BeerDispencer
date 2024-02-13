namespace BeerDispenser.Shared
{
    public class UsageResponse
    {
        public decimal Amount { get; set; }
        public UsageEntry[] Usages { get; set; }
    }

    public class UsageEntry
    {
        public Guid Id { get; set; }
        public DateTimeOffset? OpenedAt { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
        public decimal? FlowVolume { get; set; }
        public decimal? TotalSpent { get; set; }

    }
}