using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;

using AspectCore.Extensions.Reflection;

namespace Origine.Network
{
    public abstract class CommandHandler<TSession, TData, THandlerResult> : Grain
        where TSession : ISession
        where THandlerResult : IHandlerResult, new()
    {
        protected TSession session;
        protected ILogger logger;
        protected IDictionary<short, HandlerInfo> handlers;

        public override async Task OnActivateAsync()
        {
            logger = ServiceProvider.GetService<ILoggerFactory>().CreateLogger(GetType());
            handlers = ServiceProvider.GetService<IDictionary<short, HandlerInfo>>();
            session = GrainFactory.GetGrain<TSession>(this.GetPrimaryKey());
            await base.OnActivateAsync();
        }

        public virtual async Task<IHandlerResult> Execute(IPacket<TData> packet, IPlayer player = null)
        {
            IHandlerResult handlerResult = null;
            object result = null;
            var reflector = handlers[packet.Command].Reflector;
            var parameters = reflector.ParameterReflectors;
            if (parameters.Length > 0)
            {
                var args = Array.ConvertAll(parameters, r => GetParameter(packet, r, player));
                result = reflector.Invoke(this, args);
            }
            else result = reflector.Invoke(this);
            switch (result)
            {
                case Task<StatusDescriptor> codeTask:
                    handlerResult = new THandlerResult { Status = await codeTask };
                    break;
                case Task<IHandlerResult> resultTask:
                    handlerResult = await resultTask;
                    break;
                case Task emptyTask:
                    await emptyTask;
                    handlerResult = new THandlerResult();
                    break;
                default: throw new FormatException($"Command:{packet.Command} invoke invaild return type : {result.GetType()}");
            }
            return handlerResult;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract object GetParameter(IPacket<TData> packet, ParameterReflector reflector, IPlayer player);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract IHandlerResult Respond(StatusDescriptor code = default, object obj = null);

    }
}
