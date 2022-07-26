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
    public partial class WeaponData : StarryData
    {
        public readonly string displayName;
        public readonly IStarryDataReference<WeaponData> baseWeapon;
        public readonly IStarryDataReference<BulletData> bulletType;
        public readonly float speed;
        public readonly uint chamber;
        public readonly uint magazine;
        public readonly uint surplus;
        public readonly byte warhead;
        public readonly float delay;
        public readonly float spread;
        public readonly float maxSpread;
        public readonly float spreadDecrese;
        public readonly float changeDelay;
        public readonly float reload;
        public readonly bool automatic;
        public readonly IStarryDataReference<BulletData>[] bullet;
        public readonly string animationName;
        public readonly string icon;
        public readonly string sprite;
        public readonly string magazineSprite;
        public readonly string killmark;
        public readonly string aimSprite;

        public override void Regist()
        {
            DataController.weaponDataIDTable.Add(id, this);
            DataController.weaponDataNameTable.Add(name, this);
            DataController.weaponDataList.Add(this);
        }

        public static WeaponData Get(uint id)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Weapon", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.weaponDataIDTable.TryGetValue(id, out var data))
                return data;
            return null;
        }

        public static WeaponData Get(string name)
        {
            if (GetDataCount == 0)
            {
                Log.Error("Weapon", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            if (DataController.weaponDataNameTable.TryGetValue(name, out var data))
                return data;
            return null;
        }

        public static WeaponData GetRandom()
        {
            if (GetDataCount == 0)
            {
                Log.Error("Weapon", "데이터가 로드된 적 없는데 사용하려 합니다.");
                return null;
            }

            uint dataCount = (uint)GetDataCount;

            if (DataController.weaponDataIDTable.TryGetValue(Random.NextUint(1u, dataCount), out var data))
                return data;
            return null;
        }

        public static IDictionary GetDictionary()
        {
            return DataController.weaponDataIDTable;
        }

        public static IEnumerable<WeaponData> GetAllDatas()
        {
            foreach (var data in DataController.weaponDataIDTable.Values)
                yield return data;
        }

        public static List<WeaponData> GetAllDataList()
        {
            return DataController.weaponDataList;
        }

        public static Type GetNameReferenceType()
        {
            return typeof(WeaponDataName);
        }

        public static Type GetIDReferenceType()
        {
            return typeof(WeaponDataID);
        }

        public static void UnloadAll()
        {
            DataController.weaponDataIDTable.Clear();
            DataController.weaponDataNameTable.Clear();
            DataController.weaponDataList.Clear();
        }

        public static WeaponData GetInstanceValue(IStarryDataReference<WeaponData> data)
        {
            if (data == null)
                return null;
            return data.Get();
        }

        public static List<WeaponData> GetInstanceValue(IStarryDataReference<WeaponData>[] data)
        {
            List<WeaponData> result = new List<WeaponData>();
            if (data == null)
                return result;

            foreach (var value in data)
            {
                result.Add(value.Get());
            }
            return result;
        }

        public static int GetDataCount => DataController.weaponDataIDTable.Count;
    }

    public partial class WeaponDataID : StarryDataID<WeaponData>
    {
        public override WeaponData Get()
        {
            if (DataController.weaponDataIDTable.TryGetValue(id, out var result))
                return result;
            return null;
        }

        public WeaponDataID() : base() { }
        public WeaponDataID(uint id) : base(id) { }

        public static implicit operator long(WeaponDataID dataId) => (long)dataId.id;
    }

    public partial class WeaponDataName : StarryDataName<WeaponData>
    {
        public override WeaponData Get()
        {
            if (DataController.weaponDataNameTable.TryGetValue(name, out var result))
                return result;
            return null;
        }

        public WeaponDataName() : base() { }
        public WeaponDataName(string name) : base(name) { }
    }

    public partial class WeaponInstance : StarryInstance<WeaponData>
    {
        public string displayName { get { return data.displayName; } }
        public IStarryDataReference<WeaponData> baseWeapon { get { return data.baseWeapon; } }
        public IStarryDataReference<BulletData> bulletType { get { return data.bulletType; } }
        public float speed { get { return data.speed; } }
        public uint chamber { get { return data.chamber; } }
        public uint magazine { get { return data.magazine; } }
        public uint surplus { get { return data.surplus; } }
        public byte warhead { get { return data.warhead; } }
        public float delay { get { return data.delay; } }
        public float spread { get { return data.spread; } }
        public float maxSpread { get { return data.maxSpread; } }
        public float spreadDecrese { get { return data.spreadDecrese; } }
        public float changeDelay { get { return data.changeDelay; } }
        public float reload { get { return data.reload; } }
        public bool automatic { get { return data.automatic; } }
        public IStarryDataReference<BulletData>[] bullet { get { return data.bullet; } }
        public string animationName { get { return data.animationName; } }
        public string icon { get { return data.icon; } }
        public string sprite { get { return data.sprite; } }
        public string magazineSprite { get { return data.magazineSprite; } }
        public string killmark { get { return data.killmark; } }
        public string aimSprite { get { return data.aimSprite; } }

        public WeaponInstance(WeaponData data)
        {
            this.data = data;
        }

        static WeaponInstance()
        {

        }

        public WeaponInstance(uint id) : this(WeaponData.Get(id)) { }
        public WeaponInstance(string name) : this(WeaponData.Get(name)) { }
    }

    public class WeaponInstanceComparer : IEqualityComparer<WeaponInstance>
    {
        public bool Equals(WeaponInstance x, WeaponInstance y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(WeaponInstance obj)
        {
            return unchecked((int)obj.id);
        }
    }

    public static partial class DataController
    {
        public static List<WeaponData> weaponDataList = new List<WeaponData>();
        public static Dictionary<uint, WeaponData> weaponDataIDTable = new Dictionary<uint, WeaponData>();
        public static Dictionary<string, WeaponData> weaponDataNameTable = new Dictionary<string, WeaponData>();
    }

    public sealed class WeaponDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<WeaponData>
    {
        public void Serialize(ref MessagePackWriter writer, WeaponData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public WeaponData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            return WeaponData.Get(__id__);
        }
    }

    public sealed class WeaponInstanceFormatter : global::MessagePack.Formatters.IMessagePackFormatter<WeaponInstance>
    {
        public void Serialize(ref MessagePackWriter writer, WeaponInstance value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteArrayHeader(0);
                return;
            }

            writer.WriteArrayHeader(1);
            writer.WriteUInt32(value.id);
        }

        public WeaponInstance Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

            WeaponInstance result = new WeaponInstance(__id__);

            return result;
        }
    }

    public static partial class WeaponEx
    {
        public static void Write(this ref MessagePackWriter writer, WeaponData value)
        {
            writer.Write(value.id);
        }

        public static void Write(this ref MessagePackWriter writer, List<WeaponData> value)
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

        public static void Write(this ref MessagePackWriter writer, IStarryDataReference<WeaponData> value)
        {
            writer.Write(value.Get().id);
        }

        public static void Read(this ref MessagePackReader reader, out WeaponData value)
        {
            value = WeaponData.Get(reader.ReadUInt16());
        }

        public static void Read(this ref MessagePackReader reader, out List<WeaponData> value)
        {
            int length = reader.ReadInt32();
            value = new List<WeaponData>();
            for (int i = 0; i < length; i++)
            {
                WeaponData atom;
                reader.Read(out atom);
                value.Add(atom);
            }
        }

        public static void Read(this ref MessagePackReader reader, out IStarryDataReference<WeaponData> value)
        {
            value = new WeaponDataID(reader.ReadUInt16());
        }
    }
}