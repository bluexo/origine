using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Mongo;
using NLog.Extensions.Logging;
using LogLevel = NLog.LogLevel;

using Origine.Configuration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Origine
{
    public static class LoggingExtensions
    {
        public static NLogLoggingConfiguration UseMongoTarget(this NLogLoggingConfiguration nlogConf, IConfiguration configuration)
        {
            var targetOption = configuration.GetSection(nameof(NLogNoSqlTargetOptions)).Get<NLogNoSqlTargetOptions>();
            var mongoOption = configuration.GetSection(nameof(LogStorageOptions)).Get<LogStorageOptions>();

            if (targetOption == null || mongoOption == null)
            {
                throw new NullReferenceException($"Cannot found {nameof(NLogNoSqlTargetOptions)} or {nameof(LogStorageOptions)} from configuration file!");
            }

            var mongoTarget = new MongoTarget()
            {
                Name = targetOption.Name,
                CollectionName = targetOption.CollectionName,
                ConnectionString = configuration["MongoDBConnection"],
                DatabaseName = mongoOption.DatabaseName,
                IncludeDefaults = true
            };

            nlogConf.AddTarget(mongoTarget);

            foreach (var r in targetOption.Rules)
            {
                var minlevel = LogLevel.FromString(r.MinLevel);
                var maxLevel = LogLevel.FromString(r.MaxLevel);
                nlogConf.AddRule(minlevel, maxLevel, mongoTarget, r.Logger);
            }

            return nlogConf;
        }

        public static ILoggingBuilder AddNLogProvider(this ILoggingBuilder builder, IConfiguration configuration)
        {
            builder.AddNLog(new NLogProviderOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true
            });

            var nlogConf = configuration.GetSection(nameof(NLog));
            LogManager.Configuration = new NLogLoggingConfiguration(nlogConf).UseMongoTarget(configuration);
            return builder;
        }

        public static IServiceCollection AddMongoDbLoggingProvider(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoOption = configuration.GetSection(nameof(LogStorageOptions)).Get<LogStorageOptions>();
            var targetOption = configuration.GetSection(nameof(NLogNoSqlTargetOptions)).Get<NLogNoSqlTargetOptions>();
            var provider = new DefaultLoggingQueryProvider(configuration["MongoDBConnection"], mongoOption.DatabaseName, targetOption.CollectionName);
            services.AddSingleton<LoggingQueryProvider>(provider);
            return services;
        }
    }
}
