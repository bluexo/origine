using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Origine.StorageProviders.Migrations
{
    /// <summary>
    /// 设计时
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("dbsettings.json")
               .Build();

            var connection = configuration.GetValue<string>("ConnectionString");
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UsePostgresql(connection);
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
