using Orleans.Concurrency;

namespace Origine
{
    [Immutable]
    public struct DefaultHandlerResult : IHandlerResult
    {
        public byte[] Content { get; set; }

        public StatusDescriptor Status { get; set; }

        public T GetData<T>() where T : class => Content as T;
    }
}
