using Orleans.Streams;

using System;
using System.Net;

namespace Origine
{
    public class SessionContext<TPacket> : ISessionContext
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 通道
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// 远程地址 
        /// </summary>
        public IPEndPoint RemoteAddress { get; set; }

        /// <summary>
        /// 流句柄
        /// </summary>
        public StreamSubscriptionHandle<TPacket> StreamHandle { get; set; }

        public string RemoteAddressIPv4String() => RemoteAddress?.Address?.MapToIPv4()?.ToString();
    }
}
