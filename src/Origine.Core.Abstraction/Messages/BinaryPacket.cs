using System;
using System.Text;
using Newtonsoft.Json;
using Orleans.Concurrency;

namespace Origine
{
    [Serializable]
    public class BinaryPacket : IPacket<Memory<byte>>
    {
        public short Length { get; set; }

        public short Command { get; set; }

        public Memory<byte> Data { get; set; }

        public StatusDescriptor Status { get; set; }

        public BinaryPacket() { }

        public BinaryPacket(short command) => Command = command;

        public BinaryPacket(short command, byte[] data, StatusDescriptor status = default)
        {
            Command = command;
            Status = status;
            Data = data?.AsMemory() ?? default;
        }
    }
}
