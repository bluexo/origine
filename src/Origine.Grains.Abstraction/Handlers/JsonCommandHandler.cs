using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;

using AspectCore.Extensions.Reflection;
using Newtonsoft.Json;
using Origine.Interfaces;
using Origine.Network;

namespace Origine.Grains
{
    [Reentrant]
    public class JsonCommandHandler : CommandHandler<IHubSession, string, JsonHandlerResult>, IJsonCommandHandler
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override object GetParameter(IPacket<string> packet, ParameterReflector reflector, IPlayer player)
        {
            if (reflector.ParameterType == typeof(IPlayer)) return player;
            if (reflector.ParameterType == typeof(string)) return packet.Data;
            if (!string.IsNullOrWhiteSpace(packet.Data))
                return JsonConvert.DeserializeObject(packet.Data, reflector.ParameterType);
            logger.LogWarning($"Command:{packet.Command} invoke invaild parameter:{reflector.ParameterType}");
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override IHandlerResult Respond(StatusDescriptor Status = default, object obj = null)
        {
            if (Status != 0)
                logger.LogWarning($"Response error code {Status} !");

            return new JsonHandlerResult
            {
                Status = Status,
                Content = obj != null ? JsonConvert.SerializeObject(obj) : null
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual IHandlerResult Respond(object obj) => Respond(default, obj);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //protected virtual IHandlerResult Respond(ResponseCode code) => Respond(new StatusDescriptor { StatusCode = (int)code });
    }
}
