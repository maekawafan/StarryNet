using System;
using System.Collections.Generic;
using System.Reflection;

namespace StarryNet.StarryLibrary
{
    public static class PacketController
    {
        public static Dictionary<ushort, (Type type, bool isFast)> packetDictionary = new Dictionary<ushort, (Type, bool)>();
        public static Dictionary<Type, ushort> typeToPacketIndex = new Dictionary<Type, ushort>();
        public static Dictionary<string, (Type type, bool isFast)> stringToPacket = new Dictionary<string, (Type, bool)>();

        public static void Initialize(params Assembly[] assemblies)
        {
            Clear();
            List<Type> packetType = typeof(Packet).GetSubClassList(assemblies);
            packetType.PacketSort();

            ushort index = 1;
            foreach (Type type in packetType)
            {
                bool isFast = type.IsSubclassOf(typeof(FastPacket));
                packetDictionary.Add(index, (type, isFast));
                typeToPacketIndex.Add(type, index);
                stringToPacket.Add(type.FullName, (type, isFast));
                index++;
            }
        }

        public static void Initialize(Type standardType, params Assembly[] assemblies)
        {
            Clear();
            List<Type> packetType = standardType.GetSubClassList(assemblies);
            packetType.PacketSort();

            ushort index = 1;
            foreach (Type type in packetType)
            {
                bool isFast = type.IsSubclassOf(typeof(FastPacket));
                packetDictionary.Add(index, (type, isFast));
                typeToPacketIndex.Add(type, index);
                stringToPacket.Add(type.FullName, (type, isFast));
                index++;
            }
        }

        public static void Clear()
        {
            packetDictionary.Clear();
            typeToPacketIndex.Clear();
            stringToPacket.Clear();
        }

        public static ushort GetIndex(Type type)
        {
            if (typeToPacketIndex.TryGetValue(type, out var id))
                return id;
            return 0;
        }

        public static ushort GetIndex<T>()
        {
            if (typeToPacketIndex.TryGetValue(typeof(T), out var id))
                return id;
            return 0;
        }

        public static Type GetType(ushort index)
        {
            if (packetDictionary.TryGetValue(index, out var typeTuple))
                return typeTuple.type;
            return null;
        }

        public static (Type type, bool isFast) GetTypeTuple(ushort index)
        {
            if (packetDictionary.TryGetValue(index, out var typeTuple))
                return typeTuple;
            return (null, false);
        }

        public static Type GetType(string typeName)
        {
            if (stringToPacket.TryGetValue(typeName, out var typeTuple))
                return typeTuple.type; ;
            return null;
        }
    }

    public static class PacketEx
    {
        public static void PacketSort(this List<Type> packetList)
        {
            packetList.Sort((a, b) => { return a.Name.CompareTo(b.Name); });
        }
    }
}
