using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Origine
{
    public sealed class ClientConnectRetryFilter : IClientConnectionRetryFilter
    {
        private int _retryCount = 0;
        private const int MaxRetry = 5;
        private const int Delay = 1_500;

        public async Task<bool> ShouldRetryConnectionAttempt(
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (_retryCount >= MaxRetry)
            {
                return false;
            }

            if (!cancellationToken.IsCancellationRequested &&
                exception is SiloUnavailableException siloUnavailableException)
            {
                await Task.Delay(++_retryCount * Delay, cancellationToken);
                return true;
            }

            return false;
        }
    }

    public static class ClusterClientExtensions
    {
        public static IClientBuilder ConfigureClusterClient(this IClientBuilder builder, IConfiguration config)
        {
            builder
               .Configure<ClusterOptions>(config.GetSection(nameof(ClusterOptions)))
               .Configure<GatewayOptions>(option => option.GatewayListRefreshPeriod = TimeSpan.FromSeconds(10))
               .AddClusterConnectionLostHandler((o, e) => ConsoleLogger.WriteError($"Cluster Connection Lost!"))
               .AddBroadcastChannel("SMS", options => options.FireAndForgetDelivery = false);

            return builder;
        }
    }
}
