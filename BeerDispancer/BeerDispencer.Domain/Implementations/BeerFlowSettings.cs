using System;
using BeerDispencer.Domain.Abstractions;

namespace BeerDispencer.Domain.Implementations
{
    public class BeerFlowSettings : IBeerFlowSettings
    {
        public double LitersPerSecond { get; set; }
        public double PricePerLiter { get; set; }

    }
}

