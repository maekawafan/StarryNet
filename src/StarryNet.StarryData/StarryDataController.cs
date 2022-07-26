using ExcelDataReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;
using StarryNet.StarryLibrary;

using BigInteger = System.Numerics.BigInteger;

namespace StarryNet.StarryData
{
    public static partial class StarryDataController
    {
        public const char splitCharacter = ',';

        public static Dictionary<Type, IDictionary> allDataDictionary = new Dictionary<Type, IDictionary>();

        public static Dictionary<string, Type> objectTypes                  = new Dictionary<string, Type>();
        public static Dictionary<string, Type> idTypes                      = new Dictionary<string, Type>();
        public static Dictionary<string, Type> nameTypes                    = new Dictionary<string, Type>();
        public static Dictionary<Type, HashSet<StarryDataID>> verifyID      = new Dictionary<Type, HashSet<StarryDataID>>();
        public static Dictionary<Type, HashSet<StarryDataName>> verifyName  = new Dictionary<Type, HashSet<StarryDataName>>();

        static StarryDataController()
        {
            foreach (Type type in typeof(StarryData).GetSubClassList())
            {
                objectTypes.Add(type.Name, type);
                allDataDictionary.Add(type, type.GetMethod("GetDictionary").Invoke(null, null) as IDictionary);
            }
            foreach (Type type in typeof(StarryDataID).GetSubClassList())
                idTypes.Add(type.Name, type);
            foreach (Type type in typeof(StarryDataName).GetSubClassList())
                nameTypes.Add(type.Name, type);
        }

        public static bool VerifyAllData()
        {
            foreach (var pair in verifyID)
            {
                Type type = pair.Key;
                foreach (var dataID in pair.Value)
                {
                    var data = type.GetMethod("Get", new[] { typeof(uint) }).Invoke(null, new object[] { dataID.id });
                    if (data == null)
                    {
                        Log.Error("DataController", $"레퍼런스 검증 실패 [{type.Name}] 타입의 [id:{dataID.id}]가 없습니다.");
                        return false;
                    }
                }
            }
            foreach (var pair in verifyName)
            {
                Type type = pair.Key;
                foreach (var dataName in pair.Value)
                {
                    var data = type.GetMethod("Get", new[] { typeof(string) }).Invoke(null, new object[] { dataName.name });
                    if (data == null)
                    {
                        Log.Error("DataController", $"레퍼런스 검증 실패 [{type.Name}] 타입의 [name:{dataName.name}]가 없습니다.");
                        return false;
                    }
                }
            }

            verifyID.Clear();
            verifyName.Clear();
            return true;
        }

        public static void UnloadAllData()
        {
            foreach (var pair in objectTypes)
                pair.Value.GetMethod("UnloadAll").Invoke(null, null);
            verifyID.Clear();
            verifyName.Clear();
        }

        public static void LoadFromBinary(BinaryReader reader)
        {
            // Header
            string className = reader.ReadString();
            Type type;
            if (!objectTypes.TryGetValue(className, out type))
            {
                Log.Error("DataController", $"바이너리 로드 중 [{className}] 클래스가 존재하지 않습니다!");
                return;
            }

            int fieldCount = reader.ReadInt32();
            FieldInfo[] fieldInfos = new FieldInfo[fieldCount];
            for (int i = 0; i < fieldCount; i++)
            {
                string fieldName = reader.ReadString();
                var fieldInfo = type.GetField(fieldName);
                if (fieldInfo == null)
                {
                    Log.Error("DataController", $"바이너리에 존재하는 [{fieldName}]필드가 클래스에 없습니다!");
                    return;
                }
                fieldInfos[i] = fieldInfo;
            }

            int dataCount = reader.ReadInt32();

            // Table
            for (int i = 0; i < dataCount; i++)
            {
                StarryData data = Activator.CreateInstance(type) as StarryData;

                foreach (var field in fieldInfos)
                {
                    Type valueType = field.FieldType;

                    if (!valueType.IsArray)
                    {
                        field.SetValue(data, LoadBinary(reader, valueType));
                    }
                    else
                    {
                        ushort arrayCount = reader.ReadUInt16();
                        if (arrayCount == 0)
                            continue;

                        Type atomType = valueType.GetElementType();
                        Array array = Array.CreateInstance(atomType, arrayCount);
                        for (int index = 0; index < arrayCount; index++)
                        {
                            array.SetValue(LoadBinary(reader, atomType), index);
                        }
                        field.SetValue(data, array);
                    }
                }

                data.Regist();
            }
        }

