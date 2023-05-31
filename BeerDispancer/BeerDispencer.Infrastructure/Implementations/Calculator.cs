using System;
namespace BeerDispencer.Infrastructure.Implementations
{
	public static class Calculator
	{
        public static double? GetFlowVolume(DateTime? closedAt, DateTime? openAt, double litersPerSec)
        {
            var duration = closedAt - openAt;

            if (!duration.HasValue)
            {
                return null;
            }

            return duration.Value.TotalSeconds * litersPerSec;
        }

        public static double? GetTotalSpent(double? volume, double pricePerLiter)
        {
            return volume * pricePerLiter;
        }
    }
}

