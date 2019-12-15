using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using LiteNetLib;
using Origine.Gateway.Hosts;
using Origine.Gateway.Network;
using Origine.Gateway.Network.LiteNet;


namespace Origine.Gateway.Extensions
{
    public static class ClientBuilderExtensions
    {
        public static IClientBuilder UseDotNetty(this IClientBuilder clientBuilder)
        {
            clientBuilder.ConfigureServices(s =>
            {
                s.AddSingleton<IHostedService, DotNettyHostService>();
                s.AddTransient<ChannelHandler>();
                s.AddTransient<DotNettyClientObserver>();
            });
            return clientBuilder;
        }

        public static IClientBuilder UseLiteNet(this IClientBuilder clientBuilder)
        {
            clientBuilder.ConfigureServices(s =>
            {
                s.AddSingleton<IHostedService, LiteNetHostService>();
                s.AddSingleton<LiteNetEventHandler>();
                s.AddTransient<LiteNetClientObserver>();
            });
            return clientBuilder;
        }
    }
}
