using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;

namespace Origine.Interfaces
{

    public interface IPersistableWithGuidKey<TGrainState> : IGrainWithGuidKey where TGrainState : new()
    {
        [AlwaysInterleave]
        ValueTask<TGrainState> GetState();
    }

    public interface IPersistableWithStringKey<TGrainState> : IGrainWithStringKey where TGrainState : new()
    {
        [AlwaysInterleave]
        ValueTask<TGrainState> GetState();
    }

    public interface IPersistableWithIntegerKey<TGrainState> : IGrainWithIntegerKey where TGrainState : new()
    {
        [AlwaysInterleave]
        ValueTask<TGrainState> GetState();
    }

    public interface IPersistableWithGuidCompoundKey<TGrainState> : IGrainWithGuidCompoundKey where TGrainState : new()
    {
        [AlwaysInterleave]
        ValueTask<TGrainState> GetState();
    }

    public interface IPersistableWithIntegerCompoundKey<TGrainState> : IGrainWithIntegerCompoundKey where TGrainState : new()
    {
        [AlwaysInterleave]
        ValueTask<TGrainState> GetState();
    }
}
