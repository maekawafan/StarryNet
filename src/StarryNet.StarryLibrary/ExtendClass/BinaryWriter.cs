using System.IO;
using UnityEngine;

namespace StarryNet.StarryLibrary
{
    public static class BinaryWriterEx
    {
        public static void Write(this BinaryWriter writer, Vector2 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public static void Write(this BinaryWriter writer, Vector3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public static void Write(this BinaryWriter writer, Vector4 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public static void Write(this BinaryWriter writer, Vector2Int value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public static void Write(this BinaryWriter writer, Vector3Int value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public static void Write(this BinaryWriter writer, Color value)
        {
            writer.Write(value.r);
            writer.Write(value.g);
            writer.Write(value.b);
            writer.Write(value.a);
        }
    }
}