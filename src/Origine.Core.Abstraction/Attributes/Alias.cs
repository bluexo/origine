using System;

namespace Origine
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class Alias : Attribute
    {
        public string Name { get; set; }

        public Alias(string flag) => Name = flag;
    }
}
