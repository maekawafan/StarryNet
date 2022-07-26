using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SuperSocket;
using System;
using System.Collections.Concurrent;
using MessagePack;
using SuperSocket.Channel;
using StarryNet.StarryLibrary;

namespace StarryNet.ServerModule
{
    public enum ServerEvent
    {
        PacketDecodeFail,
    }

    public class Server
    {
        public string serverName { get; private set; }
        public string localIP;
        public string ip;
        public ushort port;

        //public Func<IAppSession, PacketPackage, ValueTask> onData;
        public Action<IAppSession, dynamic> onData;
        public Action<IAppSession, dynamic> onFastData;
        public Func<IAppSession, ValueTask> onConnect;
        public Func<IAppSession, string, ValueTask> onDisconnect;
        public Action<IAppSession, ServerEvent> onEvent;
        public Action<IAppSession, double> onPing;

        public IServer server;

        public ConcurrentDictionary<string, IAppSession> sessions = new ConcurrentDictionary<string, IAppSession>();
        public ConcurrentDictionary<IAppSession, DateTime> lastPing = new ConcurrentDictionary<IAppSession, DateTime>();

        private PacketStorage<(IAppSession, dynamic)> packetStorage = new PacketStorage<(IAppSession, dynamic)>();

        public Server(string serverName, string localIP, string ip, ushort port)
        {
            this.serverName = serverName;
            this.localIP = localIP;
            this.ip = ip;
            this.port = port;
        }

        public async void StartServer()
        {
            var builder = SuperSocketHostBuilder.Create<DataPackage, DataFilter>();
            builder.UsePackageDecoder<DataDecoder>();
            builder.ConfigureSuperSocket(options =>
            {
                options.Name = serverName;
                options.ReceiveBufferSize = 40960;
                options.SendBufferSize = 40960;
                options.MaxPackageLength = 409600;
                options.DefaultTextEncoding = Encoding.Unicode;
                options.ReceiveTimeout = 5000;
                options.SendTimeout = 5000;
                ListenOptions listonOptions = new ListenOptions
                {
                    Ip = localIP,
                    Port = port,
                    NoDelay = true,
                    Security = System.Security.Authentication.SslProtocols.None,
                };
                options.AddListener(listonOptions);
            });

            var host = builder
            .UseSessionHandler(OnConnect, OnDisconnect)
            .UsePackageHandler(OnPackage)
            .ConfigureLogging((hostCtx, loggingBuilder) =>
            {
            //loggingBuilder.AddConsole();
            loggingBuilder.ClearProviders();
            })
            .Build();

            server = host.AsServer();
            Log.Info("ServerModule", $"StartServer {serverName} [Listen={localIP}:{port}]");
            await host.RunAsync();
        }

        public async void StopServer()
        {
            Log.Info("ServerModule", $"StopServer {serverName} [Listen={localIP}:{port}]");
            await server.StopAsync();
        }

        private async ValueTask OnPackage(IAppSession appSession, DataPackage package)
        {
            try
            {
                if (package.Key == 0xFFFF)
                {
                    if (lastPing.TryGetValue(appSession, out var lastPingTime))
                    {
                        DateTime now = DateTime.UtcNow;
                        onPing?.Invoke(appSession, (now - lastPingTime).TotalSeconds);
                    }
                    return;
                }

                var typeTuple = PacketController.GetTypeTuple(package.Key);
                dynamic pks = MessagePack.MessagePackSerializer.Deserialize(typeTuple.type, package.Body);
                if (typeTuple.isFast)
                    onFastData.Invoke(appSession, pks);
                else
                    packetStorage.AddPacket((appSession, pks));
            }
            catch (Exception e)
            {
                Log.Error("ServerModule", $"패킷 디코드 실패 {e.Message}");
                onEvent?.Invoke(appSession, ServerEvent.PacketDecodeFail);
            }
        }

        public void DecodePacket()
        {
            foreach (var pair in packetStorage.TakeAllPacket())
                onData(pair.Item1, pair.Item2);
        }

