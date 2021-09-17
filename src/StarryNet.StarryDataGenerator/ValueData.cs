using System;
using System.Collections.Generic;
using StarryNet.StarryLibrary;

namespace StarryNet.StarryDataGenerator
{
    public static partial class ClassGenerator
    {
        public class ValueData
        {
            public ClassData classData;
            public DataType dataType;
            public string rawTypeName;
            public string valueName;
            public string subOption;  // a:b에서 b값
            public HashSet<string> localEnumValues = new HashSet<string>();
            public bool isArray;
            public bool isInstanceValue;

            public struct VectorStatus
            {
                public int maxCount;
                public bool isDecimal;
            }
            public VectorStatus vectorStatus;

            public ValueData(ClassData classData, DataType dataType, string rawTypeName, string valueName, bool isArray, bool isInstanceValue, string subOption = null)
            {
                this.classData = classData;
                this.dataType = dataType;
                this.rawTypeName = rawTypeName.Replace("[]", string.Empty);
                this.valueName = valueName;
                this.isArray = isArray;
                this.isInstanceValue = isInstanceValue;
                this.subOption = subOption;
            }

            public static bool ValidateData(string typeName, string valueName)
            {
                if (autoParseTypeTable.ContainsKey(valueName))
                    return true;
                return !string.IsNullOrEmpty(typeName);
            }

            public bool IsDefaultValue()
            {
                return autoParseTypeTable.ContainsKey(valueName);
            }

            public static DataType FindType(string typeName, string valueName)
            {
                if (autoParseTypeTable.TryGetValue(valueName, out var autoParseType))
                    return autoParseType;

                if (typeName.Contains("[]"))
                    typeName = typeName.Replace("[]", string.Empty);

                if (typeTable.TryGetValue(typeName, out var sameTypePair))
                    if (!sameTypePair.isStartWord)
                        return sameTypePair.type;

                string firstToken = typeName.Split(':')?[0];
                if (firstToken != null)
                {
                    firstToken += ":";
                    if (typeTable.TryGetValue(firstToken, out var startTypePair))
                        if (startTypePair.isStartWord)
                            return startTypePair.type;
                }

                return DataType.Reference;
            }

            public string GetValueCodeTypeName(bool forInstance = false)
            {
                switch (dataType)
                {
                    case DataType.LocalEnum:
                        {
                            if (forInstance)
                                return $"{classData.className}Data.{valueName.ToPascalCase()}{(isArray ? "[]" : string.Empty)}";
                            else
                                return valueName.ToPascalCase() + (isArray ? "[]" : string.Empty);
                        }

                    case DataType.GlobalEnum:
                        return subOption + (isArray ? "[]" : string.Empty);

                    case DataType.Reference:
                        {
                            if (forInstance && isInstanceValue)
                                return isArray ? $"List<{rawTypeName}Data>" : $"{rawTypeName}Data";
                            return $"IStarryDataReference<{rawTypeName}Data>" + (isArray ? "[]" : string.Empty);
                        }
                    case DataType.Vector:
                        return GetVectorCodeName();// + (isArray ? "[]" : string.Empty);

                    default:
                        {
                            if (codeTypeTable.TryGetValue(dataType, out var codeType))
                            {
                                if (forInstance && isInstanceValue)
                                    return isArray ? $"List<{codeType}>" : codeType;
                                return codeType + (isArray ? "[]" : string.Empty);
                            }
                            break;
                        }
                };
                return string.Empty;
            }

            private string GetVectorCodeName()
            {
                if (vectorStatus.isDecimal)
                {
                    if (vectorStatus.maxCount == 2)
                        return "Vector2";
                    else if (vectorStatus.maxCount == 3)
                        return "Vector3";
                    else if (vectorStatus.maxCount == 4)
                        return "Vector4";
                }
                else
                {
                    if (vectorStatus.maxCount == 2)
                        return "Vector2Int";
                    else if (vectorStatus.maxCount == 3)
                        return "Vector3Int";
                }
                return "Vector4";
            }

            public void AppendEnumData(string enumValue)
            {
                if (enumValue == null || dataType != DataType.LocalEnum)
                    return;
                if (isArray)
                {
                    string[] tokens = enumValue.Split(splitCharacter);
                    foreach (var token in tokens)
                        TryAddEnum(token);
                }
                else
                    TryAddEnum(enumValue);

                void TryAddEnum(string text)
                {
                    if (text == null || text.Length == 0)
                        return;
                    localEnumValues.Add(text);
                }
            }

            public void AppendVectorData(string vectorValue)
            {
                if (vectorValue == null || dataType != DataType.Vector)
                    return;

                string[] tokens = vectorValue.Split(splitCharacter);
                int count = tokens.Length;
                bool isDecimal = vectorValue.Contains('.');

                vectorStatus.maxCount = Math.Max(vectorStatus.maxCount, count);
                vectorStatus.isDecimal |= isDecimal;
            }

            public string GetEnumCode(string space)
            {
                if (dataType != DataType.LocalEnum || localEnumValues.IsEmpty())
                    return string.Empty;

                string result = string.Empty;
                result += space + "public enum " + GetValueCodeTypeName().Replace("[]", string.Empty) + "\n" + space + "{\n";
                foreach (string enumString in localEnumValues)
                    result += $"{space}{tabSpace}{enumString},\n";
                result += space + "};\n\n";
                return result;
            }
        }
    }
}