using Orleans;
using System.Collections.Generic;

namespace Origine.Interfaces
{
    public class PlayerEqualityComparer : IEqualityComparer<IPlayer>
    {
        public bool Equals(IPlayer x, IPlayer y) => x.GetPrimaryKeyLong() == y.GetPrimaryKeyLong();

        public int GetHashCode(IPlayer obj) => obj.GetPrimaryKeyLong().GetHashCode();
    }
}
