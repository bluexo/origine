using Orleans.Concurrency;

namespace Origine
{
    public struct DefaultHandlerResult : IHandlerResult
    {
        public byte[] Content { get; set; }

        public StatusDescriptor Status { get; set; }

        public T GetData<T>() where T : class => Content as T;
    }
}
