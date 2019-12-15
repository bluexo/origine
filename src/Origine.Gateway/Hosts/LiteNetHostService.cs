using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Orleans;
using LiteNetLib;
using LiteNetLib.Utils;
using Origine.Gateway.Network.LiteNet;
using Origine.Interfaces;

namespace Origine.Gateway.Hosts
{
    public class LiteNetHostService : IHostedService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly NetManager Server;

        public LiteNetHostService(LiteNetEventHandler handler, IConfigurationRoot configuration)
        {
            _configuration = configuration;
            Server = handler.NetManager;
            Server.AutoRecycle = true;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var cmdPort = _configuration.GetSection("port").Get<int?>();
            var port = cmdPort ?? _configuration.GetSection("HostPort").Get<int>();
            Server.Start(IPAddress.Any, IPAddress.IPv6Any, port);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Server.Stop(true);
            return Task.CompletedTask;
        }
    }
}
