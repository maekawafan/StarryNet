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
    public partial class BulletData : StarryData
    {
        public readonly string displayName;
        public readonly float damage;
        public readonly float wallBreak;
        public readonly float speed;
        public readonly string icon;
        public readonly string prefab;
        public readonly string shellPrefab;
        public readonly string fireSound;
        public readonly string shellSound;
        public readonly float lifeTime;

        public override void Regist()
        {
            DataController.bulletDataIDTable.Add(id, this);
            DataController.bulletDataNameTable.Add(name, this);
            DataController.bulletDataList.Add(this);
        }

        public static BulletData Get(uint id)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Bullet", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.bulletDataIDTable.TryGetValue(id, out var data))
                return data;
            return null;
        }

        public static BulletData Get(string name)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Bullet", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.bulletDataNameTable.TryGetValue(name, out var data))
                return data;
            return null;
        }

        public static BulletData GetRandom()
        {
            if (GetDataCount == 0)
            {
                Log.Error("Bullet", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            uint dataCount = (uint)GetDataCount;

            if (DataController.bulletDataIDTable.TryGetValue(Random.NextUint(1u, dataCount), out var data))
                return data;
            return null;
        }

        public static IDictionary GetDictionary()
        {
            return DataController.bulletDataIDTable;
        }

        public static IEnumerable<BulletData> GetAllDatas()
        {
            foreach (var data in DataController.bulletDataIDTable.Values)
                yield return data;
        }

        public static List<BulletData> GetAllDataList()
        {
            return DataController.bulletDataList;
        }

        public static Type GetNameReferenceType()
        {
            return typeof(BulletDataName);
        }

        public static Type GetIDReferenceType()
        {
            return typeof(BulletDataID);
        }

        public static void UnloadAll()
        {
            DataController.bulletDataIDTable.Clear();
            DataController.bulletDataNameTable.Clear();
            DataController.bulletDataList.Clear();
        }

        public static BulletData GetInstanceValue(IStarryDataReference<BulletData> data)
        {
            if (data == null)
                return null;
            return data.Get();
        }

        public static List<BulletData> GetInstanceValue(IStarryDataReference<BulletData>[] data)
        {
            List<BulletData> result = new List<BulletData>();
            if (data == null)
                return result;

            foreach (var value in data)
            {
                result.Add(value.Get());
            }
            return result;
        }

        public static int GetDataCount => DataController.bulletDataIDTable.Count;
    }

    public partial class BulletDataID : StarryDataID<BulletData>
    {
        public override BulletData Get()
        {
            if (DataController.bulletDataIDTable.TryGetValue(id, out var result))
                return result;
            return null;
        }

        public BulletDataID() : base() { }
        public BulletDataID(uint id) : base(id) { }

        public static implicit operator long(BulletDataID dataId) => (long)dataId.id;
    }

    public partial class BulletDataName : StarryDataName<BulletData>
    {
        public override BulletData Get()
        {
            if (DataController.bulletDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public BulletDataName() : base() { }
        public BulletDataName(string name) : base(name) { }
    }

    public partial class BulletInstance : StarryInstance<BulletData>
    {
        public string displayName { get { return data.displayName; } }
        public float damage { get { return data.damage; } }
        public float wallBreak { get { return data.wallBreak; } }
        public float speed { get { return data.speed; } }
        public string icon { get { return data.icon; } }
        public string prefab { get { return data.prefab; } }
        public string shellPrefab { get { return data.shellPrefab; } }
        public string fireSound { get { return data.fireSound; } }
        public string shellSound { get { return data.shellSound; } }
        public float lifeTime { get { return data.lifeTime; } }

        public BulletInstance(BulletData data)
        {
            this.data = data;
        }

        static BulletInstance()
        {

        }

        public BulletInstance(uint id) : this(BulletData.Get(id)) { }
        public BulletInstance(string name) : this(BulletData.Get(name)) { }
    }

    public class BulletInstanceComparer : IEqualityComparer<BulletInstance>
    {
        public bool Equals(BulletInstance x, BulletInstance y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(BulletInstance obj)
        {
            return unchecked((int)obj.id);
        }
    }

    public static partial class DataController
    {
        public static List<BulletData> bulletDataList = new List<BulletData>();
        public static Dictionary<uint, BulletData> bulletDataIDTable = new Dictionary<uint, BulletData>();
        public static Dictionary<string, BulletData> bulletDataNameTable = new Dictionary<string, BulletData>();
    }

    public sealed class BulletDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<BulletData>
    {
        public void Serialize(ref MessagePackWriter writer, BulletData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public BulletData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            return BulletData.Get(__id__);
        }
    }

    public sealed class BulletInstanceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<BulletInstance>
    {
        public void Serialize(ref MessagePackWriter writer, BulletInstance value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public BulletInstance Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            BulletInstance result = new BulletInstance(__id__);

            return result;
        }
    }

    public static partial class BulletEx
    {
        public static void Write(this ref MessagePackWriter writer, BulletData value)
        {
            writer.Write(value.id);
        }

        public static void Write(this ref MessagePackWriter writer, List<BulletData> value)
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

        public static void Write(this ref MessagePackWriter writer, IStarryDataReference<BulletData> value)
        {
            writer.Write(value.Get().id);
        }

        public static void Read(this ref MessagePackReader reader, out BulletData value)
        {
            value = BulletData.Get(reader.ReadUInt16());
        }

        public static void Read(this ref MessagePackReader reader, out List<BulletData> value)
        {
            int length = reader.ReadInt32();
            value = new List<BulletData>();
            for (int i = 0; i < length; i++)
            {
                BulletData atom;
                reader.Read(out atom);
                value.Add(atom);
            }
        }

        public static void Read(this ref MessagePackReader reader, out IStarryDataReference<BulletData> value)
        {
            value = new BulletDataID(reader.ReadUInt16());
        }
    }
}