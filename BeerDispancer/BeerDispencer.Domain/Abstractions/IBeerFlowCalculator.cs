using System;
namespace BeerDispencer.Domain.Abstractions
{
	public interface IBeerFlowCalculator
	{
        public decimal? GetFlowVolume(DateTime? closedAt, DateTime? openAt);
        public decimal? GetTotalSpent(double? volume);     
    }
}

