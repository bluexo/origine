using System;

namespace Origine.Authorization
{
    /// <summary>
    /// 请求需要经过授权
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
    public sealed class AuthorizeAttribute : Attribute { }

    /// <summary>
    /// 允许匿名请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AllowAnonymousAttribute : Attribute { }
}
