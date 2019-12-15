using System.Threading.Tasks;
using Orleans;

namespace Origine.Interfaces
{
    public interface ILocalHealthCheckGrain : IGrainWithGuidKey
    {
        Task PingAsync();
    }
}
