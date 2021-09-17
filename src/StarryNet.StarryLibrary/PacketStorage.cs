using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarryNet.StarryLibrary
{
    public class PacketStorage<T>
    {
        private ConcurrentQueue<T> decodingPackets = new ConcurrentQueue<T>();
        private ConcurrentQueue<T> packetBuffer = new ConcurrentQueue<T>();
        public bool locked = false;

        public async ValueTask AddPacket(T packet)
        {
            await WaitLock();
            packetBuffer.Enqueue(packet);
        }

        private async ValueTask WaitLock()
        {
            do
            {
                if (!locked)
                    return;
                await Task.Delay(1);
            } while (true);
        }

        public IEnumerable<T> TakeAllPacket()
        {
            var temp = packetBuffer;
            locked = true;
            packetBuffer = decodingPackets;
            decodingPackets = temp;
            locked = false;

            while (!decodingPackets.IsEmpty)
            {
                T packet;
                if (decodingPackets.TryDequeue(out packet))
                    yield return packet;
            }
        }
    }
}