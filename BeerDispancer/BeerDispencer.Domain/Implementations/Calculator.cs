using BeerDispencer.Domain.Abstractions;

namespace BeerDispencer.Domain.Implementations
{
    public  class Calculator :IBeerFlowCalculator
	{
        private readonly IBeerFlowSettings _settings;

        public Calculator(IBeerFlowSettings settings)
        {
            _settings = settings;
        }

        public  decimal? GetFlowVolume(DateTime? closedAt, DateTime? openAt)
        {
            var duration = closedAt - openAt;

            if (!duration.HasValue)
            {
                return null;
            }

            return (decimal)duration.Value.TotalSeconds * _settings.LitersPerSecond;
        }


        public  decimal? GetTotalSpent(decimal? volume)
        {
            return volume * _settings.PricePerLiter;
        }
    }
}

