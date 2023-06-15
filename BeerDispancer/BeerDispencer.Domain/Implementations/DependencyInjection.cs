using System;
using System.Runtime;
using BeerDispencer.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeerDispencer.Domain.Implementations
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

