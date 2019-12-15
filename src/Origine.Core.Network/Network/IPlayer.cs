using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;

namespace Origine
{
    public class PlayerState
    {
        public ISession Session { get; set; }
    }

    /// <summary>
    /// 玩家接口
    /// </summary>
    public interface IPlayer : IGrainWithStringKey
    {
        /// <summary>
        /// 上线
        /// </summary>
        /// <typeparam name="TSession"></typeparam>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task Online<TSession>(TSession session) where TSession : ISession;

        /// <summary>
        /// 获取会话
        /// </summary>
        /// <typeparam name="TSession"></typeparam>
        /// <returns></returns>
        [AlwaysInterleave]
        Task<TSession> GetSession<TSession>() where TSession : ISession;

        /// <summary>
        /// 下线
        /// </summary>
        /// <returns></returns>
        [OneWay]
        Task Offline();
    }
}
