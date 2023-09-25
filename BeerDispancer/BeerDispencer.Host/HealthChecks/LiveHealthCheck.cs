using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeerDispenser.WebApi.HealthChecks
{
	public class LiveHealthCheck:IHealthCheck
	{
		public LiveHealthCheck()
		{
		}

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}

