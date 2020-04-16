using System;

namespace Origine.Authorization
{
    /// <summary>
    /// 远程调用
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class RpcAttribute : Attribute
    {
        public bool OneWay { get; set; }
        public short Id { get; set; }

        public RpcAttribute(short id = 0, bool oneWay = false)
        {
            Id = id;
            OneWay = oneWay;
        }
    }

}
