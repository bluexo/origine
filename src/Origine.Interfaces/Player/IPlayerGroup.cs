using Orleans;
using Orleans.Concurrency;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Origine.Interfaces
{
    public interface IPlayerGroup : IGrainWithStringKey
    {

        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        Task<bool> AddPlayer(IPlayer player);
        
        /// <summary>
        /// 批量添加玩家, 必须在当前玩家数量为空的状态使用
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        Task<bool> AddPlayers(IList<IPlayer> players);

        /// <summary>
        /// 移除玩家
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        Task RemovePlayer(IPlayer player);

        /// <summary>
        /// 获取玩家
        /// </summary>
        /// <returns></returns>
        [AlwaysInterleave]
        Task<List<IPlayer>> GetPlayers();

        /// <summary>
        /// 是否存在玩家
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        Task<bool> IsExistPlayer(IPlayer player);
    }
}
