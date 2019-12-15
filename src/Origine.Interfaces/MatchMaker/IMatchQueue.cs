using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Origine.Interfaces.MatchMaker
{
    public class MatchQueueState
    {
        public int MatchGroupCount { get; set; } = 2;

        public SortedDictionary<DateTime, IPlayer> Players { get; set; } = new SortedDictionary<DateTime, IPlayer>();
    }

    /// <summary>
    /// 匹配队列
    /// </summary>
    public interface IMatchQueue : IPersistableWithIntegerKey<MatchQueueState>
    {
        /// <summary>
        /// 进入队列
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        ValueTask<bool> AddPlayer(IPlayer player);

        /// <summary>
        /// 退出队列
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        Task RemovePlayer(IPlayer player);
    }
}
