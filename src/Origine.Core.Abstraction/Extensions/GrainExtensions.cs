using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Globalization;

namespace Origine
{
    public static class GrainExtensions
    {
        public static ILogger GetLogger(this Grain grain, IServiceProvider servicesProvider)
        {
            var factory = servicesProvider.GetService<ILoggerFactory>();
            return factory.CreateLogger(grain.GetType());
        }

        public static long ToGrainKeyLong(this string id)
        {
            var grainId = id.Substring(id.IndexOf('=') + 1);
            var n0 = ulong.Parse(grainId.Substring(0, 16), NumberStyles.HexNumber);
            var n1 = ulong.Parse(grainId.Substring(16, 16), NumberStyles.HexNumber);
            return unchecked((long)n1);
        }

        public static Guid ToGrainKeyGuid(this string id)
        {
            var grainId = id.Substring(id.IndexOf('=') + 1);
            var N0 = ulong.Parse(grainId.Substring(0, 16), NumberStyles.HexNumber);
            var N1 = ulong.Parse(grainId.Substring(16, 16), NumberStyles.HexNumber);
            return new Guid((uint)(N0 & 0xffffffff),
                (ushort)(N0 >> 32),
                (ushort)(N0 >> 48),
                (byte)N1, (byte)(N1 >> 8),
                (byte)(N1 >> 16),
                (byte)(N1 >> 24),
                (byte)(N1 >> 32),
                (byte)(N1 >> 40),
                (byte)(N1 >> 48),
                (byte)(N1 >> 56));
        }

        public static string ToGrainKeyString(this string id)
        {
            var splitter = id.IndexOf('+');
            return splitter > 0 ? id.Substring(splitter + 1) : string.Empty;
        }
    }
}
