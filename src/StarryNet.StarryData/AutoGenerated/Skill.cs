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
    public partial class SkillData : StarryData
    {
        public enum Slot
        {
            Active1,
            Active2,
            Active3,
            Active4,
            CharacterPassive,
            Passive1,
        };

        public readonly Slot slot;
        public readonly string displayName;
        public readonly IStarryDataReference<CharacterData>[] user;
        public readonly float cooltime;
        public readonly float range;
        public readonly float stackCooltime;
        public readonly uint stack;
        public readonly string markerSize;
        public readonly bool wallThrough;
        public readonly string iconfile;
        public readonly string description;
        public readonly string comment;
        public readonly float[] param;

        public override void Regist()
        {
            DataController.skillDataIDTable.Add(id, this);
            DataController.skillDataNameTable.Add(name, this);
            DataController.skillDataList.Add(this);
        }

        public static SkillData Get(uint id)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Skill", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.skillDataIDTable.TryGetValue(id, out var data))
                return data;
            return null;
        }

        public static SkillData Get(string name)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Skill", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.skillDataNameTable.TryGetValue(name, out var data))
                return data;
            return null;
        }

        public static SkillData GetRandom()
        {
            if (GetDataCount == 0)
            {
                Log.Error("Skill", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            uint dataCount = (uint)GetDataCount;

            if (DataController.skillDataIDTable.TryGetValue(Random.NextUint(1u, dataCount), out var data))
                return data;
            return null;
        }

        public static IDictionary GetDictionary()
        {
            return DataController.skillDataIDTable;
        }

        public static IEnumerable<SkillData> GetAllDatas()
        {
            foreach (var data in DataController.skillDataIDTable.Values)
                yield return data;
        }

        public static List<SkillData> GetAllDataList()
        {
            return DataController.skillDataList;
        }

        public static Type GetNameReferenceType()
        {
            return typeof(SkillDataName);
        }

        public static Type GetIDReferenceType()
        {
            return typeof(SkillDataID);
        }

        public static void UnloadAll()
        {
            DataController.skillDataIDTable.Clear();
            DataController.skillDataNameTable.Clear();
            DataController.skillDataList.Clear();
        }

        public static SkillData GetInstanceValue(IStarryDataReference<SkillData> data)
        {
            if (data == null)
                return null;
            return data.Get();
        }

        public static List<SkillData> GetInstanceValue(IStarryDataReference<SkillData>[] data)
        {
            List<SkillData> result = new List<SkillData>();
            if (data == null)
                return result;

            foreach (var value in data)
            {
                result.Add(value.Get());
            }
            return result;
        }

        public static int GetDataCount => DataController.skillDataIDTable.Count;
    }

    public partial class SkillDataID : StarryDataID<SkillData>
    {
        public override SkillData Get()
        {
            if (DataController.skillDataIDTable.TryGetValue(id, out var result))
                return result;
            return null;
        }

        public SkillDataID() : base() { }
        public SkillDataID(uint id) : base(id) { }

        public static implicit operator long(SkillDataID dataId) => (long)dataId.id;
    }

    public partial class SkillDataName : StarryDataName<SkillData>
    {
        public override SkillData Get()
        {
            if (DataController.skillDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public SkillDataName() : base() { }
        public SkillDataName(string name) : base(name) { }
    }

    public partial class SkillInstance : StarryInstance<SkillData>
    {
        public SkillData.Slot slot { get { return data.slot; } }
        public string displayName { get { return data.displayName; } }
        public IStarryDataReference<CharacterData>[] user { get { return data.user; } }
        public float cooltime { get { return data.cooltime; } }
        public float range { get { return data.range; } }
        public float stackCooltime { get { return data.stackCooltime; } }
        public uint stack { get { return data.stack; } }
        public string markerSize { get { return data.markerSize; } }
        public bool wallThrough { get { return data.wallThrough; } }
        public string iconfile { get { return data.iconfile; } }
        public string description { get { return data.description; } }
        public string comment { get { return data.comment; } }
        public float[] param { get { return data.param; } }

        public SkillInstance(SkillData data)
        {
            this.data = data;
        }

        static SkillInstance()
        {

        }

        public SkillInstance(uint id) : this(SkillData.Get(id)) { }
        public SkillInstance(string name) : this(SkillData.Get(name)) { }
    }

    public class SkillInstanceComparer : IEqualityComparer<SkillInstance>
    {
        public bool Equals(SkillInstance x, SkillInstance y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(SkillInstance obj)
        {
            return unchecked((int)obj.id);
        }
    }

    public static partial class DataController
    {
        public static List<SkillData> skillDataList = new List<SkillData>();
        public static Dictionary<uint, SkillData> skillDataIDTable = new Dictionary<uint, SkillData>();
        public static Dictionary<string, SkillData> skillDataNameTable = new Dictionary<string, SkillData>();
    }

    public sealed class SkillDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<SkillData>
    {
        public void Serialize(ref MessagePackWriter writer, SkillData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public SkillData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            return SkillData.Get(__id__);
        }
    }

    public sealed class SkillInstanceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<SkillInstance>
    {
        public void Serialize(ref MessagePackWriter writer, SkillInstance value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public SkillInstance Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            SkillInstance result = new SkillInstance(__id__);

            return result;
        }
    }

    public static partial class SkillEx
    {
        public static void Write(this ref MessagePackWriter writer, SkillData value)
        {
            writer.Write(value.id);
        }

        public static void Write(this ref MessagePackWriter writer, List<SkillData> value)
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

        public static void Write(this ref MessagePackWriter writer, IStarryDataReference<SkillData> value)
        {
            writer.Write(value.Get().id);
        }

        public static void Read(this ref MessagePackReader reader, out SkillData value)
        {
            value = SkillData.Get(reader.ReadUInt16());
        }

        public static void Read(this ref MessagePackReader reader, out List<SkillData> value)
        {
            int length = reader.ReadInt32();
            value = new List<SkillData>();
            for (int i = 0; i < length; i++)
            {
                SkillData atom;
                reader.Read(out atom);
                value.Add(atom);
            }
        }

        public static void Read(this ref MessagePackReader reader, out IStarryDataReference<SkillData> value)
        {
            value = new SkillDataID(reader.ReadUInt16());
        }
    }
}