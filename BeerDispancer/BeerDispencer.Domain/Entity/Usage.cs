using BeerDispencer.Domain.Abstractions;

namespace BeerDispencer.Domain.Entity
{
    public sealed class Usage
    {
        public Guid? Id { get; set; }
        public Guid DispencerId { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public decimal? FlowVolume { get; set; }
        public decimal? TotalSpent { get; set; }

        internal Usage(
            Guid? id,
        Guid dispencerId,
         DateTime openAt,
         DateTime? closedAt,
         decimal? flowVolume,
         decimal? totalSpent
         )
        {
            Id = id;
            DispencerId = dispencerId;
            OpenAt = openAt;
            ClosedAt = closedAt;
            FlowVolume = flowVolume;
            TotalSpent = totalSpent;
        }

        public Usage SetClose(IBeerFlowCalculator calculator)
        {
            ClosedAt = DateTime.UtcNow;
            FlowVolume = calculator.GetFlowVolume(ClosedAt, OpenAt);
            TotalSpent = calculator.GetTotalSpent(FlowVolume);
            return this;
        }

        public static Usage Create(Guid dispencerId)
        {
            return new Usage(null, dispencerId, DateTime.UtcNow, null, null, null);
        }

    }
}

