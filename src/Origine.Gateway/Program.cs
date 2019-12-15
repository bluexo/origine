using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

using NLog;
using NLog.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;

using Origine.Gateway.Extensions;
using Orleans.Providers.MongoDB.Configuration;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Origine.Gateway
{
    [Command]
    [HelpOption("-?")]
    internal class Program
    {
        public static IClusterClient ClusterClient { get; private set; }
        public static IConfigurationRoot Configuration { get; private set; }

        private static AutoResetEvent waitCloseHandle = new AutoResetEvent(false);

        public static void Main(string[] args)
        {
            Console.Title = nameof(Gateway);
            Console.CancelKeyPress += OnStopClient;
            AppDomain.CurrentDomain.UnhandledException += HandleCrash;
            StartClient(args).Wait();
            ConsoleLogger.WriteStatus("Press Ctrl + C|Break key to exit!");
            waitCloseHandle.WaitOne();
        }

        private static async Task StartClient(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddCommandLine(args)
                .Build();

            var clusteringOptions = Configuration.GetSection(nameof(MongoDBMembershipTableOptions));

            var builder = new ClientBuilder()
                .Configure<ClusterOptions>(Configuration.GetSection(nameof(ClusterOptions)))
                .AddSimpleMessageStreamProvider("SMS", options => options.FireAndForgetDelivery = true)
                .UseMongoDBClustering(clusteringOptions)
                .ConfigureLogging(AddLogging)
                .ConfigureServices(AddServices);

            HandleCommand(builder);
            ClusterClient = builder.Build();
            ClusterClient.ConnectWithRetry();

            var boot = ClusterClient.ServiceProvider.GetRequiredService<IHostedService>();
            await boot.StartAsync(default);
            ConsoleLogger.WriteStatus("Client connect to silo host successfully!");
        }

        private static void HandleCommand(IClientBuilder builder)
        {
            var hostName = Configuration.GetSection("host").Get<string>();
            ConsoleLogger.WriteStatus($"Start Gateway with {hostName} HostService");
            if (!string.IsNullOrWhiteSpace(hostName)
                && string.Equals(hostName, "LiteNet", StringComparison.CurrentCultureIgnoreCase))
                builder.UseLiteNet();
            else 
                builder.UseDotNetty();
        }

        private static void HandleCrash(object sender, UnhandledExceptionEventArgs e)
        {
            string path = $"{Directory.GetCurrentDirectory()}/Logs/Crash/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            File.WriteAllText($"{path}/{DateTime.Now.Ticks}.txt", $"Crash info:{ e.ExceptionObject.ToString()}");
        }

        private static void OnStopClient(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            ClusterClient.Close().Wait();
            waitCloseHandle.Set();
        }

        private static void AddLogging(ILoggingBuilder builder)
        {
            builder.AddNLog(new NLogProviderOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true
            });
            LogManager.Configuration = new NLogLoggingConfiguration(Configuration.GetSection("NLog"));
            builder.SetMinimumLevel(LogLevel.Information);
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddSingleton(s => ClusterClient.GetStreamProvider("SMS"));
        }
    }
}