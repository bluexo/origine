namespace Origine
{
    public interface IHandlerResult
    {
        StatusDescriptor Status { get; set; }

        T GetData<T>() where T : class;
    }
}
