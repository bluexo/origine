using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Origine.Gateway.Network.DotNetty.Coder;
using System;
namespace Origine.Gateway.Network.Handler
{
    public class ChannelInitializer : ChannelInitializer<ISocketChannel>
    {
        private readonly IServiceProvider ServiceProvider;

        public ChannelInitializer(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

        /// <summary>
        /// 初始化消息通道
        /// </summary>
        /// <param name="channel"></param>
        protected override void InitChannel(ISocketChannel channel)
        {
            var pipeline = channel.Pipeline;
            pipeline.AddLast("timeout", new IdleStateHandler(0, 60, 0));
            pipeline.AddLast("framing-dec", new MessageDecoder());
            pipeline.AddLast(ServiceProvider.GetService<ChannelHandler>());
            ConsoleLogger.WriteStatus("Initialize channel completely!");
        }
    }
}
