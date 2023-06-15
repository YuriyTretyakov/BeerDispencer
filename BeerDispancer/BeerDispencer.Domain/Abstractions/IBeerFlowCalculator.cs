using System;
namespace BeerDispencer.Domain.Abstractions
{
	public interface IBeerFlowCalculator
	{
        public double? GetFlowVolume(DateTime? closedAt, DateTime? openAt);
        public double? GetTotalSpent(double? volume);     
    }
}

