using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Origine
{
    public static class ClusterClientExtensions
    {
        public static IClusterClient CreateClusterClient(this IServiceProvider serviceProvider, Action<IClientBuilder> buildAction = null)
        {
            var logProvider = serviceProvider.GetService<ILoggerProvider>();
            var config = serviceProvider.GetService<IConfiguration>();
            var log = serviceProvider.GetService<ILogger<ClientBuilder>>();

            var buidler = new ClientBuilder()
               .Configure<ClusterOptions>(config.GetSection(nameof(ClusterOptions)))
               .Configure<GatewayOptions>(option => option.GatewayListRefreshPeriod = TimeSpan.FromSeconds(10))
               .AddClusterConnectionLostHandler((o, e) => ConsoleLogger.WriteError($"Cluster Connection Lost!"))
               .ConfigureLogging(logging => logging.AddProvider(logProvider))
               .AddSimpleMessageStreamProvider("SMS");
            buildAction?.Invoke(buidler);

            var client = buidler.Build();

            client.Connect(RetryFilter).GetAwaiter().GetResult();
            log.LogInformation($"Orleans client connected!");
            return client;

            async Task<bool> RetryFilter(Exception exception)
            {
                log?.LogWarning("Exception while attempting to connect to Orleans cluster: {Exception}", exception);
                await Task.Delay(TimeSpan.FromSeconds(3));
                return true;
            }
        }

        public static IClusterClient ConnectWithRetry(this IClusterClient client)
        {
            int attempt = 0;
            if (client.IsInitialized) return client;
            client.Connect(async e =>
            {
                ConsoleLogger.WriteStatus($"Attempt {attempt++} failed to initialize the Orleans client.");
                ConsoleLogger.WriteError(e.Message);
                await Task.Delay(3000);
                return true;
            })
           .Wait();
            return client;
        }
    }
}
