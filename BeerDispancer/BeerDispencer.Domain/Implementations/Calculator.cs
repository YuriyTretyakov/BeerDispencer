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

        public  double? GetFlowVolume(DateTime? closedAt, DateTime? openAt)
        {
            var duration = closedAt - openAt;

            if (!duration.HasValue)
            {
                return null;
            }

            return duration.Value.TotalSeconds * _settings.LitersPerSecond;
        }

        public  double? GetTotalSpent(double? volume)
        {
            return volume * _settings.PricePerLiter;
        }
    }
}

