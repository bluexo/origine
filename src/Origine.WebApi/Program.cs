using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Orleans.Hosting;
using Orleans;
using Orleans.Providers.MongoDB.Configuration;

namespace Origine.WebApi
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
                    var dir = context.HostingEnvironment.ContentRootPath;
                    builder.SetBasePath(dir)
                     .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json")
                     .AddJsonFile($"appsettings.json")
                     .AddEnvironmentVariables();
                })
                .UseOrleansClient((ctx,builder) =>
                {
                    builder.ConfigureClusterClient(ctx.Configuration);
                    builder.UseMongoDBClient(ctx.Configuration["MongoDBConnection"]);
                    builder.UseMongoDBClustering(ctx.Configuration.GetSection(nameof(MongoDBMembershipTableOptions)));
                    builder.UseConnectionRetryFilter<ClientConnectRetryFilter>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls($"http://*:5000;http://*:5001");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
