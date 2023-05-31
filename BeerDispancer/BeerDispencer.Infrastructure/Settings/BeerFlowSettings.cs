using System;
using BeerDispancer.Application.Abstractions;

namespace BeerDispencer.Infrastructure.Settings
{
    public class BeerFlowSettings : IBeerFlowSettings
    {
        public double LitersPerSecond { get; set; }
        public double PricePerLiter { get; set; }

    }
}

