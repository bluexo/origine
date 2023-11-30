using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using Orleans.Runtime;

namespace Origine
{
    public class SiloHealthCheck : IHealthCheck
    {
        private readonly IHealthCheck participant;

        public SiloHealthCheck(IHealthCheck participant)
        {
            this.participant = participant;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return await participant.CheckHealthAsync(context);
        }
    }
}
