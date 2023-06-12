using System;
using System.Runtime;
using Beerdispancer.Domain.Abstractions;
using Beerdispancer.Domain.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeerDispancer.Application.Implementation
{
	public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection collection)
        {
            collection.AddSingleton<IBeerFlowCalculator, Calculator>();
        }
	}
}

