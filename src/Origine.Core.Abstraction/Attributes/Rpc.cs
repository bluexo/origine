using System;

namespace Origine.Authorization
{
    /// <summary>
    /// 请求需要经过授权
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute { }

    /// <summary>
    /// 允许匿名请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute { }

    /// <summary>
    /// 远程调用
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
    public class RpcAttribute : Attribute
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
