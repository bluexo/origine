using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Origine
{
    public interface ISessionContext
    {
        /// <summary>
        /// ID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 通道
        /// </summary>
        string ChannelId { get; }

        /// <summary>
        /// 远程地址 
        /// </summary>
        IPEndPoint RemoteAddress { get; }
    }
}
