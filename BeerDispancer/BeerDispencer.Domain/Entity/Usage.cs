using BeerDispenser.Domain.Abstractions;
using BeerDispenser.Domain.Implementations;

namespace BeerDispenser.Domain.Entity
{
    public sealed class Usage
    {
        public Guid Id { get; set; }
        public Guid DispencerId { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public decimal? FlowVolume { get; set; }
        public decimal? TotalSpent { get; set; }

        private IBeerFlowSettings _beerFlowSettings;

        internal Usage(
            Guid id,
        Guid dispencerId,
         DateTime openAt,
         DateTime? closedAt,
         decimal? flowVolume,
         decimal? totalSpent,
         IBeerFlowSettings beerFlowSettings = null
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
            var calculator = new Calculator(_beerFlowSettings);
            ClosedAt = DateTime.UtcNow;
            FlowVolume = calculator.GetFlowVolume(ClosedAt, OpenAt);
            TotalSpent = calculator.GetTotalSpent(FlowVolume);
            return this;
        }


        public static Usage Create(
            Guid id,
        Guid dispencerId,
         DateTime openAt,
         DateTime? closedAt,
         decimal? flowVolume,
         decimal? totalSpent)
        {
            return new Usage(id, dispencerId, openAt, closedAt, flowVolume, totalSpent);
        }

        public static Usage CreateReserved(Guid dispencerId, decimal amount)
        {
            return new Usage(Guid.NewGuid(), dispencerId, DateTime.UtcNow, DateTime.UtcNow, null, amount);
        }

        public static Usage CreateStarted(Guid dispencerId, IBeerFlowSettings beerFlowSettings)
        {
            return new Usage(Guid.NewGuid(), dispencerId, DateTime.UtcNow, null, null, null, beerFlowSettings);
        }

       

        public static Usage Create(Guid id, Guid dispencerId, DateTime openAt, IBeerFlowSettings beerFlowSettings)
        {
            return new Usage(id, dispencerId, openAt, null, null, null, beerFlowSettings);
        }
    }
}