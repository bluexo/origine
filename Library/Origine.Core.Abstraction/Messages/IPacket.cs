using System;
using System.Collections.Generic;
using System.Text;

namespace Origine
{
    public interface IPacket<TData>
    {
        short Command { get; set; }

        StatusDescriptor Status { get; set; }

        TData Data { get; set; }
    }
}
