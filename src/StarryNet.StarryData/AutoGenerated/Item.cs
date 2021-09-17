using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;
using StarryNet.StarryLibrary;

using BigInteger = System.Numerics.BigInteger;

namespace StarryNet.StarryData
{
    public class ItemData : StarryData
    {
        public readonly string description;
        public readonly int upgrade;
        public readonly int[] test;
        public readonly Color mainColor;

        public override void Regist()
        {
            DataController.itemDataIDTable.Add(id, this);
            DataController.itemDataNameTable.Add(name, this);
            DataController.itemDataList.Add(this);
        }

        public static ItemData Get(uint id)
        {
            if (DataController.itemDataIDTable.TryGetValue(id, out var data))
                return data;
            return null;
        }

        public static ItemData Get(string name)
        {
            if (DataController.itemDataNameTable.TryGetValue(name, out var data))
                return data;
            return null;
        }

        public static IDictionary GetDictionary()
        {
            return DataController.itemDataIDTable;
        }

        public static IEnumerable<ItemData> GetAllDatas()
        {
            foreach (var data in DataController.itemDataIDTable.Values)
                yield return data;
        }

        public static List<ItemData> GetAllDataList()
        {
            return DataController.itemDataList;
        }

        public static Type GetNameReferenceType()
        {
            return typeof(ItemDataName);
        }

        public static Type GetIDReferenceType()
        {
            return typeof(ItemDataID);
        }

        public static void UnloadAll()
        {
            DataController.itemDataIDTable.Clear();
            DataController.itemDataNameTable.Clear();
            DataController.itemDataList.Clear();
        }

        public static ItemData GetInstanceValue(IStarryDataReference<ItemData> data)
        {
            if (data == null)
                return null;
            return data.Get();
        }

        public static List<ItemData> GetInstanceValue(IStarryDataReference<ItemData>[] data)
        {
            List<ItemData> result = new List<ItemData>();
            if (data == null)
                return result;

            foreach (var value in data)
            {
                result.Add(value.Get());
            }
            return result;
        }
    }

    public partial class ItemDataID : StarryDataID<ItemData>
    {
        public override ItemData Get()
        {
            if (DataController.itemDataIDTable.TryGetValue(id, out var result))
                return result;
            return null;
        }

        public ItemDataID() : base() { }
        public ItemDataID(uint id) : base(id) { }

        public static implicit operator long(ItemDataID dataId) => (long)dataId.id;
    }

    public partial class ItemDataName : StarryDataName<ItemData>
    {
        public override ItemData Get()
        {
            if (DataController.itemDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public ItemDataName() : base() { }
        public ItemDataName(string name) : base(name) { }
    }

    public partial class ItemInstance : StarryInstance<ItemData>
    {
        public string description { get { return data.description; } }
        public int upgrade;
        public int[] test { get { return data.test; } }
        public Color mainColor { get { return data.mainColor; } }

        public ItemInstance(ItemData data)
        {
            this.data = data;
            upgrade = data.upgrade;
        }

        static ItemInstance()
        {

        }

        public ItemInstance(uint id) : this(ItemData.Get(id)) { }
        public ItemInstance(string name) : this(ItemData.Get(name)) { }
    }

    public class ItemInstanceComparer : IEqualityComparer<ItemInstance>
    {
        public bool Equals(ItemInstance x, ItemInstance y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(ItemInstance obj)
        {
            return unchecked((int)obj.id);
        }
    }

    public static partial class DataController
    {
        public static List<ItemData> itemDataList = new List<ItemData>();
        public static Dictionary<uint, ItemData> itemDataIDTable = new Dictionary<uint, ItemData>();
        public static Dictionary<string, ItemData> itemDataNameTable = new Dictionary<string, ItemData>();
    }

    public sealed class ItemDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<ItemData>
    {
        public void Serialize(ref MessagePackWriter writer, ItemData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public ItemData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            return ItemData.Get(__id__);
        }
    }

    public sealed class ItemInstanceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<ItemInstance>
    {
        public void Serialize(ref MessagePackWriter writer, ItemInstance value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(2);
            writer.WriteUInt32(value.id);
            writer.Write(value.upgrade);
        }

        public ItemInstance Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = reader.ReadArrayHeader();
            var __id__ = default(uint);

            if (length == 0)
                return null;

            int __upgrade__ = default(int);

            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __id__ = reader.ReadUInt32();
                        break;
                    case 1:
                        reader.Read(out __upgrade__);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            ItemInstance result = new ItemInstance(__id__);
            result.upgrade = __upgrade__;

            return result;
        }
    }

    public static partial class ItemEx
    {
        public static void Write(this ref MessagePackWriter writer, ItemData value)
        {
            writer.Write(value.id);
        }

        public static void Write(this ref MessagePackWriter writer, List<ItemData> value)
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

        public static void Write(this ref MessagePackWriter writer, IStarryDataReference<ItemData> value)
        {
            writer.Write(value.Get().id);
        }

        public static void Read(this ref MessagePackReader reader, out ItemData value)
        {
            value = ItemData.Get(reader.ReadUInt16());
        }

        public static void Read(this ref MessagePackReader reader, out List<ItemData> value)
        {
            int length = reader.ReadInt32();
            value = new List<ItemData>();
            for (int i = 0; i < length; i++)
            {
                ItemData atom;
                reader.Read(out atom);
                value.Add(atom);
            }
        }

        public static void Read(this ref MessagePackReader reader, out IStarryDataReference<ItemData> value)
        {
            value = new ItemDataID(reader.ReadUInt16());
        }
    }
}