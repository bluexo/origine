using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Orleans.Concurrency;
using Origine.Interfaces;

namespace Origine
{
    public interface IHubSession : ISession
    {
        ValueTask<HubSessionContext> GetContext();

        Task SendPacket(IPacket<string> packet);

        Task<IHandlerResult> DispatchPacket(IPacket<string> packet);
    }
}
