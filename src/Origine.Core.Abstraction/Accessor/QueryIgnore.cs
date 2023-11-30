using System;
using System.Collections.Generic;
using System.Text;

namespace Origine
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class QueryIgnore : Attribute
    {
    }
}
