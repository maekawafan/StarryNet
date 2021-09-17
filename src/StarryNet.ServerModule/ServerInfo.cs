using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using StarryNet.StarryLibrary;
using MessagePack;
using SuperSocket;

namespace StarryNet.ServerModule
{
    public class ServerInfo
    {
        public ushort serverID;
        public ServerType serverType;
        public string internalIP;
        public ushort internalPort;
        public string externalIP;
        public ushort externalPort;
        public IAppSession session { get; private set; }

        public static Dictionary<ushort, ServerInfo> serverList = new Dictionary<ushort, ServerInfo>();
        private static Dictionary<IAppSession, ServerInfo> sessionToServerInfo = new Dictionary<IAppSession, ServerInfo>();
        public static ServerInfo self;

        public ServerInfo(ushort serverID, ServerType serverType, string internalIP, ushort internalPort, string externalIP, ushort externalPort)
        {
            this.serverID = serverID;
            this.serverType = serverType;
            this.internalIP = internalIP;
            this.internalPort = internalPort;
            this.externalIP = externalIP;
            this.externalPort = externalPort;
        }

        public ServerInfo(ushort serverID, string serverType, string internalIP, ushort internalPort, string externalIP, ushort externalPort)
            : this(serverID, Enum.Parse<ServerType>(serverType), internalIP, internalPort, externalIP, externalPort)
        { }

        public static async Task GetAllServerInfo(DB db, string myIP, ushort myPort)
        {
            serverList.Clear();
            await db.RunProcedureAsync("GetAllServerInfo",
            (reader) =>
            {
                do
                {
                    ushort serverID = reader.GetUInt16(0);
                    string serverType = reader.GetString(1);
                    string internalIP = reader.GetString(2);
                    ushort internalPort = reader.GetUInt16(3);
                    string externalIP = reader.GetString(4);
                    ushort externalPort = reader.GetUInt16(5);
                    ServerInfo info = new ServerInfo(serverID, serverType, internalIP, internalPort, externalIP, externalPort);
                    lock (serverList)
                    {
                        serverList.Add(serverID, info);
                    }
                    if (internalIP == myIP && internalPort == myPort)
                        self = info;
                } while (reader.Read());
            });
        }

        public static async Task GetAllServerInfo(string myIP, ushort myPort)
        {
            DB db = new DB("db.maekawamiku.com", 13939, "maekawamiku", "Asterisk03!", "ProjectER_Front");
            await GetAllServerInfo(db, myIP, myPort);
        }

        public static ServerInfo FindServerInfo(Server internalServer, Server externalServer)
        {
            if (internalServer == null && externalServer == null)
                return null;

            if (serverList == null)
            {
                Log.Error("ServerInfo", $"서버 리스트가 로딩되지 않은 상태에서 FindServerInfo()가 호출되었습니다.");
                return null;
            }

            foreach (ServerInfo serverInfo in serverList.Values)
            {
                if ((internalServer == null || (serverInfo.internalIP == internalServer.ip && serverInfo.internalPort == internalServer.port))
                && (externalServer == null || (serverInfo.externalIP == externalServer.ip && serverInfo.externalPort == externalServer.port)))
                {
                    return serverInfo;
                }
            }

            return null;
        }

        public static void RegistServer(IAppSession session, ushort serverID, ServerType serverType)
        {
            ServerInfo serverInfo;
            if (serverList.TryGetValue(serverID, out serverInfo))
            {
                if (serverInfo.serverType != serverType)
                {
                    Log.Error("ServerInfo", $"서버 데이터와 등록을 요청한 서버의 타입이 다릅니다. [데이터:{serverInfo.serverType}/요청:{serverType}]");
                    return;
                }
                serverInfo.session = session;
                sessionToServerInfo.Add(session, serverInfo);
            }
            else
            {
                Log.Error("ServerInfo", $"서버 리스트에 존재하지 않는 서버 등록을 시도했습니다. [ServerID:{serverID}]");
            }
        }

        public static ServerInfo UnregistServer(IAppSession session)
        {
            ServerInfo result = null;
            foreach (ServerInfo serverInfo in serverList.Values)
            {
                if (serverInfo.session.SessionID == session.SessionID)
                {
                    serverInfo.session = null;
                    result = serverInfo;
                }
            }

            sessionToServerInfo.Remove(session);
            return result;
        }

        public static ServerInfo FindServerType(ServerType serverType)
        {
            foreach (ServerInfo serverInfo in serverList.Values)
                if (serverInfo.serverType == serverType)
                    return serverInfo;
            return null;
        }

        public static IEnumerable<ServerInfo> FindAllServerType(ServerType serverType)
        {
            foreach (ServerInfo serverInfo in serverList.Values)
                if (serverInfo.serverType == serverType)
                    yield return serverInfo;
        }

        public static bool IsInternalServer(string ip)
        {
            foreach (ServerInfo serverInfo in serverList.Values)
                if (serverInfo.internalIP == ip)
                    return true;
            return false;
        }

        public static ServerInfo GetServerInfo(ushort serverID)
        {
            ServerInfo result = null;
            serverList.TryGetValue(serverID, out result);
            return result;
        }

        public static int ServerTypeCount(ServerType serverType)
        {
            int result = 0;
            foreach (ServerInfo serverInfo in serverList.Values)
                if (serverInfo.serverType == serverType)
                    result++;
            return result;
        }

        public static ServerInfo SessionToInfo(IAppSession session)
        {
            ServerInfo result = null;
            sessionToServerInfo.TryGetValue(session, out result);
            return result;
        }
    }

    public static partial class ExPacket
    {
        public static void SendPacket<T>(this T packet, ServerInfo serverInfo) where T : Packet
        {
            if (serverInfo.session.State != SessionState.Connected)
                return;
            DataPackage packetPackage = new DataPackage();
            packetPackage.Key = PacketController.GetIndex(typeof(T));
            packetPackage.Body = MessagePackSerializer.Serialize(packet);
            packet.SendPacket(serverInfo.session);
        }

    }
}