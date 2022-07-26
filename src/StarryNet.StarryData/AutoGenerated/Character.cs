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
    public partial class CharacterData : StarryData
    {
        public enum Division
        {
            holoMyth,
            ProjectHOPE,
            holoCouncil,
            hololive0th,
        };

        public readonly string displayName;
        public readonly string subName;
        public readonly Division division;
        public readonly IStarryDataReference<SkillData>[] skill;

        public override void Regist()
        {
            DataController.characterDataIDTable.Add(id, this);
            DataController.characterDataNameTable.Add(name, this);
            DataController.characterDataList.Add(this);
        }

        public static CharacterData Get(uint id)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Character", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.characterDataIDTable.TryGetValue(id, out var data))
                return data;
            return null;
        }

        public static CharacterData Get(string name)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Character", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.characterDataNameTable.TryGetValue(name, out var data))
                return data;
            return null;
        }

        public static CharacterData GetRandom()
        {
            if (GetDataCount == 0)
            {
                Log.Error("Character", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            uint dataCount = (uint)GetDataCount;

            if (DataController.characterDataIDTable.TryGetValue(Random.NextUint(1u, dataCount), out var data))
                return data;
            return null;
        }

        public static IDictionary GetDictionary()
        {
            return DataController.characterDataIDTable;
        }

        public static IEnumerable<CharacterData> GetAllDatas()
        {
            foreach (var data in DataController.characterDataIDTable.Values)
                yield return data;
        }

        public static List<CharacterData> GetAllDataList()
        {
            return DataController.characterDataList;
        }

        public static Type GetNameReferenceType()
        {
            return typeof(CharacterDataName);
        }

        public static Type GetIDReferenceType()
        {
            return typeof(CharacterDataID);
        }

        public static void UnloadAll()
        {
            DataController.characterDataIDTable.Clear();
            DataController.characterDataNameTable.Clear();
            DataController.characterDataList.Clear();
        }

        public static CharacterData GetInstanceValue(IStarryDataReference<CharacterData> data)
        {
            if (data == null)
                return null;
            return data.Get();
        }

        public static List<CharacterData> GetInstanceValue(IStarryDataReference<CharacterData>[] data)
        {
            List<CharacterData> result = new List<CharacterData>();
            if (data == null)
                return result;

            foreach (var value in data)
            {
                result.Add(value.Get());
            }
            return result;
        }

        public static int GetDataCount => DataController.characterDataIDTable.Count;
    }

    public partial class CharacterDataID : StarryDataID<CharacterData>
    {
        public override CharacterData Get()
        {
            if (DataController.characterDataIDTable.TryGetValue(id, out var result))
                return result;
            return null;
        }

        public CharacterDataID() : base() { }
        public CharacterDataID(uint id) : base(id) { }

        public static implicit operator long(CharacterDataID dataId) => (long)dataId.id;
    }

    public partial class CharacterDataName : StarryDataName<CharacterData>
    {
        public override CharacterData Get()
        {
            if (DataController.characterDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public CharacterDataName() : base() { }
        public CharacterDataName(string name) : base(name) { }
    }

    public partial class CharacterInstance : StarryInstance<CharacterData>
    {
        public string displayName { get { return data.displayName; } }
        public string subName { get { return data.subName; } }
        public CharacterData.Division division { get { return data.division; } }
        public IStarryDataReference<SkillData>[] skill { get { return data.skill; } }

        public CharacterInstance(CharacterData data)
        {
            this.data = data;
        }

        static CharacterInstance()
        {

        }

        public CharacterInstance(uint id) : this(CharacterData.Get(id)) { }
        public CharacterInstance(string name) : this(CharacterData.Get(name)) { }
    }

    public class CharacterInstanceComparer : IEqualityComparer<CharacterInstance>
    {
        public bool Equals(CharacterInstance x, CharacterInstance y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(CharacterInstance obj)
        {
            return unchecked((int)obj.id);
        }
    }

    public static partial class DataController
    {
        public static List<CharacterData> characterDataList = new List<CharacterData>();
        public static Dictionary<uint, CharacterData> characterDataIDTable = new Dictionary<uint, CharacterData>();
        public static Dictionary<string, CharacterData> characterDataNameTable = new Dictionary<string, CharacterData>();
    }

    public sealed class CharacterDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<CharacterData>
    {
        public void Serialize(ref MessagePackWriter writer, CharacterData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public CharacterData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            return CharacterData.Get(__id__);
        }
    }

    public sealed class CharacterInstanceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<CharacterInstance>
    {
        public void Serialize(ref MessagePackWriter writer, CharacterInstance value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public CharacterInstance Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            CharacterInstance result = new CharacterInstance(__id__);

            return result;
        }
    }

    public static partial class CharacterEx
    {
        public static void Write(this ref MessagePackWriter writer, CharacterData value)
        {
            writer.Write(value.id);
        }

        public static void Write(this ref MessagePackWriter writer, List<CharacterData> value)
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

        public static void Write(this ref MessagePackWriter writer, IStarryDataReference<CharacterData> value)
        {
            writer.Write(value.Get().id);
        }

        public static void Read(this ref MessagePackReader reader, out CharacterData value)
        {
            value = CharacterData.Get(reader.ReadUInt16());
        }

        public static void Read(this ref MessagePackReader reader, out List<CharacterData> value)
        {
            int length = reader.ReadInt32();
            value = new List<CharacterData>();
            for (int i = 0; i < length; i++)
            {
                CharacterData atom;
                reader.Read(out atom);
                value.Add(atom);
            }
        }

        public static void Read(this ref MessagePackReader reader, out IStarryDataReference<CharacterData> value)
        {
            value = new CharacterDataID(reader.ReadUInt16());
        }
    }
}