using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace Origine.Interfaces.MatchMaker
{
    /// <summary>
    /// 匹配管理器
    /// </summary>
    public interface IMatchManager : IGrainWithIntegerKey
    {
        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="player"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> EnterQueue(IPlayer player, int id);

        Task ExitQueue(IPlayer player);

        Task MatchSucceed(IPlayer player);

        ValueTask<IMatchQueue> GetPlayerQueue(IPlayer player);

        ValueTask<IList<IMatchQueue>> GetQueues();
    }
}
