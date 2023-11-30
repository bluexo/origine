using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Streams;
using Orleans.Runtime;
using Origine.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Origine.Grains
{
    /// <summary>
    /// 玩家事件订阅方基类,继承此类可以接受玩家相关事件
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public class PlayerEventSubscriber<TState> : Grain where TState : new()
    {
        protected readonly ILogger _logger;
        protected readonly IPersistentState<TState> _state;
        protected StreamSubscriptionHandle<(IPlayer, string)> _playerEventStreamHandle;
        protected IAsyncStream<(IPlayer, string)> _playerEventStream;

        public PlayerEventSubscriber(IPersistentState<TState> state, ILogger logger)
        {
            _state = state;
            _logger = logger;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
            _playerEventStream = this.GetStreamProvider("SMS").GetStream<(IPlayer, string)>(default, nameof(IPlayer));
            _playerEventStreamHandle = await _playerEventStream.SubscribeAsync(OnPlayerEventAsync);
        }

        protected virtual Task OnPlayerEventAsync((IPlayer player, string eventName) item, StreamSequenceToken token)
        {
            return Task.CompletedTask;
        }

        public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            await base.OnDeactivateAsync(reason, cancellationToken);
            await _playerEventStreamHandle?.UnsubscribeAsync();
        }
    }
}
