using System;
using Beerdispancer.Domain.Abstractions;

namespace Beerdispancer.Domain.Implementations
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

