using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Orleans.Hosting;
using Orleans.Providers.MongoDB.Configuration;

namespace Origine.Accessor
{
    public static class MongoDbExtensions
    {
        public static ISiloBuilder UseMongoDbAccessor(this ISiloBuilder siloBuilder)
        {
            siloBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IDataAccessor, MongoDbDataAccessor>();
            });
            return siloBuilder;
        }
    }
}
