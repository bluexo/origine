using Newtonsoft.Json;

namespace Origine
{
    public class JsonPacket : IPacket<string>
    {
        public short Command { get; set; }
        public string Data { get; set; }
        public StatusDescriptor Status { get; set; }
    }
}
