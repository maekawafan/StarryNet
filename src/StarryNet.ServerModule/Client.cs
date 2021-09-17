using MessagePack;
using StarryNet.StarryLibrary;
using SuperSocket.Client;
using System;
using System.Net;
using System.Threading.Tasks;

namespace StarryNet.ServerModule
{
    public class Client
    {
        public IEasyClient<DataPackage> client;
        private IPEndPoint endPoint;
        public bool isConnected { get; private set; }
        private bool isTryConnecting;
        public bool isConnectOrConnecting { get { return isConnected || isTryConnecting; } }

        public Action onConnect;
        public Action onDisconnect;
        public Action<Packet> onData;

        private DateTime lastTryingConnectTime;
        private const float reconnectDelay = 3.0f;

        DataEncoder packageEncoder = new DataEncoder();

        private PacketStorage<dynamic> packetStorage = new PacketStorage<dynamic>();

        public Client(string ip, ushort port)
        {
            this.endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            client = new EasyClient<DataPackage>(new DataFilter()).AsClient();
            client.Closed += OnClose;
            client.PackageHandler += HandlePackage;
        }

        public async ValueTask HandlePackage(EasyClient<DataPackage> sender, DataPackage package)
        {
            try
            {
                Type type = PacketController.GetType(package.Key);
                dynamic pks = MessagePack.MessagePackSerializer.Deserialize(type, package.Body);
                await packetStorage.AddPacket(pks);
            }
            catch
            {
                Log.Error("ClientModule", "패킷 디코드 실패");
            }
        }

        public void DecodePacket()
        {
            foreach (dynamic packet in packetStorage.TakeAllPacket())
                onData?.Invoke(packet);
        }

        public void Connect()
        {
            if (isConnectOrConnecting)
                return;
            if (lastTryingConnectTime.AddSeconds(reconnectDelay) > DateTime.UtcNow)
                return;
            isTryConnecting = true;
            lastTryingConnectTime = DateTime.UtcNow;
            ValueTask<bool> task = client.ConnectAsync(endPoint);
            isConnected = task.Result;
            isTryConnecting = false;
            if (task.Result)
            {
                Log.Info("ClientModule", $"Connect [{endPoint.ToString()}]");
                onConnect?.Invoke();
                client.StartReceive();
            }
            else
                Log.Info("ClientModule", $"Connect Fail [{endPoint.ToString()}]");
        }

        public async ValueTask ConnectAsync()
        {
            if (isConnectOrConnecting)
                return;
            if (lastTryingConnectTime.AddSeconds(reconnectDelay) > DateTime.UtcNow)
                return;
            isTryConnecting = true;
            lastTryingConnectTime = DateTime.UtcNow;
            bool result = await client.ConnectAsync(endPoint);
            isConnected = result;
            isTryConnecting = false;
            if (result)
            {
                onConnect?.Invoke();
                client.StartReceive();
            }
        }

        public void Disconnect()
        {
            if (!isConnected)
                return;
            client.CloseAsync();
        }

        private void OnClose(object sender, EventArgs e)
        {
            isConnected = false;
            onDisconnect?.Invoke();
        }

        public void Send(DataPackage package)
        {
            client.SendAsync(packageEncoder, package);
        }
    }

    public static partial class PacketEx
    {
        public static void SendPacket<T>(this T packet, Client client) where T : Packet
        {
            DataPackage packetPackage = new DataPackage();
            packetPackage.Key = PacketController.GetIndex(typeof(T));
            packetPackage.Body = MessagePackSerializer.Serialize(packet);
            client.Send(packetPackage);
        }
    }
}