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
    public partial class WallData : StarryData
    {
        public readonly string displayName;
        public readonly ushort index;
        public readonly int durability;
        public readonly float strength;
        public readonly string sprite;
        public readonly float buildCompleteTime;
        public readonly ulong builder;

        public override void Regist()
        {
            DataController.wallDataIDTable.Add(id, this);
            DataController.wallDataNameTable.Add(name, this);
            DataController.wallDataList.Add(this);
        }

        public static WallData Get(uint id)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Wall", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.wallDataIDTable.TryGetValue(id, out var data))
                return data;
            return null;
        }

        public static WallData Get(string name)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Wall", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.wallDataNameTable.TryGetValue(name, out var data))
                return data;
            return null;
        }

        public static WallData GetRandom()
        {
            if (GetDataCount == 0)
            {
                Log.Error("Wall", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            uint dataCount = (uint)GetDataCount;

            if (DataController.wallDataIDTable.TryGetValue(Random.NextUint(1u, dataCount), out var data))
                return data;
            return null;
        }

        public static IDictionary GetDictionary()
        {
            return DataController.wallDataIDTable;
        }

        public static IEnumerable<WallData> GetAllDatas()
        {
            foreach (var data in DataController.wallDataIDTable.Values)
                yield return data;
        }

        public static List<WallData> GetAllDataList()
        {
            return DataController.wallDataList;
        }

        public static Type GetNameReferenceType()
        {
            return typeof(WallDataName);
        }

        public static Type GetIDReferenceType()
        {
            return typeof(WallDataID);
        }

        public static void UnloadAll()
        {
            DataController.wallDataIDTable.Clear();
            DataController.wallDataNameTable.Clear();
            DataController.wallDataList.Clear();
        }

        public static WallData GetInstanceValue(IStarryDataReference<WallData> data)
        {
            if (data == null)
                return null;
            return data.Get();
        }

        public static List<WallData> GetInstanceValue(IStarryDataReference<WallData>[] data)
        {
            List<WallData> result = new List<WallData>();
            if (data == null)
                return result;

            foreach (var value in data)
            {
                result.Add(value.Get());
            }
            return result;
        }

        public static int GetDataCount => DataController.wallDataIDTable.Count;
    }

    public partial class WallDataID : StarryDataID<WallData>
    {
        public override WallData Get()
        {
            if (DataController.wallDataIDTable.TryGetValue(id, out var result))
                return result;
            return null;
        }

        public WallDataID() : base() { }
        public WallDataID(uint id) : base(id) { }

        public static implicit operator long(WallDataID dataId) => (long)dataId.id;
    }

    public partial class WallDataName : StarryDataName<WallData>
    {
        public override WallData Get()
        {
            if (DataController.wallDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public WallDataName() : base() { }
        public WallDataName(string name) : base(name) { }
    }

    public partial class WallInstance : StarryInstance<WallData>
    {
        public string displayName { get { return data.displayName; } }
        public ushort index;
        public int durability;
        public float strength { get { return data.strength; } }
        public string sprite { get { return data.sprite; } }
        public float buildCompleteTime;
        public ulong builder;

        public WallInstance(WallData data)
        {
            this.data = data;
            index = data.index;
            durability = data.durability;
            buildCompleteTime = data.buildCompleteTime;
            builder = data.builder;
        }

        static WallInstance()
        {

        }

        public WallInstance(uint id) : this(WallData.Get(id)) { }
        public WallInstance(string name) : this(WallData.Get(name)) { }
    }

    public class WallInstanceComparer : IEqualityComparer<WallInstance>
    {
        public bool Equals(WallInstance x, WallInstance y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(WallInstance obj)
        {
            return unchecked((int)obj.id);
        }
    }

    public static partial class DataController
    {
        public static List<WallData> wallDataList = new List<WallData>();
        public static Dictionary<uint, WallData> wallDataIDTable = new Dictionary<uint, WallData>();
        public static Dictionary<string, WallData> wallDataNameTable = new Dictionary<string, WallData>();
    }

    public sealed class WallDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<WallData>
    {
        public void Serialize(ref MessagePackWriter writer, WallData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public WallData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            return WallData.Get(__id__);
        }
    }

    public sealed class WallInstanceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<WallInstance>
    {
        public void Serialize(ref MessagePackWriter writer, WallInstance value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(5);
            writer.WriteUInt32(value.id);
            writer.Write(value.index);
            writer.Write(value.durability);
            writer.Write(value.buildCompleteTime);
            writer.Write(value.builder);
        }

        public WallInstance Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = reader.ReadArrayHeader();
            var __id__ = default(uint);

            if (length == 0)
                return null;

            ushort __index__ = default(ushort);
            int __durability__ = default(int);
            float __buildCompleteTime__ = default(float);
            ulong __builder__ = default(ulong);

            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __id__ = reader.ReadUInt32();
                        break;
                    case 1:
                        reader.Read(out __index__);
                        break;
                    case 2:
                        reader.Read(out __durability__);
                        break;
                    case 3:
                        reader.Read(out __buildCompleteTime__);
                        break;
                    case 4:
                        reader.Read(out __builder__);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            WallInstance result = new WallInstance(__id__);
            result.index = __index__;
            result.durability = __durability__;
            result.buildCompleteTime = __buildCompleteTime__;
            result.builder = __builder__;

            return result;
        }
    }

    public static partial class WallEx
    {
        public static void Write(this ref MessagePackWriter writer, WallData value)
        {
            writer.Write(value.id);
        }

        public static void Write(this ref MessagePackWriter writer, List<WallData> value)
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

        public static void Write(this ref MessagePackWriter writer, IStarryDataReference<WallData> value)
        {
            writer.Write(value.Get().id);
        }

        public static void Read(this ref MessagePackReader reader, out WallData value)
        {
            value = WallData.Get(reader.ReadUInt16());
        }

        public static void Read(this ref MessagePackReader reader, out List<WallData> value)
        {
            int length = reader.ReadInt32();
            value = new List<WallData>();
            for (int i = 0; i < length; i++)
            {
                WallData atom;
                reader.Read(out atom);
                value.Add(atom);
            }
        }

        public static void Read(this ref MessagePackReader reader, out IStarryDataReference<WallData> value)
        {
            value = new WallDataID(reader.ReadUInt16());
        }
    }
}