using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Streams;

using Origine.Interfaces;
using System.Linq;

namespace Origine.Grains.Network
{
    public class SessionManagerState
    {
        public Dictionary<string, DateTime> Sessions { get; set; } = new Dictionary<string, DateTime>();
    }

    [Reentrant]
    [StorageProvider(ProviderName = "MongoDb")]
    public class SessionManager : PlayerEventSubscriber<SessionManagerState>, ISessionManager
    {
        readonly TimeSpan ExpiredPingTimeSpan = TimeSpan.FromMinutes(1);

        public SessionManager(
            [PersistentState(nameof(SessionManager))]
            IPersistentState<SessionManagerState> state, ILogger<SessionManager> logger) : base(state, logger)
        {
        }

        protected override Task OnPlayerEventAsync((IPlayer player, string eventName) item, StreamSequenceToken token)
        {
            var id = item.player.GetPrimaryKeyString();

            switch (item.eventName)
            {
                case nameof(IPlayer.Offline):
                    _state.State.Sessions.Remove(id);
                    return _state.WriteStateAsync();
                case nameof(IPlayer.Online):
                    _state.State.Sessions[id] = DateTime.Now;
                    return _state.WriteStateAsync();
            }

            return Task.CompletedTask;
        }

        public ValueTask<bool> IsOnline(string id)
        {
            if (!_state.State.Sessions.ContainsKey(id))
                return new ValueTask<bool>(false);

            var diff = DateTime.Now - _state.State.Sessions[id];
            return new ValueTask<bool>(diff <= ExpiredPingTimeSpan);
        }

        public Task Ping(string id)
        {
            _state.State.Sessions[id] = DateTime.Now;
            return Task.CompletedTask;
        }

        public ValueTask<IList<string>> GetOnlinePlayers()
        {
            var playerIds = _state.State.Sessions.Keys.ToList();
            return new ValueTask<IList<string>>(playerIds);
        }
    }
}
