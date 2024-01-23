using BeerDispenser.Domain.Abstractions;

namespace BeerDispenser.Domain.Implementations
{
    public class BeerFlowSettings : IBeerFlowSettings
    {
        public decimal LitersPerSecond { get; set; }
        public decimal PricePerLiter { get; set; }

    }
}

