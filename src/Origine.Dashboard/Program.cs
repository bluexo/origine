using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Origine;
using Orleans.Hosting;
using Orleans.Providers.MongoDB.Configuration;

namespace Origine.Control
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder
                    .AddJsonFile($"appsettings.json")
                    .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json")
                    .AddCommandLine(args);
                })
                .UseUnobserveExceptionHandler()
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddNLogProvider(context.Configuration);
                })
                .UseOrleansClient((ctx, builder) =>
                {
                    builder.ConfigureClusterClient(ctx.Configuration);
                    builder.UseMongoDBClient(ctx.Configuration["MongoDBConnection"]);
                    builder.UseMongoDBClustering(ctx.Configuration.GetSection(nameof(MongoDBMembershipTableOptions)));
                    builder.UseConnectionRetryFilter<ClientConnectRetryFilter>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls($"http://*:8000");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
