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
    public partial class ReviveTextData : StarryData
    {
        public readonly string reviveText;
        public readonly string sprite;
        public readonly IStarryDataReference<CharacterData> usingCharacter;

        public override void Regist()
        {
            DataController.reviveTextDataIDTable.Add(id, this);
            DataController.reviveTextDataNameTable.Add(name, this);
            DataController.reviveTextDataList.Add(this);
        }

        public static ReviveTextData Get(uint id)
        {
            if (GetDataCount == 0)
            {
                Log.Error("ReviveText", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.reviveTextDataIDTable.TryGetValue(id, out var data))
                return data;
            return null;
        }

        public static ReviveTextData Get(string name)
        {
            if (GetDataCount == 0)
            {
                Log.Error("ReviveText", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.reviveTextDataNameTable.TryGetValue(name, out var data))
                return data;
            return null;
        }

        public static ReviveTextData GetRandom()
        {
            if (GetDataCount == 0)
            {
                Log.Error("ReviveText", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            uint dataCount = (uint)GetDataCount;

            if (DataController.reviveTextDataIDTable.TryGetValue(Random.NextUint(1u, dataCount), out var data))
                return data;
            return null;
        }

        public static IDictionary GetDictionary()
        {
            return DataController.reviveTextDataIDTable;
        }

        public static IEnumerable<ReviveTextData> GetAllDatas()
        {
            foreach (var data in DataController.reviveTextDataIDTable.Values)
                yield return data;
        }

        public static List<ReviveTextData> GetAllDataList()
        {
            return DataController.reviveTextDataList;
        }

        public static Type GetNameReferenceType()
        {
            return typeof(ReviveTextDataName);
        }

        public static Type GetIDReferenceType()
        {
            return typeof(ReviveTextDataID);
        }

        public static void UnloadAll()
        {
            DataController.reviveTextDataIDTable.Clear();
            DataController.reviveTextDataNameTable.Clear();
            DataController.reviveTextDataList.Clear();
        }

        public static ReviveTextData GetInstanceValue(IStarryDataReference<ReviveTextData> data)
        {
            if (data == null)
                return null;
            return data.Get();
        }

        public static List<ReviveTextData> GetInstanceValue(IStarryDataReference<ReviveTextData>[] data)
        {
            List<ReviveTextData> result = new List<ReviveTextData>();
            if (data == null)
                return result;

            foreach (var value in data)
            {
                result.Add(value.Get());
            }
            return result;
        }

        public static int GetDataCount => DataController.reviveTextDataIDTable.Count;
    }

    public partial class ReviveTextDataID : StarryDataID<ReviveTextData>
    {
        public override ReviveTextData Get()
        {
            if (DataController.reviveTextDataIDTable.TryGetValue(id, out var result))
                return result;
            return null;
        }

        public ReviveTextDataID() : base() { }
        public ReviveTextDataID(uint id) : base(id) { }

        public static implicit operator long(ReviveTextDataID dataId) => (long)dataId.id;
    }

    public partial class ReviveTextDataName : StarryDataName<ReviveTextData>
    {
        public override ReviveTextData Get()
        {
            if (DataController.reviveTextDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public ReviveTextDataName() : base() { }
        public ReviveTextDataName(string name) : base(name) { }
    }

    public partial class ReviveTextInstance : StarryInstance<ReviveTextData>
    {
        public string reviveText { get { return data.reviveText; } }
        public string sprite { get { return data.sprite; } }
        public IStarryDataReference<CharacterData> usingCharacter { get { return data.usingCharacter; } }

        public ReviveTextInstance(ReviveTextData data)
        {
            this.data = data;
        }

        static ReviveTextInstance()
        {

        }

        public ReviveTextInstance(uint id) : this(ReviveTextData.Get(id)) { }
        public ReviveTextInstance(string name) : this(ReviveTextData.Get(name)) { }
    }

    public class ReviveTextInstanceComparer : IEqualityComparer<ReviveTextInstance>
    {
        public bool Equals(ReviveTextInstance x, ReviveTextInstance y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(ReviveTextInstance obj)
        {
            return unchecked((int)obj.id);
        }
    }

    public static partial class DataController
    {
        public static List<ReviveTextData> reviveTextDataList = new List<ReviveTextData>();
        public static Dictionary<uint, ReviveTextData> reviveTextDataIDTable = new Dictionary<uint, ReviveTextData>();
        public static Dictionary<string, ReviveTextData> reviveTextDataNameTable = new Dictionary<string, ReviveTextData>();
    }

    public sealed class ReviveTextDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<ReviveTextData>
    {
        public void Serialize(ref MessagePackWriter writer, ReviveTextData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public ReviveTextData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            return ReviveTextData.Get(__id__);
        }
    }

    public sealed class ReviveTextInstanceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<ReviveTextInstance>
    {
        public void Serialize(ref MessagePackWriter writer, ReviveTextInstance value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public ReviveTextInstance Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            ReviveTextInstance result = new ReviveTextInstance(__id__);

            return result;
        }
    }

    public static partial class ReviveTextEx
    {
        public static void Write(this ref MessagePackWriter writer, ReviveTextData value)
        {
            writer.Write(value.id);
        }

        public static void Write(this ref MessagePackWriter writer, List<ReviveTextData> value)
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

        public static void Write(this ref MessagePackWriter writer, IStarryDataReference<ReviveTextData> value)
        {
            writer.Write(value.Get().id);
        }

        public static void Read(this ref MessagePackReader reader, out ReviveTextData value)
        {
            value = ReviveTextData.Get(reader.ReadUInt16());
        }

        public static void Read(this ref MessagePackReader reader, out List<ReviveTextData> value)
        {
            int length = reader.ReadInt32();
            value = new List<ReviveTextData>();
            for (int i = 0; i < length; i++)
            {
                ReviveTextData atom;
                reader.Read(out atom);
                value.Add(atom);
            }
        }

        public static void Read(this ref MessagePackReader reader, out IStarryDataReference<ReviveTextData> value)
        {
            value = new ReviveTextDataID(reader.ReadUInt16());
        }
    }
}