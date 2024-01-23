using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace BeerDispenser.WebApi.HealthChecks
{
	public class ReadyHealthCheck: IHealthCheck
	{
        private readonly IHostApplicationLifetime _lifetime;
        private bool _ready = false;

        public ReadyHealthCheck(IHostApplicationLifetime lifetime)
		{
            _lifetime = lifetime;
            lifetime.ApplicationStarted.Register(() => {
                _ready = true;
            });
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return _ready ?
                Task.FromResult(HealthCheckResult.Healthy()) :
                Task.FromResult(HealthCheckResult.Unhealthy());
        }
    }
}

