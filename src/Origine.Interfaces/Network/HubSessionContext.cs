using Orleans.Concurrency;

namespace Origine.Interfaces
{
    [Immutable]
    public class HubSessionContext : SessionContext<IPacket<string>>
    {

    }
}
