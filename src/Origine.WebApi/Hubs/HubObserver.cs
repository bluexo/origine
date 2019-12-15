using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using Orleans.Streams;

using Origine.Interfaces;

namespace Origine.WebApi.Hubs
{
    public class HubObserver : IAsyncObserver<IPacket<string>>
    {
        public readonly IClientProxy ClientProxy;
        public readonly IHubSession HubSession;

        public HubObserver(IHubSession hubSession, IClientProxy connectionId)
        {
            HubSession = hubSession;
            ClientProxy = connectionId;
        }

        public Task<IHandlerResult> Send(JsonPacket packet) => HubSession.DispatchPacket(packet);

        public Task OnCompletedAsync() => Task.CompletedTask;

        public Task OnErrorAsync(Exception ex) => Task.CompletedTask;

        public Task OnNextAsync(IPacket<string> item, StreamSequenceToken token = null) => ClientProxy.SendAsync("OnServerMessage", item);

        public void Close()
        {
            HubSession.Close().Ignore();
        }
    }
}
