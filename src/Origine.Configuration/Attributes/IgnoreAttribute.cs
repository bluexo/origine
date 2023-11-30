using System;
using System.Collections.Generic;
using System.Text;

namespace Origine.Configuration
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Ignore : Attribute { }
}