        private async ValueTask OnConnect(IAppSession appSession)
        {
            sessions.TryAdd(appSession.SessionID, appSession);
            lastPing.TryAdd(appSession, DateTime.UtcNow);
            if (onConnect != null)
                await onConnect.Invoke(appSession);
            Log.Info("ServerModule", $"Connect [{appSession.RemoteEndPoint.ToString()}]");
        }

        private async ValueTask OnDisconnect(IAppSession appSession, CloseEventArgs reason)
        {
            sessions.TryRemove(appSession.SessionID, out appSession);
            lastPing.TryRemove(appSession, out _);
            if (onDisconnect != null)
                await onDisconnect.Invoke(appSession, reason.Reason.ToString());
            Log.Info("ServerModule", $"Disconnect [{appSession.RemoteEndPoint.ToString()}] : {reason.Reason.ToString()}");
        }

        public int GetSessionCount()
        {
            return sessions.Count;
        }

        public IAppSession GetSession(string sessionID)
        {
            IAppSession result;
            sessions.TryGetValue(sessionID, out result);
            return result;
        }

        public void PingAll()
        {
            foreach (var session in sessions.Values)
            {
                session.SendPing();
                lastPing[session] = DateTime.UtcNow;
            }

        }
    }

    //public class UdpServer
    //{
    //    public async void StartServer()
    //    {
    //        var builder = SuperSocketHostBuilder.Create<DataPackage, DataFilter>();
    //        builder.UseUdp();
    //        builder.UsePackageDecoder<DataDecoder>();
    //        builder.ConfigureSuperSocket(options =>
    //        {
    //            options.Name = serverName;
    //            options.ReceiveBufferSize = 40960;
    //            options.SendBufferSize = 40960;
    //            options.MaxPackageLength = 409600;
    //            options.DefaultTextEncoding = Encoding.Unicode;
    //            options.ReceiveTimeout = 5000;
    //            options.SendTimeout = 5000;
    //            ListenOptions listonOptions = new ListenOptions
    //            {
    //                Ip = localIP,
    //                Port = port,
    //                NoDelay = true,
    //                Security = System.Security.Authentication.SslProtocols.None,
    //            };
    //            options.AddListener(listonOptions);
    //        });

    //        var host = builder
    //        .UseSessionHandler(OnConnect, OnDisconnect)
    //        .UsePackageHandler(OnPackage)
    //        .ConfigureLogging((hostCtx, loggingBuilder) =>
    //        {
    //            //loggingBuilder.AddConsole();
    //            loggingBuilder.ClearProviders();
    //        })
    //        .Build();

    //        server = host.AsServer();
    //        Log.Info("ServerModule", $"StartServer {serverName} [Listen={localIP}:{port}]");
    //        await host.RunAsync();
    //    }
    //}

    public static partial class PacketEx
    {
        public static async void SendPacket<T>(this T packet, IAppSession appSession) where T : Packet
        {
            if (appSession == null || appSession.State != SessionState.Connected)
                return;
            try
            {
                DataPackage packetPackage = new DataPackage();
                packetPackage.Key = PacketController.GetIndex(typeof(T));
                packetPackage.Body = MessagePackSerializer.Serialize(packet);
                await appSession.SendAsync(new DataEncoder(), packetPackage);
            }
            catch (Exception e)
            {
                Log.Error($"패킷 전송 실패 {e.Message}");
            }
        }

        public static async void SendPing(this IAppSession session)
        {
            if (session.State != SessionState.Connected)
                return;

            try
            {
                DataPackage packetPackage = new DataPackage();
                packetPackage.Key = 0xFFFF;
                packetPackage.Body = new byte[1] { 0 };
                await session.SendAsync(new DataEncoder(), packetPackage);
            }
            catch (Exception e)
            {
                Log.Error($"핑 전송 실패 {e.Message}");
            }
        }
    }
}
