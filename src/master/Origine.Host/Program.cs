using System;
using System.IO;
using System.Threading;
using System.Runtime.Loader;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Orleans;
using Orleans.Runtime;
using Orleans.Hosting;
using Orleans.Statistics;
using Orleans.Configuration;
using Orleans.Providers.MongoDB.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HostBuilderContext = Microsoft.Extensions.Hosting.HostBuilderContext;
using McMaster.Extensions.CommandLineUtils;

using Origine.Network;
using Origine.StorageProviders;
using Origine.Accessor;

namespace Origine
{
    [Command]
    [HelpOption("-?")]
    public class Program
    {
        public static IHost Host { get; private set; }
        public static IConfiguration Configuration { get; private set; }

        private static readonly AutoResetEvent siloStopEvent = new AutoResetEvent(false);
        private static readonly object syncLock = new object();
        private static bool siloStopping = false;

        [Option("-S|--siloport", Description = "Silo port")]
        private int? SiloPort { get; set; }

        [Option("-G|--gatewayport", Description = "Gateway port")]
        private int? GatewayPort { get; set; }

        [Option("-E|--endpoint", Description = "Silo endpoint")]
        private string Endpoint { get; set; }

        [Option("-D|--dashboard")]
        private bool Dashboard { get; set; }

        [Option("-H|--healthcheck")]
        private bool HealthCheck { get; set; }

        [Option("-V|--env")]
        private string EnvironmentVariable { get; set; }

        public static int Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args).GetAwaiter().GetResult();

        private async Task<int> OnExecuteAsync()
        {
            try
            {
                Console.Title = nameof(Host);
                Console.CancelKeyPress += OnStopSilo;
                AssemblyLoadContext.Default.Unloading += context => OnStopSilo(context, default);
                Host = await StartSiloAsync();
                ConsoleLogger.WriteLine("Press Ctrl + C|Break key to exit.");
                siloStopEvent.WaitOne();
                return 0;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteError($"{ex.Message},{ex.StackTrace}");
                return 1;
            }
        }

        private async Task<IHost> StartSiloAsync()
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration(BuildAppConfiguration)
                .ConfigureLogging(BuildLogging)
                .ConfigureServices(BuildServices)
                .UseOrleans(BuildOrleans)
                .Build();

            await host.StartAsync();
            return host;
        }

        private void BuildAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
        {
            var environmentName = EnvironmentVariable ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT", EnvironmentVariableTarget.Machine) ?? "Production";

            builder.SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile($"appsettings.{environmentName}.json")
                  .AddJsonFile($"appsettings.json")
                  .AddEnvironmentVariables();
            var settingPath = Path.Combine(Environment.CurrentDirectory, "settings");
            var files = Directory.GetFiles(settingPath, "*.json", SearchOption.AllDirectories);
            Array.ForEach(files, p => builder.AddJsonFile(p));
            Configuration = builder.Build();
        }

        private void BuildOrleans(HostBuilderContext context, ISiloBuilder builder)
        {
            var mongoConfig = Configuration.GetSection(nameof(MongoDBOptions));
            var clusterConfig = Configuration.GetSection(nameof(ClusterOptions));

            var membershipOptions = Configuration.GetSection(nameof(MongoDBMembershipTableOptions));
            var remindableOptions = Configuration.GetSection(nameof(MongoDBRemindersOptions));
            var storageOptions = mongoConfig.Get<MongoDBGrainStorageOptions>();
            var adoNetOptions = Configuration.GetSection("AdoNetOptions").Get<AdoNetGrainStorageOptions>();
            var mongodbConnection = Configuration.GetSection("MongoDBConnection").Get<string>();

            void ConfigureStorage(MongoDBGrainStorageOptions options)
            {
                options.CollectionPrefix = storageOptions.CollectionPrefix;
                options.ClientName = storageOptions.ClientName;
                options.DatabaseName = storageOptions.DatabaseName;
            }

            builder
                .Configure<ClusterOptions>(clusterConfig)
                .Configure<MongoDBOptions>(mongoConfig)
                .Configure<SchedulingOptions>(options => options.PerformDeadlockDetection = true)
                .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory().WithReferences())
                .UseSiloUnobservedExceptionsHandler()

                //EntityFramework
                .AddDBContexts<ApplicationDbContext>(adoNetOptions.ConnectionString)

                //ConsistencyProvider
                .AddLogStorageBasedLogConsistencyProvider()
                .AddStateStorageBasedLogConsistencyProvider()
                .AddCustomStorageBasedLogConsistencyProvider("CustomStorage")

                //MongoDB
                .UseMongoDbAccessor()
                .UseMongoDBMembershipTable(membershipOptions)
                .UseMongoDBReminders(remindableOptions)
                .UseMongoDBClient(mongodbConnection)
                .AddMongoDBGrainStorage("Default", ConfigureStorage)
                .AddMongoDBGrainStorage("PubSubStore", ConfigureStorage)
                .AddMongoDBGrainStorage("MongoDb", ConfigureStorage)

                //SMS
                .AddSimpleMessageStreamProvider("SMS", options => options.FireAndForgetDelivery = true);

            ProcessCommand(context, builder);
        }

        private void ProcessCommand(HostBuilderContext context, ISiloBuilder builder)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                builder.UsePerfCounterEnvironmentStatistics();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                builder.UseLinuxEnvironmentStatistics();

            var siloPort = Configuration.GetValue<int>("SiloPort");
            var gatewayPort = Configuration.GetValue<int>("GatewayPort");

            if (string.IsNullOrWhiteSpace(Endpoint))
                builder.ConfigureEndpoints(SiloPort ?? siloPort, GatewayPort ?? gatewayPort);
            else
                builder.ConfigureEndpoints(Endpoint, SiloPort ?? siloPort, GatewayPort ?? gatewayPort);

            if (Dashboard)
                builder.UseDashboard(options => options.HostSelf = false);
        }

        private void BuildServices(HostBuilderContext context, IServiceCollection services)
        {
            if (HealthCheck)
            {
                services.Configure<ConsoleLifetimeOptions>(options =>
                {
                    options.SuppressStatusMessages = true;
                })
                .Configure<HealthCheckHostedServiceOptions>(options =>
                {
                    options.Port = Configuration.GetValue<int>("HealthCheckPort");
                    options.PathString = "/health";
                })
                .AddHostedService<HealthCheckHostedService>();
            }

            services.ConfigureHandlers<ICommandHandler>(context);
            services.ConfigureExcelFilesReader(Configuration.GetSection(nameof(ExcelReaderOptions)));
            services.Configure<OidcOptions>(Configuration.GetSection(nameof(OidcOptions)));
            services.Configure<ValidateCodeOptions>(Configuration.GetSection(nameof(ValidateCodeOptions)));
        }

        private void BuildLogging(ILoggingBuilder builder)
        {
            builder.AddNLogProvider(Configuration);
        }

        private void OnStopSilo(object sender, ConsoleCancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            lock (syncLock)
            {
                if (siloStopping)
                {
                    return;
                }

                siloStopping = true;

                Task.Run(() =>
                {
                    Host?.StopAsync().GetAwaiter().GetResult();
                    siloStopEvent.Set();
                })
                .Ignore();
            }
        }
    }
}
