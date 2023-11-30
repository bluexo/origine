using System.Threading.Tasks;
using Orleans;

namespace Origine.Interfaces
{
    public interface IStorageHealthCheckGrain : IGrainWithGuidKey
    {
        Task CheckAsync();
    }
}
