using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Origine.StorageProviders
{
    public static class DbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder UsePostgresql(this DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            var migrationsAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;
            optionsBuilder.UseNpgsql(connectionString, o =>
            {
                o.MigrationsAssembly(migrationsAssembly);
            });
            return optionsBuilder;
        }
    }
}
