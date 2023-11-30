using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orleans.Metadata;
using AspectCore.Extensions.Reflection;
using Origine.Authorization;

namespace Origine
{
    public class HandlerInfo
    {
        /// <summary>
        /// 处理类型
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 是否授权
        /// </summary>
        public bool Authorize { get; set; }

        /// <summary>
        /// 单向通信
        /// </summary>
        public bool OneWay { get; set; }

        /// <summary>
        /// RPC调用方法
        /// </summary>
        public MethodReflector Reflector { get; set; }
    }

    public static class HandlerCollector
    {
        public static IDictionary<short, HandlerInfo> GetAllHandlers(IEnumerable<Type> handlerTypes)
        {
            var contexts = new ConcurrentDictionary<short, HandlerInfo>();
            foreach (var handlerType in handlerTypes)
                SetHandlerContext(handlerType, contexts);
            return contexts;
        }

        private static void SetHandlerContext(Type handlerType, ConcurrentDictionary<short, HandlerInfo> contexts)
        {
            var attr = handlerType.GetCustomAttribute<RpcAttribute>();
            var authorize = handlerType.GetCustomAttribute<AuthorizeAttribute>() != null;

            if (attr != null)
            {
                var context = new HandlerInfo
                {
                    Authorize = authorize,
                    ClassName = handlerType.FullName,
                };
                contexts.TryAdd(attr.Id, context);
                return;
            }

            var rpcMethods = handlerType
                .GetRuntimeMethods()
                .Select(method =>
                {
                    var rpcAttr = method.GetCustomAttribute<RpcAttribute>();
                    var auth = method.GetCustomAttribute<AuthorizeAttribute>() != null;
                    var allowAnonymous = method.GetCustomAttribute<AllowAnonymousAttribute>() != null;
                    return (attr: rpcAttr, rpcAuthorize: auth, allowAnonymous, method);
                })
                .Where(p => p.attr != null);

            foreach (var rpc in rpcMethods)
            {
                var reflector = rpc.method.GetReflector();
                var context = new HandlerInfo
                {
                    ClassName = handlerType.FullName,
                    Authorize = rpc.allowAnonymous
                        ? false
                        : (rpc.rpcAuthorize ? rpc.rpcAuthorize : authorize),
                    OneWay = rpc.attr.OneWay,
                    Reflector = reflector
                };
                contexts.TryAdd(rpc.attr.Id, context);
            }
        }
    }
}
