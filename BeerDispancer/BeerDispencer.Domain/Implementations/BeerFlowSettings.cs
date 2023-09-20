using BeerDispencer.Domain.Abstractions;

namespace BeerDispencer.Domain.Implementations
{
    public class BeerFlowSettings : IBeerFlowSettings
    {
        public decimal LitersPerSecond { get; set; }
        public decimal PricePerLiter { get; set; }

    }
}

