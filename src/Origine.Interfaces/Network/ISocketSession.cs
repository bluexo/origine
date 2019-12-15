using System;
using System.Threading.Tasks;

namespace Origine.Interfaces
{
    public interface ISocketSession : ISession
    {
        ValueTask<SocketSessionContext> GetContext();

        Task SendPacket(IPacket<Memory<byte>> packet);

        Task<IHandlerResult> DispatchPacket(IPacket<Memory<byte>> packet);
    }
}
