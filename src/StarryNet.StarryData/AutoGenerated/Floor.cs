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
    public partial class FloorData : StarryData
    {
        public readonly string displayName;
        public readonly string sprite;
        public readonly string footSound;

        public override void Regist()
        {
            DataController.floorDataIDTable.Add(id, this);
            DataController.floorDataNameTable.Add(name, this);
            DataController.floorDataList.Add(this);
        }

        public static FloorData Get(uint id)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Floor", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.floorDataIDTable.TryGetValue(id, out var data))
                return data;
            return null;
        }

        public static FloorData Get(string name)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Floor", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.floorDataNameTable.TryGetValue(name, out var data))
                return data;
            return null;
        }

        public static FloorData GetRandom()
        {
            if (GetDataCount == 0)
            {
                Log.Error("Floor", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            uint dataCount = (uint)GetDataCount;

            if (DataController.floorDataIDTable.TryGetValue(Random.NextUint(1u, dataCount), out var data))
                return data;
            return null;
        }

        public static IDictionary GetDictionary()
        {
            return DataController.floorDataIDTable;
        }

        public static IEnumerable<FloorData> GetAllDatas()
        {
            foreach (var data in DataController.floorDataIDTable.Values)
                yield return data;
        }

        public static List<FloorData> GetAllDataList()
        {
            return DataController.floorDataList;
        }

        public static Type GetNameReferenceType()
        {
            return typeof(FloorDataName);
        }

        public static Type GetIDReferenceType()
        {
            return typeof(FloorDataID);
        }

        public static void UnloadAll()
        {
            DataController.floorDataIDTable.Clear();
            DataController.floorDataNameTable.Clear();
            DataController.floorDataList.Clear();
        }

        public static FloorData GetInstanceValue(IStarryDataReference<FloorData> data)
        {
            if (data == null)
                return null;
            return data.Get();
        }

        public static List<FloorData> GetInstanceValue(IStarryDataReference<FloorData>[] data)
        {
            List<FloorData> result = new List<FloorData>();
            if (data == null)
                return result;

            foreach (var value in data)
            {
                result.Add(value.Get());
            }
            return result;
        }

        public static int GetDataCount => DataController.floorDataIDTable.Count;
    }

    public partial class FloorDataID : StarryDataID<FloorData>
    {
        public override FloorData Get()
        {
            if (DataController.floorDataIDTable.TryGetValue(id, out var result))
                return result;
            return null;
        }

        public FloorDataID() : base() { }
        public FloorDataID(uint id) : base(id) { }

        public static implicit operator long(FloorDataID dataId) => (long)dataId.id;
    }

    public partial class FloorDataName : StarryDataName<FloorData>
    {
        public override FloorData Get()
        {
            if (DataController.floorDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public FloorDataName() : base() { }
        public FloorDataName(string name) : base(name) { }
    }

    public partial class FloorInstance : StarryInstance<FloorData>
    {
        public string displayName { get { return data.displayName; } }
        public string sprite { get { return data.sprite; } }
        public string footSound { get { return data.footSound; } }

        public FloorInstance(FloorData data)
        {
            this.data = data;
        }

        static FloorInstance()
        {

        }

        public FloorInstance(uint id) : this(FloorData.Get(id)) { }
        public FloorInstance(string name) : this(FloorData.Get(name)) { }
    }

    public class FloorInstanceComparer : IEqualityComparer<FloorInstance>
    {
        public bool Equals(FloorInstance x, FloorInstance y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(FloorInstance obj)
        {
            return unchecked((int)obj.id);
        }
    }

    public static partial class DataController
    {
        public static List<FloorData> floorDataList = new List<FloorData>();
        public static Dictionary<uint, FloorData> floorDataIDTable = new Dictionary<uint, FloorData>();
        public static Dictionary<string, FloorData> floorDataNameTable = new Dictionary<string, FloorData>();
    }

    public sealed class FloorDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<FloorData>
    {
        public void Serialize(ref MessagePackWriter writer, FloorData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public FloorData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            return FloorData.Get(__id__);
        }
    }

    public sealed class FloorInstanceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<FloorInstance>
    {
        public void Serialize(ref MessagePackWriter writer, FloorInstance value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public FloorInstance Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            FloorInstance result = new FloorInstance(__id__);

            return result;
        }
    }

    public static partial class FloorEx
    {
        public static void Write(this ref MessagePackWriter writer, FloorData value)
        {
            writer.Write(value.id);
        }

        public static void Write(this ref MessagePackWriter writer, List<FloorData> value)
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

        public static void Write(this ref MessagePackWriter writer, IStarryDataReference<FloorData> value)
        {
            writer.Write(value.Get().id);
        }

        public static void Read(this ref MessagePackReader reader, out FloorData value)
        {
            value = FloorData.Get(reader.ReadUInt16());
        }

        public static void Read(this ref MessagePackReader reader, out List<FloorData> value)
        {
            int length = reader.ReadInt32();
            value = new List<FloorData>();
            for (int i = 0; i < length; i++)
            {
                FloorData atom;
                reader.Read(out atom);
                value.Add(atom);
            }
        }

        public static void Read(this ref MessagePackReader reader, out IStarryDataReference<FloorData> value)
        {
            value = new FloorDataID(reader.ReadUInt16());
        }
    }
}