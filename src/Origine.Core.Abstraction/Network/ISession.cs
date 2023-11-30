using Orleans;
using System.Threading.Tasks;

namespace Origine
{
    public interface ISession : IGrainWithGuidKey
    {
        ValueTask<IPlayer> GetPlayer();

        Task Send<T>(short cmd, T obj);

        Task Online<TContext>(TContext context, IPlayer user = null) where TContext : ISessionContext;

        Task Offline(bool deactive = true);

        Task Close();
    }
}
