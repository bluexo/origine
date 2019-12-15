//using System;
//using System.Text;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;

//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.DependencyInjection;
//using AspectCore.Extensions.Reflection;
//using static MessagePack.MessagePackSerializer;

//using Orleans;
//using Orleans.Concurrency;

//using Origine.Interfaces;
//using Origine.Network;

//namespace Origine.Grains
//{
//    [Reentrant]
//    public abstract class BinaryCommandHandler : CommandHandler<ISocketSession, Memory<byte>, DefaultHandlerResult>, IBinaryCommandHandler
//    {
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        protected override object GetParameter(IPacket<Memory<byte>> packet, ParameterReflector reflector, IPlayer player)
//        {
//            if (reflector.ParameterType == typeof(IPlayer)) return player;
//            var array = packet.Data.ToArray();
//            if (array.Length > 0)
//            {
//                return reflector.ParameterType == typeof(string)
//                    ? Encoding.UTF8.GetString(array, 0, packet.Data.Length)
//                    : NonGeneric.Deserialize(reflector.ParameterType, array);
//            }
//            logger.LogWarning($"Command:{packet.Command} invoke invaild parameter:{reflector.ParameterType}");
//            return default;
//        }

//        protected override IHandlerResult Respond(StatusDescriptor code = default, object obj = null) => new DefaultHandlerResult
//        {
//            Status = code,
//            Content = obj != null ? Serialize(obj) : null
//        };

//        protected IHandlerResult Respond(object obj) => Respond(default, obj);
//    }
//}
