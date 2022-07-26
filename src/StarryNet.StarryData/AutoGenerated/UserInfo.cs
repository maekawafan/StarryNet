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
    public partial class UserInfoData : StarryData
    {
        public readonly ulong auid;
        public readonly string userName;
        public readonly byte team;
        public readonly ushort killCount;
        public readonly ushort ping;

        public override void Regist()
        {
            DataController.userInfoDataIDTable.Add(id, this);
            DataController.userInfoDataNameTable.Add(name, this);
            DataController.userInfoDataList.Add(this);
        }

        public static UserInfoData Get(uint id)
        {
            if (GetDataCount == 0)
            {
                Log.Error("UserInfo", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.userInfoDataIDTable.TryGetValue(id, out var data))
                return data;
            return null;
        }

        public static UserInfoData Get(string name)
        {
            if (GetDataCount == 0)
            {
                Log.Error("UserInfo", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.userInfoDataNameTable.TryGetValue(name, out var data))
                return data;
            return null;
        }

        public static UserInfoData GetRandom()
        {
            if (GetDataCount == 0)
            {
                Log.Error("UserInfo", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            uint dataCount = (uint)GetDataCount;

            if (DataController.userInfoDataIDTable.TryGetValue(Random.NextUint(1u, dataCount), out var data))
                return data;
            return null;
        }

        public static IDictionary GetDictionary()
        {
            return DataController.userInfoDataIDTable;
        }

        public static IEnumerable<UserInfoData> GetAllDatas()
        {
            foreach (var data in DataController.userInfoDataIDTable.Values)
                yield return data;
        }

        public static List<UserInfoData> GetAllDataList()
        {
            return DataController.userInfoDataList;
        }

        public static Type GetNameReferenceType()
        {
            return typeof(UserInfoDataName);
        }

        public static Type GetIDReferenceType()
        {
            return typeof(UserInfoDataID);
        }

        public static void UnloadAll()
        {
            DataController.userInfoDataIDTable.Clear();
            DataController.userInfoDataNameTable.Clear();
            DataController.userInfoDataList.Clear();
        }

        public static UserInfoData GetInstanceValue(IStarryDataReference<UserInfoData> data)
        {
            if (data == null)
                return null;
            return data.Get();
        }

        public static List<UserInfoData> GetInstanceValue(IStarryDataReference<UserInfoData>[] data)
        {
            List<UserInfoData> result = new List<UserInfoData>();
            if (data == null)
                return result;

            foreach (var value in data)
            {
                result.Add(value.Get());
            }
            return result;
        }

        public static int GetDataCount => DataController.userInfoDataIDTable.Count;
    }

    public partial class UserInfoDataID : StarryDataID<UserInfoData>
    {
        public override UserInfoData Get()
        {
            if (DataController.userInfoDataIDTable.TryGetValue(id, out var result))
                return result;
            return null;
        }

        public UserInfoDataID() : base() { }
        public UserInfoDataID(uint id) : base(id) { }

        public static implicit operator long(UserInfoDataID dataId) => (long)dataId.id;
    }

    public partial class UserInfoDataName : StarryDataName<UserInfoData>
    {
        public override UserInfoData Get()
        {
            if (DataController.userInfoDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public UserInfoDataName() : base() { }
        public UserInfoDataName(string name) : base(name) { }
    }

    public partial class UserInfoInstance : StarryInstance<UserInfoData>
    {
        public ulong auid;
        public string userName;
        public byte team;
        public ushort killCount;
        public ushort ping;

        public UserInfoInstance(UserInfoData data)
        {
            this.data = data;
            auid = data.auid;
            userName = data.userName;
            team = data.team;
            killCount = data.killCount;
            ping = data.ping;
        }

        static UserInfoInstance()
        {

        }

        public UserInfoInstance(uint id) : this(UserInfoData.Get(id)) { }
        public UserInfoInstance(string name) : this(UserInfoData.Get(name)) { }
    }

    public class UserInfoInstanceComparer : IEqualityComparer<UserInfoInstance>
    {
        public bool Equals(UserInfoInstance x, UserInfoInstance y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(UserInfoInstance obj)
        {
            return unchecked((int)obj.id);
        }
    }

    public static partial class DataController
    {
        public static List<UserInfoData> userInfoDataList = new List<UserInfoData>();
        public static Dictionary<uint, UserInfoData> userInfoDataIDTable = new Dictionary<uint, UserInfoData>();
        public static Dictionary<string, UserInfoData> userInfoDataNameTable = new Dictionary<string, UserInfoData>();
    }

    public sealed class UserInfoDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<UserInfoData>
    {
        public void Serialize(ref MessagePackWriter writer, UserInfoData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public UserInfoData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            return UserInfoData.Get(__id__);
        }
    }

    public sealed class UserInfoInstanceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<UserInfoInstance>
    {
        public void Serialize(ref MessagePackWriter writer, UserInfoInstance value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(6);
            writer.WriteUInt32(value.id);
            writer.Write(value.auid);
            writer.Write(value.userName);
            writer.Write(value.team);
            writer.Write(value.killCount);
            writer.Write(value.ping);
        }

        public UserInfoInstance Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = reader.ReadArrayHeader();
            var __id__ = default(uint);

            if (length == 0)
                return null;

            ulong __auid__ = default(ulong);
            string __userName__ = default(string);
            byte __team__ = default(byte);
            ushort __killCount__ = default(ushort);
            ushort __ping__ = default(ushort);

            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __id__ = reader.ReadUInt32();
                        break;
                    case 1:
                        reader.Read(out __auid__);
                        break;
                    case 2:
                        reader.Read(out __userName__);
                        break;
                    case 3:
                        reader.Read(out __team__);
                        break;
                    case 4:
                        reader.Read(out __killCount__);
                        break;
                    case 5:
                        reader.Read(out __ping__);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            UserInfoInstance result = new UserInfoInstance(__id__);
            result.auid = __auid__;
            result.userName = __userName__;
            result.team = __team__;
            result.killCount = __killCount__;
            result.ping = __ping__;

            return result;
        }
    }

    public static partial class UserInfoEx
    {
        public static void Write(this ref MessagePackWriter writer, UserInfoData value)
        {
            writer.Write(value.id);
        }

        public static void Write(this ref MessagePackWriter writer, List<UserInfoData> value)
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

        public static void Write(this ref MessagePackWriter writer, IStarryDataReference<UserInfoData> value)
        {
            writer.Write(value.Get().id);
        }

        public static void Read(this ref MessagePackReader reader, out UserInfoData value)
        {
            value = UserInfoData.Get(reader.ReadUInt16());
        }

        public static void Read(this ref MessagePackReader reader, out List<UserInfoData> value)
        {
            int length = reader.ReadInt32();
            value = new List<UserInfoData>();
            for (int i = 0; i < length; i++)
            {
                UserInfoData atom;
                reader.Read(out atom);
                value.Add(atom);
            }
        }

        public static void Read(this ref MessagePackReader reader, out IStarryDataReference<UserInfoData> value)
        {
            value = new UserInfoDataID(reader.ReadUInt16());
        }
    }
}