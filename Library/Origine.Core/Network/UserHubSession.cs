using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using Orleans.Concurrency;

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

        protected override string Serialize<T>(T obj) => JsonSerializer.Serialize(obj);
    }
}
