using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using Orleans.Streams;
using Newtonsoft.Json;
using Origine.Interfaces;
using Origine.Network;

namespace Origine.Grains.Network
{
    [Reentrant]
    public class UserHubSession
        : UserSession<HubSessionContext, IHubSession, IJsonCommandHandler, JsonPacket, JsonHandlerResult, string>, IHubSession
    {
        public UserHubSession(IDictionary<short, HandlerInfo> handlerInitializer, ILogger<UserHubSession> logger)
            : base(handlerInitializer, logger)
        {

        }

        protected override string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj);
    }
}
