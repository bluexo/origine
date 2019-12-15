using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Concurrency;
using Orleans.Configuration;
using Orleans.Runtime;
using Orleans.Streams;

using Newtonsoft.Json;
using Origine.Interfaces;
using Origine.Network;

namespace Origine.Grains.Network
{
    [Reentrant]
    public class UserSocketSession
        : UserSession<SocketSessionContext, ISocketSession, IBinaryCommandHandler, BinaryPacket, DefaultHandlerResult, Memory<byte>>, ISocketSession
    {
        public UserSocketSession(IDictionary<short, HandlerInfo> handlerInitializer, ILogger<UserSocketSession> logger)
            : base(handlerInitializer, logger)
        {
        }

        protected override Memory<byte> Serialize<T>(T obj) => MessagePack.MessagePackSerializer.Serialize(obj).AsMemory();
    }
}
