using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Connections.Features;

using Orleans;
using Orleans.Streams;
using Origine.Interfaces;
using Orleans.Runtime;

namespace Origine.WebApi.Hubs
{
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class LobbyHub : Hub
    {
        readonly ILogger _logger;
        readonly IClusterClient _clusterClient;
        readonly static ConcurrentDictionary<string, HubObserver> Users = new ConcurrentDictionary<string, HubObserver>();

        public LobbyHub(IClusterClient clusterClient, ILogger<LobbyHub> logger)
        {
            _logger = logger;
            _clusterClient = clusterClient;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            _logger.LogInformation($"{Context.UserIdentifier} connected!");
            await ActiveSessionAsync(Context);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            _logger.LogInformation($"{Context.UserIdentifier} disconnected!");

            if (Users.TryRemove(Context.UserIdentifier, out HubObserver observer))
                observer.Close();
        }

        private async Task ActiveSessionAsync(HubCallerContext context)
        {
            var sessionId = context.UserIdentifier.ToGuid();
            var guid = StreamId.Create(nameof(IHubSession), sessionId);
            var stream = _clusterClient.GetStreamProvider("SMS").GetStream<IPacket<string>>(guid);
            var player = _clusterClient.GetGrain<IPlayer>(context.UserIdentifier);
            var userSession = _clusterClient.GetGrain<IHubSession>(sessionId);
            var observer = new HubObserver(userSession, Clients.Client(context.ConnectionId));
            var handle = await stream.SubscribeAsync(observer);
            var feature = context.Features.Get<IHttpContextFeature>();
            var sessionContext = new HubSessionContext
            {
                Id = sessionId,
                ChannelId = context.ConnectionId,
                StreamHandle = handle,
                RemoteAddress = new IPEndPoint(feature.HttpContext.Connection.RemoteIpAddress, feature.HttpContext.Connection.RemotePort)
            };
            await userSession.Online(sessionContext, player);
            if (!Users.TryAdd(context.UserIdentifier, observer))
                observer.Close();

            var hearbeat = Context.Features.Get<IConnectionHeartbeatFeature>();
            hearbeat.OnHeartbeat(ctx => Task.Factory.StartNew(OnClientPing, ctx), Context);
        }

        public async Task OnClientPing(object state)
        {
            if (!(state is HubCallerContext context))
                return;
            var uid = context.UserIdentifier;
            if (!Users.ContainsKey(uid))
                return;
            var sessionManager = _clusterClient.GetGrain<ISessionManager>(0);
            await sessionManager.Ping(uid);
        }

        /// <summary>
        /// 消息读取处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        public async Task<JsonPacket> OnClientMessage(JsonPacket packet)
        {
            if (!Users.TryGetValue(Context.UserIdentifier, out HubObserver client))
            {
                _logger.LogWarning($"Cannot found user {Context.UserIdentifier} , ConnectionId {Context.ConnectionId}");
                return default;
            }
            var result = await client.Send(packet);
            return new JsonPacket
            {
                Command = packet.Command,
                Data = result?.GetData<string>(),
                Status = result?.Status ?? StatusDescriptor.Status503ServiceUnavailable
            };
        }
    }
}
