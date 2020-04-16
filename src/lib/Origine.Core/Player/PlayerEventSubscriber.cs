using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Streams;
using Orleans.Runtime;
using Origine.Interfaces;
using System.Threading.Tasks;

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

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            _playerEventStream = GetStreamProvider("SMS").GetStream<(IPlayer, string)>(default, nameof(IPlayer));
            _playerEventStreamHandle = await _playerEventStream.SubscribeAsync(OnPlayerEventAsync);
        }

        protected virtual Task OnPlayerEventAsync((IPlayer player, string eventName) item, StreamSequenceToken token)
        {
            return Task.CompletedTask;
        }

        public override async Task OnDeactivateAsync()
        {
            await base.OnDeactivateAsync();
            await _playerEventStreamHandle?.UnsubscribeAsync();
        }
    }
}
