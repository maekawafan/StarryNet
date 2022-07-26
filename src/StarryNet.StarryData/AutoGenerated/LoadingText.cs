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
    public partial class LoadingTextData : StarryData
    {
        public readonly string reviveText;
        public readonly string sprite;

        public override void Regist()
        {
            DataController.loadingTextDataIDTable.Add(id, this);
            DataController.loadingTextDataNameTable.Add(name, this);
            DataController.loadingTextDataList.Add(this);
        }

        public static LoadingTextData Get(uint id)
        {
            if (GetDataCount == 0)
            {
                Log.Error("LoadingText", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.loadingTextDataIDTable.TryGetValue(id, out var data))
                return data;
            return null;
        }

        public static LoadingTextData Get(string name)
        {
            if (GetDataCount == 0)
            {
                Log.Error("LoadingText", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.loadingTextDataNameTable.TryGetValue(name, out var data))
                return data;
            return null;
        }

        public static LoadingTextData GetRandom()
        {
            if (GetDataCount == 0)
            {
                Log.Error("LoadingText", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            uint dataCount = (uint)GetDataCount;

            if (DataController.loadingTextDataIDTable.TryGetValue(Random.NextUint(1u, dataCount), out var data))
                return data;
            return null;
        }

        public static IDictionary GetDictionary()
        {
            return DataController.loadingTextDataIDTable;
        }

        public static IEnumerable<LoadingTextData> GetAllDatas()
        {
            foreach (var data in DataController.loadingTextDataIDTable.Values)
                yield return data;
        }

        public static List<LoadingTextData> GetAllDataList()
        {
            return DataController.loadingTextDataList;
        }

        public static Type GetNameReferenceType()
        {
            return typeof(LoadingTextDataName);
        }

        public static Type GetIDReferenceType()
        {
            return typeof(LoadingTextDataID);
        }

        public static void UnloadAll()
        {
            DataController.loadingTextDataIDTable.Clear();
            DataController.loadingTextDataNameTable.Clear();
            DataController.loadingTextDataList.Clear();
        }

        public static LoadingTextData GetInstanceValue(IStarryDataReference<LoadingTextData> data)
        {
            if (data == null)
                return null;
            return data.Get();
        }

        public static List<LoadingTextData> GetInstanceValue(IStarryDataReference<LoadingTextData>[] data)
        {
            List<LoadingTextData> result = new List<LoadingTextData>();
            if (data == null)
                return result;

            foreach (var value in data)
            {
                result.Add(value.Get());
            }
            return result;
        }

        public static int GetDataCount => DataController.loadingTextDataIDTable.Count;
    }

    public partial class LoadingTextDataID : StarryDataID<LoadingTextData>
    {
        public override LoadingTextData Get()
        {
            if (DataController.loadingTextDataIDTable.TryGetValue(id, out var result))
                return result;
            return null;
        }

        public LoadingTextDataID() : base() { }
        public LoadingTextDataID(uint id) : base(id) { }

        public static implicit operator long(LoadingTextDataID dataId) => (long)dataId.id;
    }

    public partial class LoadingTextDataName : StarryDataName<LoadingTextData>
    {
        public override LoadingTextData Get()
        {
            if (DataController.loadingTextDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public LoadingTextDataName() : base() { }
        public LoadingTextDataName(string name) : base(name) { }
    }

    public partial class LoadingTextInstance : StarryInstance<LoadingTextData>
    {
        public string reviveText { get { return data.reviveText; } }
        public string sprite { get { return data.sprite; } }

        public LoadingTextInstance(LoadingTextData data)
        {
            this.data = data;
        }

        static LoadingTextInstance()
        {

        }

        public LoadingTextInstance(uint id) : this(LoadingTextData.Get(id)) { }
        public LoadingTextInstance(string name) : this(LoadingTextData.Get(name)) { }
    }

    public class LoadingTextInstanceComparer : IEqualityComparer<LoadingTextInstance>
    {
        public bool Equals(LoadingTextInstance x, LoadingTextInstance y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(LoadingTextInstance obj)
        {
            return unchecked((int)obj.id);
        }
    }

    public static partial class DataController
    {
        public static List<LoadingTextData> loadingTextDataList = new List<LoadingTextData>();
        public static Dictionary<uint, LoadingTextData> loadingTextDataIDTable = new Dictionary<uint, LoadingTextData>();
        public static Dictionary<string, LoadingTextData> loadingTextDataNameTable = new Dictionary<string, LoadingTextData>();
    }

    public sealed class LoadingTextDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<LoadingTextData>
    {
        public void Serialize(ref MessagePackWriter writer, LoadingTextData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public LoadingTextData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            return LoadingTextData.Get(__id__);
        }
    }

    public sealed class LoadingTextInstanceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<LoadingTextInstance>
    {
        public void Serialize(ref MessagePackWriter writer, LoadingTextInstance value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public LoadingTextInstance Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            LoadingTextInstance result = new LoadingTextInstance(__id__);

            return result;
        }
    }

    public static partial class LoadingTextEx
    {
        public static void Write(this ref MessagePackWriter writer, LoadingTextData value)
        {
            writer.Write(value.id);
        }

        public static void Write(this ref MessagePackWriter writer, List<LoadingTextData> value)
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

        public static void Write(this ref MessagePackWriter writer, IStarryDataReference<LoadingTextData> value)
        {
            writer.Write(value.Get().id);
        }

        public static void Read(this ref MessagePackReader reader, out LoadingTextData value)
        {
            value = LoadingTextData.Get(reader.ReadUInt16());
        }

        public static void Read(this ref MessagePackReader reader, out List<LoadingTextData> value)
        {
            int length = reader.ReadInt32();
            value = new List<LoadingTextData>();
            for (int i = 0; i < length; i++)
            {
                LoadingTextData atom;
                reader.Read(out atom);
                value.Add(atom);
            }
        }

        public static void Read(this ref MessagePackReader reader, out IStarryDataReference<LoadingTextData> value)
        {
            value = new LoadingTextDataID(reader.ReadUInt16());
        }
    }
}