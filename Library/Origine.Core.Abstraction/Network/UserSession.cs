using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using Orleans.Streams;
using Origine.Interfaces;

namespace Origine.Network
{
    [Reentrant]
    public abstract class UserSession<TSessionContext, TSession, TCommandHandler, TPacket, THandlerResult, TData> : Grain
        where TSessionContext : SessionContext<IPacket<TData>>
        where TSession : ISession
        where TCommandHandler : class, ICommandHandler<TData>
        where TPacket : IPacket<TData>, new()
        where THandlerResult : IHandlerResult, new()
    {
        private struct HandlerDescriptor
        {
            public TCommandHandler Handler { get; set; }
            public HandlerInfo Info { get; set; }
        }

        private Dictionary<short, HandlerDescriptor> handlers = new Dictionary<short, HandlerDescriptor>();
        private readonly IDictionary<short, HandlerInfo> HandlerInitializer;
        private readonly ILogger _logger;
        protected IAsyncStream<IPacket<TData>> asyncStream;
        protected TSessionContext sessionContext;
        protected IPlayer player;

        public UserSession(IDictionary<short, HandlerInfo> handlerInitializer, ILogger logger)
        {
            HandlerInitializer = handlerInitializer;
            _logger = logger;
        }

        public ValueTask<IPlayer> GetPlayer() => new ValueTask<IPlayer>(player);

        public ValueTask<TSessionContext> GetContext() => new ValueTask<TSessionContext>(sessionContext);

        public virtual Task Online<TContext>(TContext sessionContext, IPlayer player = null) where TContext : ISessionContext
        {
            var context = sessionContext as TSessionContext;
            this.sessionContext = context ?? throw new InvalidCastException($"Cannot cast {typeof(TContext)} to {typeof(TSession).Name}");
            this.player = player;
            return player.Online(this.AsReference<TSession>());
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            asyncStream = GetStreamProvider("SMS").GetStream<IPacket<TData>>(this.GetPrimaryKey(), typeof(TSession).Name);
            handlers = HandlerInitializer
                .ToDictionary(pair => pair.Key, pair =>
                new HandlerDescriptor
                {
                    Info = pair.Value,
                    Handler = GrainFactory.GetGrain<TCommandHandler>(this.GetPrimaryKey(), pair.Value.ClassName)
                });
        }

        protected abstract TData Serialize<T>(T obj);

        public virtual Task Send<T>(short packetId, T obj = default)
        {
            var packet = new TPacket
            {
                Command = packetId,
                Data = obj != null ? Serialize(obj) : default
            };
            return asyncStream.OnNextAsync(packet);
        }

        public virtual Task SendPacket(IPacket<TData> packet) => asyncStream.OnNextAsync(packet);

        public virtual Task SendBatchPackets(ArraySegment<IPacket<TData>> packets) => asyncStream.OnNextBatchAsync(packets);

        public virtual async Task Offline(bool deactive = true)
        {
            if (player != null)
            {
                await player.Offline();
                player = null;
            }

            if (deactive)
            {
                DeactivateOnIdle();
            }
        }

        public virtual async Task<IHandlerResult> DispatchPacket(IPacket<TData> packet)
        {
            if (!handlers.ContainsKey(packet.Command))
            {
                _logger.LogWarning($"Cannot found commmand:[{packet.Command}]");
                return default;
            }
            var descriptor = handlers[packet.Command];
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Dispatch command {packet.Command} , handler[{descriptor.Info.ClassName}");
            try
            {
                if (player == null && descriptor.Info.Authorize)
                    return new THandlerResult { Status = StatusDescriptor.Status401Unauthorized };
                if (descriptor.Info.OneWay)
                {
                    descriptor.Handler.InvokeOneWay(handler => handler.Execute(packet, player));
                    return new THandlerResult();
                }
                else
                { 
                    return await descriptor.Handler.Execute(packet, player);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(0, $"Handle command [{packet.Command}] occurs error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return default;
            }
        }

        /// <summary>
        /// 关闭并且释放会话
        /// </summary>
        /// <returns></returns>
        public virtual async Task Close()
        {
            if (sessionContext.StreamHandle != null)
            {
                _logger.LogWarning($"User session closed and unsubscribe streamhandle {sessionContext.StreamHandle.ProviderName}!");
                await sessionContext.StreamHandle.UnsubscribeAsync();
            }
            await Offline();
        }
    }
}
