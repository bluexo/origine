using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Origine.Configuration;

namespace Origine
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection ConfigureExcelFilesReader(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.Get<ExcelReaderOptions>();
            services.AddSingleton(options);
            return services.AddSingleton<IConfigFilesReader, ExcelConfigFilesReader>();
        }
    }
}
