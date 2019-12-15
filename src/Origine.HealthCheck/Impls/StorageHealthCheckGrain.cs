using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Placement;
using Orleans.Providers;
using Orleans.Runtime;
using Origine.Interfaces;

namespace Origine
{
    public class StorageHealthCheckState
    {
        public string Id { get; set; }
    }

    [PreferLocalPlacement]
    [StorageProvider(ProviderName = "MongDb")]
    public class StorageHealthCheckGrain : Grain, IStorageHealthCheckGrain
    {
        private readonly IPersistentState<StorageHealthCheckState> State;

        public StorageHealthCheckGrain(
            [PersistentState(nameof(StorageHealthCheckState))]
            IPersistentState<StorageHealthCheckState> state)
        {
            State = state;
        }

        public async Task CheckAsync()
        {
            try
            {
                State.State = new StorageHealthCheckState { Id = Guid.NewGuid().ToString() };
                await State.WriteStateAsync();
                await State.ReadStateAsync();
                await State.ClearStateAsync();
            }
            finally
            {
                DeactivateOnIdle();
            }
        }
    }
}
