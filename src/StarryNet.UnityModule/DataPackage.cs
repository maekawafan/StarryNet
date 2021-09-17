using System;
using StarryNet.StarryLibrary;
using SuperSocket.ProtoBase;

namespace UnityClientModule
{
    public class DataPackage : IPackageInfo<ushort>
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

    public class DataFilter : FixedHeaderReceiveFilter<DataPackage>
    {
        public DataFilter() : base(4)
        {
        }

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            bufferStream.Skip(2);
            return bufferStream.ReadUInt16();
        }

        public override DataPackage ResolvePackage(IBufferStream bufferStream)
        {
            try
            {
                var package = new DataPackage();
                ushort packageKey = bufferStream.ReadUInt16();
                package.Key = packageKey;
                ushort bodySize = bufferStream.ReadUInt16();
                package.Body = new byte[bodySize];
                bufferStream.Read(package.Body, 0, bodySize);
                return package;
            }
            catch (Exception e)
            {
                byte[] buffer = new byte[bufferStream.Length];
                bufferStream.Read(buffer, 0, (int)bufferStream.Length);
                Log.Error($"{e.Message}\nbinary [{BitConverter.ToString(buffer).Replace("-", " ")}]");
                return null;
            }
        }
    }
}