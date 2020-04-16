using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;
using Origine.Interfaces;

namespace Origine.Grains.HealthCheck
{
    [StatelessWorker(1)]
    public class LocalHealthCheckGrain : Grain, ILocalHealthCheckGrain
    {
        public Task PingAsync() => Task.CompletedTask;
    }
}
