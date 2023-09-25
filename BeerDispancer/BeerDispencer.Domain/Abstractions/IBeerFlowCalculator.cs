using System;
namespace BeerDispenser.Domain.Abstractions
{
	public interface IBeerFlowCalculator
	{
        public decimal? GetFlowVolume(DateTime? closedAt, DateTime? openAt);
        public decimal? GetTotalSpent(decimal? volume);     
    }
}

