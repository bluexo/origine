using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Orleans;
using Orleans.ApplicationParts;

using Microsoft.Extensions.Configuration;

namespace Origine
{
    public static class ApplicationExtensions
    {
        public static IEnumerable<Type> GetAllAssemblyTypes(this IApplicationPartManager appMgr)
            => appMgr
            .AddFromApplicationBaseDirectory()
            .Assemblies
            .SelectMany(t => t.GetTypes());
    }
}
