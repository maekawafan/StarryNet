using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;
using StarryNet.StarryLibrary;

using BigInteger = System.Numerics.BigInteger;
using Random = StarryNet.StarryLibrary.Random;

namespace StarryNet.StarryData
{
    public partial class ServerInfoData : StarryData
    {
        public readonly ulong suid;
        public readonly string serverName;
        public readonly string ip;
        public readonly ushort port;
        public readonly ushort currentUser;
        public readonly ushort maxUser;

        public override void Regist()
        {
            DataController.serverInfoDataIDTable.Add(id, this);
            DataController.serverInfoDataNameTable.Add(name, this);
            DataController.serverInfoDataList.Add(this);
        }

        public static ServerInfoData Get(uint id)
        {
            if (GetDataCount == 0)
            {
                Log.Error("ServerInfo", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.serverInfoDataIDTable.TryGetValue(id, out var data))
                return data;
            return null;
        }

        public static ServerInfoData Get(string name)
        {
            if (GetDataCount == 0)
            {
                Log.Error("ServerInfo", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.serverInfoDataNameTable.TryGetValue(name, out var data))
                return data;
            return null;
        }

        public static ServerInfoData GetRandom()
        {
            if (GetDataCount == 0)
            {
                Log.Error("ServerInfo", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            uint dataCount = (uint)GetDataCount;

            if (DataController.serverInfoDataIDTable.TryGetValue(Random.NextUint(1u, dataCount), out var data))
                return data;
            return null;
        }

        public static IDictionary GetDictionary()
        {
            return DataController.serverInfoDataIDTable;
        }

        public static IEnumerable<ServerInfoData> GetAllDatas()
        {
            foreach (var data in DataController.serverInfoDataIDTable.Values)
                yield return data;
        }

        public static List<ServerInfoData> GetAllDataList()
        {
            return DataController.serverInfoDataList;
        }

        public static Type GetNameReferenceType()
        {
            return typeof(ServerInfoDataName);
        }

        public static Type GetIDReferenceType()
        {
            return typeof(ServerInfoDataID);
        }

        public static void UnloadAll()
        {
            DataController.serverInfoDataIDTable.Clear();
            DataController.serverInfoDataNameTable.Clear();
            DataController.serverInfoDataList.Clear();
        }

        public static ServerInfoData GetInstanceValue(IStarryDataReference<ServerInfoData> data)
        {
            if (data == null)
                return null;
            return data.Get();
        }

        public static List<ServerInfoData> GetInstanceValue(IStarryDataReference<ServerInfoData>[] data)
        {
            List<ServerInfoData> result = new List<ServerInfoData>();
            if (data == null)
                return result;

            foreach (var value in data)
            {
                result.Add(value.Get());
            }
            return result;
        }

        public static int GetDataCount => DataController.serverInfoDataIDTable.Count;
    }

    public partial class ServerInfoDataID : StarryDataID<ServerInfoData>
    {
        public override ServerInfoData Get()
        {
            if (DataController.serverInfoDataIDTable.TryGetValue(id, out var result))
                return result;
            return null;
        }

        public ServerInfoDataID() : base() { }
        public ServerInfoDataID(uint id) : base(id) { }

        public static implicit operator long(ServerInfoDataID dataId) => (long)dataId.id;
    }

    public partial class ServerInfoDataName : StarryDataName<ServerInfoData>
    {
        public override ServerInfoData Get()
        {
            if (DataController.serverInfoDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public ServerInfoDataName() : base() { }
        public ServerInfoDataName(string name) : base(name) { }
    }

    public partial class ServerInfoInstance : StarryInstance<ServerInfoData>
    {
        public ulong suid;
        public string serverName;
        public string ip;
        public ushort port;
        public ushort currentUser;
        public ushort maxUser;

        public ServerInfoInstance(ServerInfoData data)
        {
            this.data = data;
            suid = data.suid;
            serverName = data.serverName;
            ip = data.ip;
            port = data.port;
            currentUser = data.currentUser;
            maxUser = data.maxUser;
        }

        static ServerInfoInstance()
        {

        }

        public ServerInfoInstance(uint id) : this(ServerInfoData.Get(id)) { }
        public ServerInfoInstance(string name) : this(ServerInfoData.Get(name)) { }
    }

    public class ServerInfoInstanceComparer : IEqualityComparer<ServerInfoInstance>
    {
        public bool Equals(ServerInfoInstance x, ServerInfoInstance y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(ServerInfoInstance obj)
        {
            return unchecked((int)obj.id);
        }
    }

    public static partial class DataController
    {
        public static List<ServerInfoData> serverInfoDataList = new List<ServerInfoData>();
        public static Dictionary<uint, ServerInfoData> serverInfoDataIDTable = new Dictionary<uint, ServerInfoData>();
        public static Dictionary<string, ServerInfoData> serverInfoDataNameTable = new Dictionary<string, ServerInfoData>();
    }

    public sealed class ServerInfoDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<ServerInfoData>
    {
        public void Serialize(ref MessagePackWriter writer, ServerInfoData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public ServerInfoData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = reader.ReadArrayHeader();
            var __id__ = default(uint);

            if (length == 0)
                return null;

            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __id__ = reader.ReadUInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return ServerInfoData.Get(__id__);
        }
    }

    public sealed class ServerInfoInstanceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<ServerInfoInstance>
    {
        public void Serialize(ref MessagePackWriter writer, ServerInfoInstance value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(7);
            writer.WriteUInt32(value.id);
            writer.Write(value.suid);
            writer.Write(value.serverName);
            writer.Write(value.ip);
            writer.Write(value.port);
            writer.Write(value.currentUser);
            writer.Write(value.maxUser);
        }

        public ServerInfoInstance Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = reader.ReadArrayHeader();
            var __id__ = default(uint);

            if (length == 0)
                return null;

            ulong __suid__ = default(ulong);
            string __serverName__ = default(string);
            string __ip__ = default(string);
            ushort __port__ = default(ushort);
            ushort __currentUser__ = default(ushort);
            ushort __maxUser__ = default(ushort);

            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __id__ = reader.ReadUInt32();
                        break;
                    case 1:
                        reader.Read(out __suid__);
                        break;
                    case 2:
                        reader.Read(out __serverName__);
                        break;
                    case 3:
                        reader.Read(out __ip__);
                        break;
                    case 4:
                        reader.Read(out __port__);
                        break;
                    case 5:
                        reader.Read(out __currentUser__);
                        break;
                    case 6:
                        reader.Read(out __maxUser__);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            ServerInfoInstance result = new ServerInfoInstance(__id__);
            result.suid = __suid__;
            result.serverName = __serverName__;
            result.ip = __ip__;
            result.port = __port__;
            result.currentUser = __currentUser__;
            result.maxUser = __maxUser__;

            return result;
        }
    }

    public static partial class ServerInfoEx
    {
        public static void Write(this ref MessagePackWriter writer, ServerInfoData value)
        {
            writer.Write(value.id);
        }

        public static void Write(this ref MessagePackWriter writer, List<ServerInfoData> value)
        {
            if (value == null)
            {
                writer.Write(0);
                return;
            }

            writer.Write(value.Count);
            foreach(var atom in value)
                writer.Write(atom);
        }

        public static void Write(this ref MessagePackWriter writer, IStarryDataReference<ServerInfoData> value)
        {
            writer.Write(value.Get().id);
        }

        public static void Read(this ref MessagePackReader reader, out ServerInfoData value)
        {
            value = ServerInfoData.Get(reader.ReadUInt16());
        }

        public static void Read(this ref MessagePackReader reader, out List<ServerInfoData> value)
        {
            int length = reader.ReadInt32();
            value = new List<ServerInfoData>();
            for (int i = 0; i < length; i++)
            {
                ServerInfoData atom;
                reader.Read(out atom);
                value.Add(atom);
            }
        }

        public static void Read(this ref MessagePackReader reader, out IStarryDataReference<ServerInfoData> value)
        {
            value = new ServerInfoDataID(reader.ReadUInt16());
        }
    }
}