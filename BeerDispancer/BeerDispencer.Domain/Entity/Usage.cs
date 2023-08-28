using BeerDispencer.Domain.Abstractions;

namespace BeerDispencer.Domain.Entity
{
    public class Usage
    {
        public int? Id { get; private set; }
        public Guid DispencerId { get; private set; }
        public DateTime OpenAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }
        public decimal? FlowVolume { get; private set; }
        public decimal? TotalSpent { get; private set; }

        private Usage(
            int? id,
        Guid dispencerId,
         DateTime openAt,
         DateTime? closedAt,
         double? flowVolume,
         double? totalSpent
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

        public static Usage CreateNew(Guid dispencerId)
        {
            return new Usage(null, dispencerId, DateTime.UtcNow, null, null, null);
        }

        public static Usage Create(
            int? id,
        Guid dispencerId,
         DateTime openAt,
         DateTime? closedAt,
         double? flowVolume,
         double? totalSpent)
        {
            return new Usage(id, dispencerId, openAt, closedAt, flowVolume, totalSpent);
        }

    }
}

