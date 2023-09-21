using BeerDispencer.Domain.Abstractions;
using BeerDispencer.Domain.Implementations;

namespace BeerDispencer.Domain.Entity
{
    public sealed class Usage : Entity
    {
        public Guid DispencerId { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public decimal? FlowVolume { get; set; }
        public decimal? TotalSpent { get; set; }

        private IBeerFlowSettings _beerFlowSettings;

        internal Usage(
         Guid? id,
         Guid dispencerId,
         DateTime openAt,
         IBeerFlowSettings beerFlowSettings,
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
            _beerFlowSettings = beerFlowSettings;
        }

        public Usage SetClose()
        {
            ClosedAt = DateTime.UtcNow;
            var calculator = new Calculator(_beerFlowSettings);
            FlowVolume = calculator.GetFlowVolume(ClosedAt, OpenAt);
            TotalSpent = calculator.GetTotalSpent(FlowVolume);
            return this;
        }

        public static Usage Create(Guid dispencerId, IBeerFlowSettings beerFlowSettings)
        {
            return new Usage(null, dispencerId, DateTime.UtcNow, beerFlowSettings, null, null, null);
        }

        public static Usage Create(
            Guid? id,
        Guid dispencerId,
         DateTime openAt,
         IBeerFlowSettings beerFlowSettings,
         DateTime? closedAt,
         decimal? flowVolume,
         decimal? totalSpent
         )
        {
            return new Usage(id, dispencerId, openAt, beerFlowSettings,closedAt, flowVolume, totalSpent);
        }
    }
}