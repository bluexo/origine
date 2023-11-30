using System;
using Orleans.Concurrency;

namespace Origine.Interfaces
{
    public class SocketSessionContext : SessionContext<IPacket<Memory<byte>>>
    {

    }
}
