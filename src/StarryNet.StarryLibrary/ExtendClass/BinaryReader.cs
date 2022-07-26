using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace StarryNet.StarryLibrary
{
    public static class BinaryReaderEx
    {
        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            return new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static Vector4 ReadVector4(this BinaryReader reader)
        {
            return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static Vector2Int ReadVector2Int(this BinaryReader reader)
        {
            return new Vector2Int(reader.ReadInt32(), reader.ReadInt32());
        }

        public static Vector3Int ReadVector3Int(this BinaryReader reader)
        {
            return new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        }

        public static Color ReadColor(this BinaryReader reader)
        {
            return new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static Quaternion ReadQuaternion(this BinaryReader reader)
        {
            return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static List<T> ReadList<T>(this BinaryReader reader, Func<BinaryReader, T> readAction)
        {
            int count = reader.ReadInt32();

            List<T> result = new List<T>(count);
            for(int i = 0; i < count; i++)
                result.Add(readAction(reader));
            return result;
        }
    }
}