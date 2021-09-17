using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

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
    }
    
    public partial class ItemDataName : StarryDataName<ItemData>
    {
        public override ItemData Get()
        {
            if (DataController.itemDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }
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

        public ItemInstance(uint id) : this(ItemData.Get(id)) { }
        public ItemInstance(string name) : this(ItemData.Get(name)) { }
    }
    
    public static partial class DataController
    {
        public static Dictionary<uint, ItemData> itemDataIDTable = new Dictionary<uint, ItemData>();
        public static Dictionary<string, ItemData> itemDataNameTable = new Dictionary<string, ItemData>();
    }

    public sealed class ItemDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<ItemData>
    {
        public void Serialize(ref MessagePackWriter writer, ItemData value, global::MessagePack.MessagePackSerializerOptions options)
        {
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
}