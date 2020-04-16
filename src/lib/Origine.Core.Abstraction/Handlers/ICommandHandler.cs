using System;
using System.Threading.Tasks;
using Orleans;

namespace Origine.Network
{
    public interface ICommandHandler : IGrainWithGuidKey
    {

    }

    public interface ICommandHandler<TData> : ICommandHandler
    {
        Task<IHandlerResult> Execute(IPacket<TData> packet, IPlayer player = null);
    }
}
