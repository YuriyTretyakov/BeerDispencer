using BeerDispenser.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeerDispenser.Domain.Implementations
{
    public static class DependencyInjection
    {
        public static void AddDomain(this IServiceCollection collection, ConfigurationManager configuration)
        {
            var beerFlowSettings = configuration.GetSection(nameof(BeerFlowSettings)).Get<BeerFlowSettings>();
            collection.AddSingleton<IBeerFlowSettings>(beerFlowSettings);
        }
	}
}

