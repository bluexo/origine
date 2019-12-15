using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace Origine
{
    /// <summary>
    /// 会话管理器
    /// </summary>
    public interface ISessionManager : IGrainWithIntegerKey
    {
        /// <summary>
        /// 玩家是否在线
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ValueTask<bool> IsOnline(string id);

        /// <summary>
        /// 获取所有在线玩家
        /// </summary>
        /// <returns></returns>
        ValueTask<IList<string>> GetOnlinePlayers();

        /// <summary>
        /// 玩家定时确认在线状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OneWay]
        Task Ping(string id);
    }
}
