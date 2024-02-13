using System;
namespace BeerDispenser.Domain.Abstractions
{
	public interface IBeerFlowCalculator
	{
        public decimal? GetFlowVolume(DateTimeOffset? closedAt, DateTimeOffset? openAt);
        public decimal? GetTotalSpent(decimal? volume);     
    }
}