        public static bool VerifyFromBinary(BinaryReader reader)
        {
            // Header
            string className = reader.ReadString();
            Type type;
            if (!objectTypes.TryGetValue(className, out type))
            {
                Log.Error("DataController", $"바이너리 로드 중 [{className}] 클래스가 존재하지 않습니다!");
                return false;
            }

            int fieldCount = reader.ReadInt32();
            (FieldInfo field, object value)[] fieldInfos = new (FieldInfo, object)[fieldCount];
            for (int i = 0; i < fieldCount; i++)
            {
                string fieldName = reader.ReadString();
                var fieldInfo = type.GetField(fieldName);
                if (fieldInfo == null)
                {
                    Log.Error("DataController", $"바이너리에 존재하는 [{fieldName}]필드가 클래스에 없습니다!");
                    return false;
                }
                fieldInfos[i] = (fieldInfo, null);
            }

            int dataCount = reader.ReadInt32();

            // Table
            for (int i = 0; i < dataCount; i++)
            {
                uint id = 0;
                for (int j = 0; j < fieldInfos.Length; j++)
                {
                    var pair = fieldInfos[j];
                    Type valueType = pair.field.FieldType;

                    if (!valueType.IsArray)
                    {
                        fieldInfos[j].value = LoadBinary(reader, valueType);
                    }
                    else
                    {
                        ushort arrayCount = reader.ReadUInt16();
                        if (arrayCount == 0)
                            continue;

                        Type atomType = valueType.GetElementType();
                        Array array = Array.CreateInstance(atomType, arrayCount);
                        for (int index = 0; index < arrayCount; index++)
                        {
                            array.SetValue(LoadBinary(reader, atomType), index);
                        }
                        fieldInfos[j].value = array;
                    }

                    if (pair.field.Name == "id")
                        id = (uint)fieldInfos[j].value;
                }

                StarryData origin = type.GetMethod("Get", new[] { typeof(uint) }).Invoke(null, new object[] { id }) as StarryData;

                for (int j = 0; j < fieldInfos.Length; j++)
                {
                    var pair = fieldInfos[j];
                    fieldInfos[j].value = null;

                    if (!pair.field.FieldType.IsArray)
                    {
                        if (pair.field.FieldType.IsGenericType && pair.field.FieldType.GetGenericTypeDefinition() == typeof(IStarryDataReference<>))
                        {
                            var originValue = pair.field.GetValue(origin);
                            var binaryValue = pair.value;

                            var method = pair.field.FieldType.GetMethod("Get");

                            StarryData originTarget = method.Invoke(originValue, null) as StarryData;
                            StarryData binaryTarget = method.Invoke(binaryValue, null) as StarryData;

                            if (originTarget.id != binaryTarget.id)
                            {
                                Log.Error("DataController", $"바이너리 데이터 검증 실패 {type}({pair.field.GetValue(origin)})");
                                return false;
                            }
                        }
                        else if (!pair.field.GetValue(origin).Equals(pair.value))
                        {
                            Log.Error("DataController", $"바이너리 데이터 검증 실패 {type}({pair.field.GetValue(origin)})");
                            return false;
                        }
                    }
                    else
                    {
                        Array originArray = pair.field.GetValue(origin) as Array;
                        Array binaryArray = pair.value as Array;
                        if (originArray == null && binaryArray == null)
                            continue;

                        if (originArray == null || binaryArray == null || originArray.Length != binaryArray.Length)
                        {
                            Log.Error("DataController", $"바이너리 데이터 검증 실패, 배열 크기가 다릅니다.{type} - {pair.field.FieldType}");
                            return false;
                        }

                        for (int k = 0; k < originArray.Length; k++)
                        {
                            if (pair.field.FieldType.IsGenericType && pair.field.FieldType.GetGenericTypeDefinition() == typeof(IStarryDataReference<>))
                            {
                                var originValue = originArray.GetValue(k);
                                var binaryValue = binaryArray.GetValue(k);

                                var method = pair.field.FieldType.GetMethod("Get");

                                StarryData originTarget = method.Invoke(originValue, null) as StarryData;
                                StarryData binaryTarget = method.Invoke(binaryValue, null) as StarryData;

                                if (originTarget.id != binaryTarget.id)
                                {
                                    Log.Error("DataController", $"바이너리 데이터 검증 실패 {type}({pair.field.GetValue(origin)})");
                                    return false;
                                }
                            }
                            if (!originArray.GetValue(k).Equals(binaryArray.GetValue(k)))
                            {
                                Log.Error("DataController", $"바이너리 데이터 검증 실패, 배열 데이터가 다릅니다. {type}({pair.field.GetValue(origin)})");
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private static object LoadBinary(BinaryReader reader, Type valueType)
        {
            if (valueType == typeof(string))
                return reader.ReadString();
            else if (valueType == typeof(bool))
                return reader.ReadBoolean();
            else if (valueType == typeof(byte))
                return reader.ReadByte();
            else if (valueType == typeof(sbyte))
                return reader.ReadSByte();
            else if (valueType == typeof(short))
                return reader.ReadInt16();
            else if (valueType == typeof(ushort))
                return reader.ReadUInt16();
            else if (valueType == typeof(int))
                return reader.ReadInt32();
            else if (valueType == typeof(uint))
                return reader.ReadUInt32();
            else if (valueType == typeof(long))
                return reader.ReadInt64();
            else if (valueType == typeof(ulong))
                return reader.ReadUInt64();
            else if (valueType == typeof(float))
                return reader.ReadSingle();
            else if (valueType == typeof(double))
                return reader.ReadDouble();
            else if (valueType == typeof(decimal))
                return reader.ReadDecimal();
            else if (valueType == typeof(DateTime))
                return new DateTime(reader.ReadInt64());
            else if (valueType == typeof(BigInteger))
            {
                int length = reader.ReadInt32();
                return new BigInteger(reader.ReadBytes(length));
            }
            else if (valueType == typeof(Vector2))
                return reader.ReadVector2();
            else if (valueType == typeof(Vector3))
                return reader.ReadVector3();
            else if (valueType == typeof(Vector4))
                return reader.ReadVector4();
            else if (valueType == typeof(Vector2Int))
                return reader.ReadVector2Int();
            else if (valueType == typeof(Vector3Int))
                return reader.ReadVector3Int();
            else if (valueType == typeof(Color))
                return reader.ReadColor();
            else if (valueType.IsEnum)
                return Enum.ToObject(valueType, reader.ReadInt32());
            else if (valueType.GetGenericTypeDefinition() == typeof(IStarryDataReference<>))
            {
                Type idType = valueType.GetGenericArguments()[0].GetMethod("GetIDReferenceType").Invoke(null, null) as Type;
                StarryDataID target = Activator.CreateInstance(idType) as StarryDataID;
                typeof(StarryDataID).GetField("id").SetValue(target, reader.ReadUInt32());
                return target;
            }
            else
            {
                Log.Error("DataController", $"바이너리 로드에 실패한 타입 - {valueType.Name}");
            }
            return null;
        }

        public static void BuildBinary(string exportDirectory)
        {
            ByteUnit totalSize = new ByteUnit();
            foreach (var pair in allDataDictionary)
            {
                Type type = pair.Key;
                string fileName = exportDirectory + $"{type.Name}.bytes";
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                {
                    // Header
                    writer.Write(type.Name);

                    var fields = type.GetFields();
                    writer.Write(fields.Length);
                    foreach (var field in fields)
                        writer.Write(field.Name);

                    writer.Write(pair.Value.Values.Count);

                    // Table
                    foreach (object data in pair.Value.Values)
                    {
                        StarryData starryData = data as StarryData;
                        foreach (var field in fields)
                        {
                            dynamic value = field.GetValue(data);
                            Type valueType = field.FieldType;

                            if (!valueType.IsArray)
                            {
                                SaveBinary(writer, valueType, value, starryData);
                            }
                            else
                            {
                                if (value == null)
                                {
                                    writer.Write((ushort)0);
                                    continue;
                                }
                                Array array = value as Array;
                                writer.Write((ushort)array.Length);

                                foreach (var atom in array)
                                {
                                    SaveBinary(writer, valueType.GetElementType(), atom, starryData);
                                }
                            }
                        }
                    }
                    totalSize.size += (ulong)writer.BaseStream.Length;
                }
            }

            Log.Info("DataController", $"바이너리 크기 : {totalSize}");

            void SaveBinary(BinaryWriter writer, Type type, dynamic value, StarryData starryData)
            {
                if (type.IsPrimitive || value is string)
                    writer.Write(value);
                else if (type.IsEnum)
                    writer.Write((int)value);
                else if (value is DateTime dateTime)
                    writer.Write(dateTime.Ticks);
                else if (value is BigInteger bigInteger)
                {
                    writer.Write(bigInteger.ToByteArray().Length);
                    writer.Write(bigInteger.ToByteArray());
                }
                else if (value is Vector2 vector2)
                    writer.Write(vector2);
                else if (value is Vector3 vector3)
                    writer.Write(vector3);
                else if (value is Vector4 vector4)
                    writer.Write(vector4);
                else if (value is Vector2Int vector2int)
                    writer.Write(vector2int);
                else if (value is Vector3Int vector3int)
                    writer.Write(vector3int);
                else if (value is Color color)
                    writer.Write(color);
                else if (value is StarryDataID dataID)
                    writer.Write(dataID.id);
                else if (value is StarryDataName dataName)
                {
                    StarryData referenceData = type.GetMethod("Get").Invoke(dataName, null) as StarryData;
                    if (referenceData == null)
                        writer.Write(0u);
                    else
                        writer.Write(referenceData.id);
                }
                else
                    Log.Error("DataController", $"바이너리화에 실패한 타입 - {type.Name} ID[{starryData.id}]");
            }
        }

        public static bool LoadAllData(string targetAddress, bool verify = false)
        {
            string[] dataFiles = DirectoryEx.GetFiles(targetAddress, "*.xlsx|*.xlsm", SearchOption.AllDirectories);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

#if (DEBUG)
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Log.Info("DataController", $"데이터 로드 시작, 파일 갯수 {dataFiles.ConditionCount((o) => !o.Contains("~$"))}개");
#endif
            foreach (string path in dataFiles)
            {
                if (path.Contains("~$"))
                    continue;

                LoadData(Path.GetFullPath(path), verify);
            }
#if (DEBUG)
            Log.Info("DataController", $"데이터 로드 종료, 소요 시간 {watch.ElapsedMilliseconds}ms");
#endif
            if (verify)
                return VerifyAllData();
            return true;
        }

        public static void LoadData(string filePath, bool verify = false)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (stream)
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet dataSet = excelReader.AsDataSet();
                foreach (DataTable table in dataSet.Tables)
                {
                    LoadTable(table, verify);
                }
            }
        }

        public static void LoadTable(DataTable table, bool verify)
        {
            if (table.Rows.Count < 2)
                return;

            string className = $"{table.TableName}Data";
            if (!objectTypes.TryGetValue(className, out var type))
                return;

            // FieldInfo 준비
            List<FieldInfo> fieldInfoList = new List<FieldInfo>();
            for (int x = 0; x < table.Columns.Count; x++)
            {
                string valueName = table.Rows[1][x].ToString();
                FieldInfo fieldInfo = type.GetField(valueName);
                fieldInfoList.Add(fieldInfo);
            }

            for (int y = 2; y < table.Rows.Count; y++)
            {
                StarryData data = Activator.CreateInstance(type) as StarryData;
                for (int x = 0; x < table.Columns.Count; x++)
                {
                    FieldInfo fieldInfo = fieldInfoList[x];
                    if (fieldInfo == null)
                        continue;
                    SetField(data, fieldInfo, table.Rows[y][x], verify);
                }
                if (data.id == 0)
                    continue;
                data.Regist();
            }
        }

        private static object GetFieldData(Type fieldType, string value, bool verify)
        {
            if (value == null)
            {
                if (fieldType == typeof(string))
                    return "";
                else if (fieldType == typeof(bool))
                    return false;
                else if (fieldType == typeof(byte))
                    return 0;
                else if (fieldType == typeof(sbyte))
                    return 0;
                else if (fieldType == typeof(short))
                    return 0;
                else if (fieldType == typeof(ushort))
                    return 0;
                else if (fieldType == typeof(int))
                    return 0;
                else if (fieldType == typeof(uint))
                    return 0;
                else if (fieldType == typeof(long))
                    return 0;
                else if (fieldType == typeof(ulong))
                    return 0;
                else if (fieldType == typeof(float))
                    return 0;
                else if (fieldType == typeof(double))
                    return 0;
                else if (fieldType == typeof(decimal))
                    return 0;
                else if (fieldType == typeof(BigInteger))
                    return 0;
                else if (fieldType == typeof(DateTime))
                    return DateTime.MinValue;
                else if (fieldType == typeof(Vector2))
                    return Vector2.zero;
                else if (fieldType == typeof(Vector3))
                    return Vector3.zero;
                else if (fieldType == typeof(Vector4))
                    return Vector4.zero;
                else if (fieldType == typeof(Vector2Int))
                    return Vector2Int.zero;
                else if (fieldType == typeof(Vector3Int))
                    return Vector3Int.zero;
                else if (fieldType == typeof(Color))
                    return Color.black;
                else if (fieldType.IsEnum)
                    return null;
                else if (fieldType.GetGenericTypeDefinition() == typeof(IStarryDataReference<>))
                    return null;
                return null;
            }

            if (fieldType == typeof(string))
                return value.ToString();
            else if (fieldType == typeof(bool))
                return Convert.ToBoolean(value);
            else if (fieldType == typeof(byte))
                return Convert.ToByte(value);
            else if (fieldType == typeof(sbyte))
                return Convert.ToSByte(value);
            else if (fieldType == typeof(short))
                return Convert.ToInt16(value);
            else if (fieldType == typeof(ushort))
                return Convert.ToUInt16(value);
            else if (fieldType == typeof(int))
                return Convert.ToInt32(value);
            else if (fieldType == typeof(uint))
                return Convert.ToUInt32(value);
            else if (fieldType == typeof(long))
                return Convert.ToInt64(value);
            else if (fieldType == typeof(ulong))
                return Convert.ToUInt64(value);
            else if (fieldType == typeof(float))
                return Convert.ToSingle(value);
            else if (fieldType == typeof(double))
                return Convert.ToDouble(value);
            else if (fieldType == typeof(decimal))
                return Convert.ToDecimal(value);
            else if (fieldType == typeof(BigInteger))
                return BigInteger.Parse(value.ToString());
            else if (fieldType == typeof(DateTime))
                return Convert.ToDateTime(value);
            else if (fieldType == typeof(Vector2))
            {
                string[] token = value.ToString().Split(splitCharacter);
                return new Vector2(Convert.ToSingle(token[0]), Convert.ToSingle(token[1]));
            }
            else if (fieldType == typeof(Vector3))
            {
                string[] token = value.ToString().Split(splitCharacter);
                if (token.Length == 2)
                    return new Vector3(Convert.ToSingle(token[0]), Convert.ToSingle(token[1]));
                else
                    return new Vector3(Convert.ToSingle(token[0]), Convert.ToSingle(token[1]), Convert.ToSingle(token[2]));
            }
            else if (fieldType == typeof(Vector4))
            {
                string[] token = value.ToString().Split(splitCharacter);
                if (token.Length == 2)
                    return new Vector4(Convert.ToSingle(token[0]), Convert.ToSingle(token[1]));
                else if (token.Length == 3)
                    return new Vector4(Convert.ToSingle(token[0]), Convert.ToSingle(token[1]), Convert.ToSingle(token[2]));
                else
                    return new Vector4(Convert.ToSingle(token[0]), Convert.ToSingle(token[1]), Convert.ToSingle(token[2]), Convert.ToSingle(token[3]));
            }
            else if (fieldType == typeof(Vector2Int))
            {
                string[] token = value.ToString().Split(splitCharacter);
                return new Vector2Int(Convert.ToInt32(token[0]), Convert.ToInt32(token[1]));
            }
            else if (fieldType == typeof(Vector3Int))
            {
                string[] token = value.ToString().Split(splitCharacter);
                if (token.Length == 2)
                    return new Vector3Int(Convert.ToInt32(token[0]), Convert.ToInt32(token[1]), 0);
                else
                    return new Vector3Int(Convert.ToInt32(token[0]), Convert.ToInt32(token[1]), Convert.ToInt32(token[2]));
            }
            else if (fieldType == typeof(Color))
            {
                if (value.Contains(splitCharacter))
                {
                    string[] token = value.ToString().Split(splitCharacter);
                    if (token.Length == 3)
                        return new Color(Convert.ToSingle(token[0]), Convert.ToSingle(token[1]), Convert.ToSingle(token[2]));
                    else
                        return new Color(Convert.ToSingle(token[0]), Convert.ToSingle(token[1]), Convert.ToSingle(token[2]), Convert.ToSingle(token[3]));
                }
                else
                {
                    if (value.StartsWith("0x"))
                        value = value.CropLeft(2);
                    else if (value.StartsWith("#"))
                        value = value.CropLeft(1);
                    string[] token = value.Divide(2);
                    Color result = new Color();
                    if (token.Length >= 1)
                        result.r = Convert.ToInt32(token[0], 16) / 255.0f;
                    if (token.Length >= 2)
                        result.g = Convert.ToInt32(token[1], 16) / 255.0f;
                    if (token.Length >= 3)
                        result.b = Convert.ToInt32(token[2], 16) / 255.0f;
                    if (token.Length >= 4)
                        result.a = Convert.ToInt32(token[3], 16) / 255.0f;
                    else
                        result.a = 1.0f;
                    return result;
                }
            }
            else if (fieldType.IsEnum)
                return Enum.Parse(fieldType, value.ToString());
            else if (fieldType.GetGenericTypeDefinition() == typeof(IStarryDataReference<>))
            {
                Type targetType = fieldType.GetGenericArguments()[0];

                if (uint.TryParse(value.ToString(), out uint id))
                {
                    Type referenceType = idTypes[targetType.Name + "ID"];
                    StarryDataID dataID = Activator.CreateInstance(referenceType) as StarryDataID;
                    referenceType.GetField("id").SetValue(dataID, id);

                    if (verify)
                    {
                        if (!verifyID.ContainsKey(targetType))
                            verifyID.Add(targetType, new HashSet<StarryDataID>(new StarryDataID.Comparer()));
                        verifyID[targetType].Add(dataID);
                    }
                    return dataID;
                }
                else
                {
                    Type referenceType = nameTypes[targetType.Name + "Name"];
                    StarryDataName dataName = Activator.CreateInstance(referenceType) as StarryDataName;
                    referenceType.GetField("name").SetValue(dataName, value.ToString());

                    if (verify)
                    {
                        if (!verifyName.ContainsKey(targetType))
                            verifyName.Add(targetType, new HashSet<StarryDataName>(new StarryDataName.Comparer()));
                        verifyName[targetType].Add(dataName);
                    }
                    return dataName;
                }
            }
            return null;
        }

        private static void SetField(StarryData data, FieldInfo fieldInfo, object cellValue, bool verify = false)
        {
            try
            {
                if (!fieldInfo.FieldType.IsArray)
                {
                    fieldInfo.SetValue(data, GetFieldData(fieldInfo.FieldType, cellValue?.ToString(), verify));
                }
                else
                {
                    string[] tokens = cellValue.ToString().Split(splitCharacter);
                    if (tokens.Length == 0 || tokens[0].ToString() == string.Empty)
                        return;
                    Array array = Array.CreateInstance(fieldInfo.FieldType.GetElementType(), tokens.Length);
                    for (int i = 0; i < tokens.Length; i++)
                        array.SetValue(GetFieldData(fieldInfo.FieldType.GetElementType(), tokens[i], verify), i);
                    fieldInfo.SetValue(data, array);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{fieldInfo.Name}의 값에 [{cellValue}]데이터를 {fieldInfo.FieldType}타입으로 읽을 수 없습니다.\n{e.Message}");
            }
        }
    }
}