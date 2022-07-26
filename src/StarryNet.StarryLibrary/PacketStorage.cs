using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarryNet.StarryLibrary
{
    public class PacketStorage<T>
    {
        private ConcurrentQueue<T> decodingPackets = new ConcurrentQueue<T>();
        private ConcurrentQueue<T> packetBuffer = new ConcurrentQueue<T>();
        public static object locker = new object();

        public void AddPacket(T packet)
        {
            lock (locker)
            {
                packetBuffer.Enqueue(packet);
            }
        }

        public IEnumerable<T> TakeAllPacket()
        {
            var temp = packetBuffer;
            lock (locker)
            {
                packetBuffer = decodingPackets;
                decodingPackets = temp;
            }

            while (!decodingPackets.IsEmpty)
            {
                T packet;
                if (decodingPackets.TryDequeue(out packet))
                    yield return packet;

            }
        }
    }
}