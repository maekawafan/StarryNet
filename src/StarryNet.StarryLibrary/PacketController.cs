using System;
using System.Collections.Generic;
using System.Reflection;

namespace StarryNet.StarryLibrary
{
    public static class PacketController
    {
        public static Dictionary<ushort, Type> packetDictionary = new Dictionary<ushort, Type>();
        public static Dictionary<Type, ushort> typeToPacketIndex = new Dictionary<Type, ushort>();
        public static Dictionary<string, Type> stringToPacket = new Dictionary<string, Type>();

        public static void Initialize(params Assembly[] assemblies)
        {
            Clear();
            List<Type> packetType = typeof(Packet).GetSubClassList(assemblies);
            packetType.PacketSort();

            ushort index = 1;
            foreach (Type type in packetType)
            {
                packetDictionary.Add(index, type);
                typeToPacketIndex.Add(type, index);
                stringToPacket.Add(type.Name, type);
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
                packetDictionary.Add(index, type);
                typeToPacketIndex.Add(type, index);
                stringToPacket.Add(type.Name, type);
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
            if (packetDictionary.TryGetValue(index, out var type))
                return type;
            return null;
        }

        public static Type GetType(string typeName)
        {
            if (stringToPacket.TryGetValue(typeName, out var type))
                return type;
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
