using System;
using System.Buffers;

using SuperSocket.ProtoBase;

namespace StarryNet.ServerModule
{
    public class DataPackage : IKeyedPackageInfo<ushort>
    {
        public ushort Key { get; set; }
        public byte[] Body { get; set; }

        public DataPackage() { }
        public DataPackage(ushort key, byte[] body)
        {
            this.Key = key;
            this.Body = body;
        }
    }

    public class DataFilter : FixedHeaderPipelineFilter<DataPackage>
    {
        public DataFilter() : base(4)
        {
        }

        protected override int GetBodyLengthFromHeader(ref ReadOnlySequence<byte> buffer)
        {
            var reader = new SequenceReader<byte>(buffer);
            reader.Advance(2);
            reader.TryReadBigEndian(out ushort len);
            return len;
        }

        protected override DataPackage DecodePackage(ref ReadOnlySequence<byte> buffer)
        {
            var package = new DataPackage();
            var reader = new SequenceReader<byte>(buffer);
            reader.TryReadBigEndian(out ushort packageKey);
            package.Key = packageKey;
            reader.Advance(2);
            package.Body = reader.UnreadSpan.ToArray();
            return package;
        }
    }

    public class DataDecoder : IPackageDecoder<DataPackage>
    {
        public DataPackage Decode(ref ReadOnlySequence<byte> buffer, object context)
        {
            var package = new DataPackage();
            var reader = new SequenceReader<byte>(buffer);
            reader.TryReadBigEndian(out ushort packageKey);
            package.Key = packageKey;
            reader.Advance(2);
            package.Body = reader.UnreadSpan.ToArray();
            return package;
        }
    }

    public class DataEncoder : IPackageEncoder<DataPackage>
    {
        public int Encode(IBufferWriter<byte> writer, DataPackage pack)
        {
            int size = pack.Body.Length;
            byte[] key = BitConverter.GetBytes(pack.Key);
            byte[] length = BitConverter.GetBytes((ushort)size);
            Array.Reverse(key);
            Array.Reverse(length);
            writer.Write(key);
            writer.Write(length);
            writer.Write(pack.Body);
            return size;
        }
    }
}