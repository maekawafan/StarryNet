using System;
using System.Linq;
using System.Net;

using StarryNet.StarryLibrary;
using SuperSocket.ClientEngine;

namespace UnityClientModule
{
    public class Client
    {
        public static Client defaultModule;

        public string name;
        public string targetIP;
        public ushort targetPort;
        public EasyClient<DataPackage> client;
        private Action<dynamic> onData;
        public Action onConnect;
        public Action onDisconnect;
        public Action<string> onError;
        private bool isTryingConnect;
        public bool IsConnected
        {
            get
            {
                if (client == null)
                    return false;
                return client.IsConnected;
            }
        }

        private DateTime lastTryingConnectTime;
        private const float reconnectDelay = 3.0f;

        private PacketStorage<dynamic> packetStorage = new PacketStorage<dynamic>();

        public Client(string name, string targetIP, ushort targetPort, Action<dynamic> onData)
        {
            this.name = name;
            this.targetIP = targetIP;
            this.targetPort = targetPort;
            this.onData = onData;

            client = new EasyClient<DataPackage>();
            client.Initialize(new DataFilter());
            client.NewPackageReceived += OnData;
            client.Connected += OnConnected;
            client.Closed += OnDisconnected;
            client.Error += OnError;
        }

        public void Connect()
        {
            if (IsConnected || isTryingConnect)
                return;
            if (lastTryingConnectTime.AddSeconds(reconnectDelay) > DateTime.UtcNow)
                return;
            lastTryingConnectTime = DateTime.UtcNow;
            isTryingConnect = true;
            Start();
        }

        public void Disconnect()
        {
            client.Close();
        }

        private void Start()
        {
            if (IsConnected)
                client.Close();
            //lastConnectTime = DateTime.Now;
            client.ConnectAsync(new IPEndPoint(IPAddress.Parse(targetIP), targetPort));
        }

        private void OnConnected(object sender, EventArgs e)
        {
            onConnect?.Invoke();
            isTryingConnect = false;
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            onDisconnect?.Invoke();
            isTryingConnect = false;
        }

        private void OnData(object sender, PackageEventArgs<DataPackage> e)
        {
            Type type = PacketController.GetType(e.Package.Key);

            dynamic pks = MessagePack.MessagePackSerializer.Deserialize(type, e.Package.Body);
            packetStorage.AddPacket(pks);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            onError?.Invoke(e.Exception.Message);
            isTryingConnect = false;
        }

        public void SendPacket<T>(T packet) where T : Packet
        {
            ushort index = PacketController.GetIndex(typeof(T));
            byte[] body = MessagePack.MessagePackSerializer.Serialize(packet);
            byte[] buffer = new byte[4 + body.Length];
            Array.Copy(BitConverter.GetBytes(index).Reverse(), 0, buffer, 0, 2);
            Array.Copy(BitConverter.GetBytes((ushort)body.Length).Reverse(), 0, buffer, 2, 2);
            Array.Copy(body, 0, buffer, 4, body.Length);
            client.Send(buffer);
        }

        public void DecodePacket()
        {
            foreach (dynamic packet in packetStorage.TakeAllPacket())
                onData?.Invoke(packet);
        }

        public void SetDefault()
        {
            defaultModule = this;
        }
    }

    public static class PacketEx
    {
        public static void SendPacket<T>(this T packet) where T : Packet
        {
            Client.defaultModule?.SendPacket(packet);
        }
    }
}