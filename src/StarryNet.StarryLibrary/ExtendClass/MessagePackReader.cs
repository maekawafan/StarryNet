using MessagePack;

using System;
using System.Collections.Generic;
using System.Text;

namespace StarryNet.StarryLibrary
{
    public static partial class MessagePackReaderEx
    {
        public static void Read(this ref MessagePackReader reader, out bool value)
        {
            value = reader.ReadBoolean();
        }

        public static void Read(this ref MessagePackReader reader, out byte value)
        {
            value = reader.ReadByte();
        }

        public static void Read(this ref MessagePackReader reader, out short value)
        {
            value = reader.ReadInt16();
        }

        public static void Read(this ref MessagePackReader reader, out int value)
        {
            value = reader.ReadInt32();
        }

        public static void Read(this ref MessagePackReader reader, out long value)
        {
            value = reader.ReadInt64();
        }

        public static void Read(this ref MessagePackReader reader, out sbyte value)
        {
            value = reader.ReadSByte();
        }

        public static void Read(this ref MessagePackReader reader, out ushort value)
        {
            value = reader.ReadUInt16();
        }

        public static void Read(this ref MessagePackReader reader, out uint value)
        {
            value = reader.ReadUInt32();
        }

        public static void Read(this ref MessagePackReader reader, out ulong value)
        {
            value = reader.ReadUInt64();
        }

        public static void Read(this ref MessagePackReader reader, out string value)
        {
            value = reader.ReadString();
        }

        public static void Read(this ref MessagePackReader reader, out float value)
        {
            value = reader.ReadSingle();
        }

        public static void Read(this ref MessagePackReader reader, out double value)
        {
            value = reader.ReadDouble();
        }

        public static void Read(this ref MessagePackReader reader, out DateTime value)
        {
            value = reader.ReadDateTime();
        }
    }
}