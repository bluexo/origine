using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Streams;
using Origine.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Origine.Grains
{
    [StorageProvider(ProviderName = "MongoDb")]
    public class Player : Grain, IPlayer
    {
        protected readonly IPersistentState<PlayerState> _state;
        protected readonly ILogger _logger;

        protected IAsyncStream<(IPlayer, string)> _stream;

        public Player([PersistentState(nameof(Player))]
                      IPersistentState<PlayerState> state, ILogger<Player> logger)
        {
            _state = state;
            _logger = logger;
        }

        public Task<TSession> GetSession<TSession>() where TSession : ISession
            => Task.FromResult(_state.State.Session.Cast<TSession>());

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _stream = this.GetStreamProvider("SMS").GetStream<(IPlayer, string)>(default, nameof(IPlayer));
            return base.OnActivateAsync(cancellationToken);
        }

        public virtual async Task Online<TSession>(TSession session)
            where TSession : ISession
        {
            _state.State.Session = session;
            await _stream.OnNextAsync((this.AsReference<IPlayer>(), nameof(Online)));
            await _state.WriteStateAsync();
        }

        public virtual async Task Offline()
        {
            _state.State.Session = null;
            await _stream.OnNextAsync((this.AsReference<IPlayer>(), nameof(Offline)));
            await _state.ClearStateAsync();
            DeactivateOnIdle();
        }
    }
}
