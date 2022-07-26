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
        private Action<dynamic> onFastData;
        public Action onConnect;
        public Action onDisconnect;
        public Action<string> onError;
        public bool isTryingConnect { get; private set; }
        public bool IsConnected
        {
            get
            {
                if (client == null)
                    return false;
                return client.IsConnected;
            }
        }

        public bool IsNeedConnect => !isTryingConnect && !IsConnected;
        private DateTime lastTryingConnectTime;
        private const float reconnectDelay = 3.0f;

        private PacketStorage<dynamic> packetStorage = new PacketStorage<dynamic>();

        public Client(string name, string targetIP, ushort targetPort, Action<dynamic> onData, Action<dynamic> onFastData)
        {
            this.name = name;
            this.targetIP = targetIP;
            this.targetPort = targetPort;
            this.onData = onData;
            this.onFastData = onFastData;

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
            if (e.Package.Key == 0xFFFF)
            {
                SendPing();
                return;
            }

            var typeTuple = PacketController.GetTypeTuple(e.Package.Key);
            dynamic pks = MessagePack.MessagePackSerializer.Deserialize(typeTuple.type, e.Package.Body);
            if (typeTuple.isFast)
                onFastData.Invoke(pks);
            else
                packetStorage.AddPacket(pks);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            onError?.Invoke(e.Exception.Message);
            isTryingConnect = false;
        }

        public void SendPacket<T>(T packet) where T : Packet
        {
            if (!client.IsConnected)
                return;
            byte[] buffer = null;
            ushort index = 0;
            try
            {
                index = PacketController.GetIndex(typeof(T));
                byte[] body = MessagePack.MessagePackSerializer.Serialize(packet);
                //if (index >= body.Length)
                //    return;
                buffer = new byte[4 + body.Length];
                Array.Copy(BitConverter.GetBytes(index).Reverse(), 0, buffer, 0, 2);
                Array.Copy(BitConverter.GetBytes((ushort) body.Length).Reverse(), 0, buffer, 2, 2);
                Array.Copy(body, 0, buffer, 4, body.Length);
                if (client.IsConnected)
                    client.Send(buffer);
            }
            catch (Exception e)
            {
                Log.Error($"패킷 전송 실패 : [{nameof(T)}] {e.Message}\nIndex : [{index}]\n");
                Log.Error($"Resolver : {(MessagePack.MessagePackSerializer.DefaultOptions.Resolver.GetFormatter<T>() == null ? "Null" : "Not Null")}");
                if (buffer != null)
                    Log.Error($"Byte : [{buffer.ToString()}]");
            }
        }

        public void DecodePacket()
        {
            foreach (dynamic packet in packetStorage.TakeAllPacket())
            {
                onData?.Invoke(packet);
            }
        }

        public void SetDefault()
        {
            defaultModule = this;
        }

        public void SendPing()
        {
            if (!IsConnected)
                return;

            try
            {
                ushort length = 0;
                byte[] buffer = new byte[4];
                Array.Copy(BitConverter.GetBytes(0xFFFF), 0, buffer, 0, 2);
                Array.Copy(BitConverter.GetBytes(length), 0, buffer, 2, 2);
                if (client.IsConnected)
                    client.Send(buffer);
            }
            catch (Exception e)
            {
                Log.Error($"핑 전송 실패 {e.Message}");
            }
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