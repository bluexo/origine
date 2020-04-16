using Orleans;
using System;

namespace Origine.Grains
{
    /// <summary>
    /// 将 2 ~ 4 个 short 索引值来组合成一个 64 位 key
    /// 通过 key 可以计算出索引值
    /// </summary>
    public readonly ref struct Key64
    {
        public readonly long N1;
        public readonly long N2;
        public readonly long N3;
        public readonly long N4;

        public readonly short Index1;
        public readonly short Index2;
        public readonly short Index3;
        public readonly short Index4;

        private readonly byte[] bytes;

        public Key64(short index1, short index2, short index3 = 0, short index4 = 0)
        {
            bytes = new byte[8];
            var len = sizeof(short);
            Buffer.BlockCopy(BitConverter.GetBytes(index1), 0, bytes, 0, len);
            Index1 = index1;
            N1 = index1;

            Buffer.BlockCopy(BitConverter.GetBytes(index2), 0, bytes, len, len);
            Index2 = index2;
            N2 = BitConverter.ToInt64(bytes, 0);

            Buffer.BlockCopy(BitConverter.GetBytes(index3), 0, bytes, len * 2, len);
            Index3 = index3;
            N3 = BitConverter.ToInt64(bytes, 0);

            Buffer.BlockCopy(BitConverter.GetBytes(index4), 0, bytes, len * 3, len);
            Index4 = index4;
            N4 = BitConverter.ToInt64(bytes, 0);
        }

        public Key64(long key)
        {
            bytes = BitConverter.GetBytes(key);
            var span = bytes.AsSpan();
            var s1 = span.Slice(0, 2);
            var s2 = span.Slice(2, 2);
            var s3 = span.Slice(4, 2);
            var s4 = span.Slice(6, 2);

            Index1 = BitConverter.ToInt16(s1);
            Index2 = BitConverter.ToInt16(s2);
            Index3 = BitConverter.ToInt16(s3);
            Index4 = BitConverter.ToInt16(s4);

            var empty = new byte[8];
            Array.Copy(bytes, 0, empty, 0, 2);
            N1 = Index1;
            Array.Copy(bytes, 2, empty, 2, 2);
            N2 = BitConverter.ToInt64(empty);
            Array.Copy(bytes, 4, empty, 4, 2);
            N3 = BitConverter.ToInt64(empty);
            N4 = key;
        }

        public static implicit operator Key64(long value) => new Key64(value);

        public static implicit operator long(Key64 key) => key.N4;
    }

    public static class Key64Extensions
    {
        public static Key64 GetPrimaryKey64(this IGrainWithIntegerKey key) => key.GetPrimaryKeyLong();
    }
}
