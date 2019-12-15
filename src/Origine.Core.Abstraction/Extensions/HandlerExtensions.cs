using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;

namespace Origine
{
    public static class HandlerExtensions
    {
        public static IServiceCollection ConfigureHandlers<THandler>(this IServiceCollection services, HostBuilderContext context)
        {
            var types = context.GetApplicationPartManager().GetAllAssemblyTypes();
            var handlerTypes = types.Where(t => t.GetInterfaces().Any(i => i == typeof(THandler))).ToList();
            services.AddSingleton(HandlerCollector.GetAllHandlers(handlerTypes));
            return services;
        }
    }
}
