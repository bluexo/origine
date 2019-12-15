using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace Origine.Gateway.Network.DotNetty.Coder
{
    public class MessageDecoder : ByteToMessageDecoder
    {
        private const int intSize = sizeof(int), shortSize = sizeof(short);

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (input.ReadableBytes < intSize)
                return;
            short length = input.ReadShortLE();
            short command = input.ReadShortLE();

            int dataSize = length - shortSize;
            if (dataSize > input.ReadableBytes)
            {
                input.ResetReaderIndex();
                output.Clear();
                return;
            }

            var pack = new BinaryPacket(command) { Length = length };
            if (dataSize > 0)
            {
                byte[] data = new byte[dataSize];
                input.ReadBytes(data, 0, data.Length);
                input.MarkReaderIndex();
                pack.Data = data;
            }
            output.Add(pack);
            Decode(context, input, output);
        }
    }
}
