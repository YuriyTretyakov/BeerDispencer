using System;
namespace BeerDispencer.DomainLayer
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

            return duration.Value.Seconds * litersPerSec;
        }

        public static double? GetTotalSpent(double? volume, double pricePerLiter)
        {
            return volume * pricePerLiter;
        }
    }
}

