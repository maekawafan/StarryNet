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
    public partial class MapData : StarryData
    {
        public readonly string displayName;
        public readonly string filename;

        public override void Regist()
        {
            DataController.mapDataIDTable.Add(id, this);
            DataController.mapDataNameTable.Add(name, this);
            DataController.mapDataList.Add(this);
        }

        public static MapData Get(uint id)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Map", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.mapDataIDTable.TryGetValue(id, out var data))
                return data;
            return null;
        }

        public static MapData Get(string name)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Map", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.mapDataNameTable.TryGetValue(name, out var data))
                return data;
            return null;
        }

        public static MapData GetRandom()
        {
            if (GetDataCount == 0)
            {
                Log.Error("Map", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            uint dataCount = (uint)GetDataCount;

            if (DataController.mapDataIDTable.TryGetValue(Random.NextUint(1u, dataCount), out var data))
                return data;
            return null;
        }

        public static IDictionary GetDictionary()
        {
            return DataController.mapDataIDTable;
        }

        public static IEnumerable<MapData> GetAllDatas()
        {
            foreach (var data in DataController.mapDataIDTable.Values)
                yield return data;
        }

        public static List<MapData> GetAllDataList()
        {
            return DataController.mapDataList;
        }

        public static Type GetNameReferenceType()
        {
            return typeof(MapDataName);
        }

        public static Type GetIDReferenceType()
        {
            return typeof(MapDataID);
        }

        public static void UnloadAll()
        {
            DataController.mapDataIDTable.Clear();
            DataController.mapDataNameTable.Clear();
            DataController.mapDataList.Clear();
        }

        public static MapData GetInstanceValue(IStarryDataReference<MapData> data)
        {
            if (data == null)
                return null;
            return data.Get();
        }

        public static List<MapData> GetInstanceValue(IStarryDataReference<MapData>[] data)
        {
            List<MapData> result = new List<MapData>();
            if (data == null)
                return result;

            foreach (var value in data)
            {
                result.Add(value.Get());
            }
            return result;
        }

        public static int GetDataCount => DataController.mapDataIDTable.Count;
    }

    public partial class MapDataID : StarryDataID<MapData>
    {
        public override MapData Get()
        {
            if (DataController.mapDataIDTable.TryGetValue(id, out var result))
                return result;
            return null;
        }

        public MapDataID() : base() { }
        public MapDataID(uint id) : base(id) { }

        public static implicit operator long(MapDataID dataId) => (long)dataId.id;
    }

    public partial class MapDataName : StarryDataName<MapData>
    {
        public override MapData Get()
        {
            if (DataController.mapDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public MapDataName() : base() { }
        public MapDataName(string name) : base(name) { }
    }

    public partial class MapInstance : StarryInstance<MapData>
    {
        public string displayName { get { return data.displayName; } }
        public string filename { get { return data.filename; } }

        public MapInstance(MapData data)
        {
            this.data = data;
        }

        static MapInstance()
        {

        }

        public MapInstance(uint id) : this(MapData.Get(id)) { }
        public MapInstance(string name) : this(MapData.Get(name)) { }
    }

    public class MapInstanceComparer : IEqualityComparer<MapInstance>
    {
        public bool Equals(MapInstance x, MapInstance y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(MapInstance obj)
        {
            return unchecked((int)obj.id);
        }
    }

    public static partial class DataController
    {
        public static List<MapData> mapDataList = new List<MapData>();
        public static Dictionary<uint, MapData> mapDataIDTable = new Dictionary<uint, MapData>();
        public static Dictionary<string, MapData> mapDataNameTable = new Dictionary<string, MapData>();
    }

    public sealed class MapDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<MapData>
    {
        public void Serialize(ref MessagePackWriter writer, MapData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public MapData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            return MapData.Get(__id__);
        }
    }

    public sealed class MapInstanceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<MapInstance>
    {
        public void Serialize(ref MessagePackWriter writer, MapInstance value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public MapInstance Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            MapInstance result = new MapInstance(__id__);

            return result;
        }
    }

    public static partial class MapEx
    {
        public static void Write(this ref MessagePackWriter writer, MapData value)
        {
            writer.Write(value.id);
        }

        public static void Write(this ref MessagePackWriter writer, List<MapData> value)
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

        public static void Write(this ref MessagePackWriter writer, IStarryDataReference<MapData> value)
        {
            writer.Write(value.Get().id);
        }

        public static void Read(this ref MessagePackReader reader, out MapData value)
        {
            value = MapData.Get(reader.ReadUInt16());
        }

        public static void Read(this ref MessagePackReader reader, out List<MapData> value)
        {
            int length = reader.ReadInt32();
            value = new List<MapData>();
            for (int i = 0; i < length; i++)
            {
                MapData atom;
                reader.Read(out atom);
                value.Add(atom);
            }
        }

        public static void Read(this ref MessagePackReader reader, out IStarryDataReference<MapData> value)
        {
            value = new MapDataID(reader.ReadUInt16());
        }
    }
}