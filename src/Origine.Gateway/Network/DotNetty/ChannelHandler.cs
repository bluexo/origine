using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Origine.Gateway.Network;
using Origine.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using IChannel = DotNetty.Transport.Channels.IChannel;

namespace Origine.Gateway
{
    public class ChannelHandler : ChannelHandlerAdapter
    {
        public static readonly ConcurrentDictionary<IChannel, DotNettyClientObserver> ClientDictionary
            = new ConcurrentDictionary<IChannel, DotNettyClientObserver>();
        private readonly ILogger<ChannelHandler> Logger;
        private readonly IClusterClient ClusterClient;

        public ChannelHandler(IClusterClient clusterClient, ILogger<ChannelHandler> logger)
        {
            ClusterClient = clusterClient;
            Logger = logger;
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            base.ChannelRegistered(context);
            Task.Factory.StartNew(RegisterChannelAsync, context.Channel);
        }

        private async Task RegisterChannelAsync(object obj)
        {
            var channel = obj as IChannel;
            Guid guid = Guid.NewGuid();
            var stream = ClusterClient.GetStreamProvider("SMS").GetStream<IPacket<Memory<byte>>>(guid, nameof(ISocketSession));
            var userSession = ClusterClient.GetGrain<ISocketSession>(guid);
            var observer = ClusterClient.ServiceProvider.GetRequiredService<DotNettyClientObserver>();
            var handle = await stream.SubscribeAsync(observer);
            observer.Initialize(channel, userSession);
            var sessionContext = new SocketSessionContext
            {
                Id = userSession.GetPrimaryKey(),
                ChannelId = channel.Id.AsLongText(),
                RemoteAddress = channel.RemoteAddress as IPEndPoint,
                StreamHandle = handle
            };
            await userSession.Online(sessionContext);
            if (!ClientDictionary.TryAdd(channel, observer))
            {
                observer.Close();
                throw new InvalidOperationException("现在已存在一个连接实例，请稍后重试！");
            }
            ConsoleLogger.WriteStatus($"{nameof(ChannelRegistered)}, " + $"Remote:{channel.RemoteAddress}," + $"AmountCount:{ClientDictionary.Count}");
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            base.ChannelUnregistered(context);
            if (ClientDictionary.TryRemove(context.Channel, out DotNettyClientObserver observer))
            {
                ConsoleLogger.WriteStatus($"{nameof(ChannelUnregistered)},Remote:{context.Channel.RemoteAddress}, HashCode:{GetHashCode()}");
                observer.Close();
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Logger.LogError(exception, $"{exception.Message},\n {exception.StackTrace}");
        }

        /// <summary>
        /// 消息读取处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            try
            {
                var packet = message as BinaryPacket;
                if (packet.Command == 0)
                {
                    Task.Factory.StartNew(async () => await SendHeartbeatData(context.Channel));
                    return;
                }

                if (ClientDictionary.ContainsKey(context.Channel))
                {
                    var client = ClientDictionary[context.Channel];
                    client.Send(packet);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
            }
            finally
            {
                ReferenceCountUtil.Release(message);
            }
        }

        private async Task SendHeartbeatData(IChannel channel)
        {
            var byteBuffer = PooledByteBufferAllocator.Default.Buffer();
            byteBuffer.WriteShortLE(4);
            byteBuffer.WriteShortLE(0);
            byteBuffer.WriteShortLE(0);
            if (channel.Active && channel.IsWritable)
            {
                await channel.WriteAndFlushAsync(byteBuffer);
            }

            if (!ClientDictionary.ContainsKey(channel)) return;
            var session = ClientDictionary[channel].UserSession;
            await session.GetPlayer();
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent eventState)
            {
                if (eventState != null && eventState.State == IdleState.WriterIdle)
                {
                    var channel = context.Channel;
                    Task.Factory.StartNew(async () => await SendHeartbeatData(channel));
                }
            }
            ConsoleLogger.WriteStatus($"{nameof(UserEventTriggered)},{evt}");
            base.UserEventTriggered(context, evt);
        }
    }
}
