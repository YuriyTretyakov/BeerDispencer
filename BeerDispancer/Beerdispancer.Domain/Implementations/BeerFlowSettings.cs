using System;
using Beerdispancer.Domain.Abstractions;

namespace Beerdispancer.Domain.Implementations
{
    public class BeerFlowSettings : IBeerFlowSettings
    {
        public double LitersPerSecond { get; set; }
        public double PricePerLiter { get; set; }

    }
}

