using System;
using Orleans.Concurrency;

namespace Origine.Interfaces
{
    [Immutable]
    public class SocketSessionContext : SessionContext<IPacket<Memory<byte>>>
    {

    }
}
