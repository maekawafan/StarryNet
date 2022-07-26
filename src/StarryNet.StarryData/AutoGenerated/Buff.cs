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
    public partial class BuffData : StarryData
    {
        public readonly string displayName;
        public readonly string iconfile;
        public readonly Color iconBackgroundColor;
        public readonly bool broadcast;
        public readonly float value;
        public readonly float startTime;
        public readonly float endTime;

        public override void Regist()
        {
            DataController.buffDataIDTable.Add(id, this);
            DataController.buffDataNameTable.Add(name, this);
            DataController.buffDataList.Add(this);
        }

        public static BuffData Get(uint id)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Buff", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.buffDataIDTable.TryGetValue(id, out var data))
                return data;
            return null;
        }

        public static BuffData Get(string name)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Buff", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.buffDataNameTable.TryGetValue(name, out var data))
                return data;
            return null;
        }

        public static BuffData GetRandom()
        {
            if (GetDataCount == 0)
            {
                Log.Error("Buff", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            uint dataCount = (uint)GetDataCount;

            if (DataController.buffDataIDTable.TryGetValue(Random.NextUint(1u, dataCount), out var data))
                return data;
            return null;
        }

        public static IDictionary GetDictionary()
        {
            return DataController.buffDataIDTable;
        }

        public static IEnumerable<BuffData> GetAllDatas()
        {
            foreach (var data in DataController.buffDataIDTable.Values)
                yield return data;
        }

        public static List<BuffData> GetAllDataList()
        {
            return DataController.buffDataList;
        }

        public static Type GetNameReferenceType()
        {
            return typeof(BuffDataName);
        }

        public static Type GetIDReferenceType()
        {
            return typeof(BuffDataID);
        }

        public static void UnloadAll()
        {
            DataController.buffDataIDTable.Clear();
            DataController.buffDataNameTable.Clear();
            DataController.buffDataList.Clear();
        }

        public static BuffData GetInstanceValue(IStarryDataReference<BuffData> data)
        {
            if (data == null)
                return null;
            return data.Get();
        }

        public static List<BuffData> GetInstanceValue(IStarryDataReference<BuffData>[] data)
        {
            List<BuffData> result = new List<BuffData>();
            if (data == null)
                return result;

            foreach (var value in data)
            {
                result.Add(value.Get());
            }
            return result;
        }

        public static int GetDataCount => DataController.buffDataIDTable.Count;
    }

    public partial class BuffDataID : StarryDataID<BuffData>
    {
        public override BuffData Get()
        {
            if (DataController.buffDataIDTable.TryGetValue(id, out var result))
                return result;
            return null;
        }

        public BuffDataID() : base() { }
        public BuffDataID(uint id) : base(id) { }

        public static implicit operator long(BuffDataID dataId) => (long)dataId.id;
    }

    public partial class BuffDataName : StarryDataName<BuffData>
    {
        public override BuffData Get()
        {
            if (DataController.buffDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public BuffDataName() : base() { }
        public BuffDataName(string name) : base(name) { }
    }

    public partial class BuffInstance : StarryInstance<BuffData>
    {
        public string displayName { get { return data.displayName; } }
        public string iconfile { get { return data.iconfile; } }
        public Color iconBackgroundColor { get { return data.iconBackgroundColor; } }
        public bool broadcast { get { return data.broadcast; } }
        public float value;
        public float startTime;
        public float endTime;

        public BuffInstance(BuffData data)
        {
            this.data = data;
            value = data.value;
            startTime = data.startTime;
            endTime = data.endTime;
        }

        static BuffInstance()
        {

        }

        public BuffInstance(uint id) : this(BuffData.Get(id)) { }
        public BuffInstance(string name) : this(BuffData.Get(name)) { }
    }

    public class BuffInstanceComparer : IEqualityComparer<BuffInstance>
    {
        public bool Equals(BuffInstance x, BuffInstance y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(BuffInstance obj)
        {
            return unchecked((int)obj.id);
        }
    }

    public static partial class DataController
    {
        public static List<BuffData> buffDataList = new List<BuffData>();
        public static Dictionary<uint, BuffData> buffDataIDTable = new Dictionary<uint, BuffData>();
        public static Dictionary<string, BuffData> buffDataNameTable = new Dictionary<string, BuffData>();
    }

    public sealed class BuffDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<BuffData>
    {
        public void Serialize(ref MessagePackWriter writer, BuffData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public BuffData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            return BuffData.Get(__id__);
        }
    }

    public sealed class BuffInstanceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<BuffInstance>
    {
        public void Serialize(ref MessagePackWriter writer, BuffInstance value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(4);
            writer.WriteUInt32(value.id);
            writer.Write(value.value);
            writer.Write(value.startTime);
            writer.Write(value.endTime);
        }

        public BuffInstance Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = reader.ReadArrayHeader();
            var __id__ = default(uint);

            if (length == 0)
                return null;

            float __value__ = default(float);
            float __startTime__ = default(float);
            float __endTime__ = default(float);

            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __id__ = reader.ReadUInt32();
                        break;
                    case 1:
                        reader.Read(out __value__);
                        break;
                    case 2:
                        reader.Read(out __startTime__);
                        break;
                    case 3:
                        reader.Read(out __endTime__);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            BuffInstance result = new BuffInstance(__id__);
            result.value = __value__;
            result.startTime = __startTime__;
            result.endTime = __endTime__;

            return result;
        }
    }

    public static partial class BuffEx
    {
        public static void Write(this ref MessagePackWriter writer, BuffData value)
        {
            writer.Write(value.id);
        }

        public static void Write(this ref MessagePackWriter writer, List<BuffData> value)
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

        public static void Write(this ref MessagePackWriter writer, IStarryDataReference<BuffData> value)
        {
            writer.Write(value.Get().id);
        }

        public static void Read(this ref MessagePackReader reader, out BuffData value)
        {
            value = BuffData.Get(reader.ReadUInt16());
        }

        public static void Read(this ref MessagePackReader reader, out List<BuffData> value)
        {
            int length = reader.ReadInt32();
            value = new List<BuffData>();
            for (int i = 0; i < length; i++)
            {
                BuffData atom;
                reader.Read(out atom);
                value.Add(atom);
            }
        }

        public static void Read(this ref MessagePackReader reader, out IStarryDataReference<BuffData> value)
        {
            value = new BuffDataID(reader.ReadUInt16());
        }
    }
}