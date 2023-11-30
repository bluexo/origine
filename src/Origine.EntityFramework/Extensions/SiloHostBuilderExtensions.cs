using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Hosting;
using Origine.Models;

namespace Origine.StorageProviders
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConfigureDBContext
    {
        public static ISiloBuilder AddDBContexts<TContext>(this ISiloBuilder builder, string connectionString) where TContext : DbContext
        {
            builder.ConfigureServices((IServiceCollection services) =>
            {
                services.AddDbContextPool<TContext>(b => b.UsePostgresql(connectionString));

                services.AddIdentity<ApplicationUser, IdentityRole>()
                        .AddEntityFrameworkStores<TContext>();

            });

            return builder;
        }
    }
}
