using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Origine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using IChannel = DotNetty.Transport.Channels.IChannel;

namespace Origine.Gateway.Network
{
    public abstract class ClientObserver<TConnection, TPacket> : IAsyncObserver<TPacket>
    {
        public TConnection Connection { get; private set; }
        public ISocketSession UserSession { get; private set; }

        protected readonly ActionBlock<TPacket> ReadActionBlock, WriteActionBlock;
        protected readonly ILogger Logger;

        public ClientObserver(IClusterClient clusterClient, ILogger logger)
        {
            var options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = sbyte.MaxValue };
            WriteActionBlock = new ActionBlock<TPacket>(WriteMessage, options);
            ReadActionBlock = new ActionBlock<TPacket>(ReadMessage, options);
            Logger = logger;
        }

        public void Initialize(TConnection channel, ISocketSession userSession)
        {
            Connection = channel;
            UserSession = userSession;
        }

        protected abstract Task ReadMessage(TPacket packet);

        /// <summary>
        /// 写入消息
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected abstract Task WriteMessage(TPacket packet);

        public void Send(TPacket packet) => Post(ReadActionBlock, packet);

        private void Post(ActionBlock<TPacket> actionBlock, TPacket packet)
        {
            actionBlock.Post(packet);

            if (actionBlock.InputCount > sbyte.MaxValue)
            {
                Logger.LogError($"Client message overload and will be close !");
                Close();
            }
        }

        public virtual Task OnNextAsync(TPacket item, StreamSequenceToken token = null)
        {
            Post(WriteActionBlock, item);
            return Task.CompletedTask;
        }

        public virtual Task OnCompletedAsync()
        {
            Logger.LogWarning($"{nameof(DotNettyClientObserver)} receive completed!");
            return Task.CompletedTask;
        }

        public virtual Task OnErrorAsync(Exception ex)
        {
            Logger.LogError(ex, $"{nameof(DotNettyClientObserver)} receive error: {ex.Message},{ex.StackTrace}");
            return Task.CompletedTask;
        }

        public virtual void Close()
        {
            ReadActionBlock.Complete();
            WriteActionBlock.Complete();
            UserSession.Close().Ignore();
            Logger.LogWarning($"Client {Connection.ToString()} closed!");
        }
    }
}
