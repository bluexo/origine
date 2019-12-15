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
    public class DotNettyClientObserver : ClientObserver<IChannel, IPacket<Memory<byte>>>
    {
        public DotNettyClientObserver(IClusterClient clusterClient, ILogger<DotNettyClientObserver> logger)
            : base(clusterClient, logger)
        {
        }

        protected override Task ReadMessage(IPacket<Memory<byte>> packet) => UserSession.DispatchPacket(packet);

        /// <summary>
        /// 写入消息
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected override async Task WriteMessage(IPacket<Memory<byte>> packet)
        {
            var byteBuffer = PooledByteBufferAllocator.Default.Buffer();
            byteBuffer.WriteShortLE(packet.Data.Length + 4);
            byteBuffer.WriteShortLE(packet.Command);
            byteBuffer.WriteShortLE(packet.Status);
            var array = packet.Data.ToArray();
            if (array != null)
            {
                byteBuffer.WriteBytes(array, 0, packet.Data.Length);
            }
            if (Connection.Active && Connection.IsWritable)
            {
                await Connection.WriteAndFlushAsync(byteBuffer);
            }
            else
            {
                ReferenceCountUtil.Release(byteBuffer);
            }
        }

        public override void Close()
        {
            ReadActionBlock.Complete();
            WriteActionBlock.Complete();
            UserSession.Close().Ignore();
            Logger.LogWarning($"Client {Connection.RemoteAddress} closed!");
        }
    }
}
