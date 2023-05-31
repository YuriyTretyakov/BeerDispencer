using System;
using BeerDispencer.Application.Abstractions;

namespace BeerDispencer.Infrastructure.Implementations
{
	public  class Calculator :IBeerFlowCalculator
	{
        public  double? GetFlowVolume(DateTime? closedAt, DateTime? openAt, double litersPerSec)
        {
            var duration = closedAt - openAt;

            if (!duration.HasValue)
            {
                return null;
            }

            return duration.Value.TotalSeconds * litersPerSec;
        }

        public  double? GetTotalSpent(double? volume, double pricePerLiter)
        {
            return volume * pricePerLiter;
        }
    }
}

