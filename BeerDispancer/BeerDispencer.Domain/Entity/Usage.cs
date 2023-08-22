using BeerDispencer.Domain.Abstractions;

namespace BeerDispencer.Domain.Entity
{
    public class Usage
    {
        public int? Id { get; set; }
        public Guid DispencerId { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public double? FlowVolume { get; set; }
        public double? TotalSpent { get; set; }

        internal Usage(
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

    }
}

