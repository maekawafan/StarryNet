using System;
using System.Collections.Generic;
using StarryNet.StarryLibrary;

namespace StarryNet.StarryDataGenerator
{
    public static partial class ClassGenerator
    {
        public class ClassData
        {
            public string className;

            public Dictionary<string, ValueData> valueDatas = new Dictionary<string, ValueData>();

            public ClassData(string className)
            {
                this.className = className;
            }

            public ValueData AppendValueData(string typeName, string valueName)
            {
                bool isInstanceValue = typeName.EndsWith("*");
                if (isInstanceValue)
                    typeName = typeName.CropRight(1);
                DataType dataType = ValueData.FindType(typeName?.ToLower(), valueName);

                if (valueDatas.TryGetValue(valueName, out var origin))
                {
                    if (dataType != origin.dataType)
                    {
                        Console.WriteLine($"{valueName} 데이터 타입이 다릅니다. ({origin.dataType}, {dataType})");
                        return null;
                    }
                    return origin;
                }
                else if (!string.IsNullOrEmpty(typeName))
                {
                    string[] token = typeName.Split(':');
                    string subOption = null;
                    if (token.Length >= 2)
                        subOption = token[1];
                    bool isArray = false;
                    if (typeName != null && typeName.Contains("[]"))
                        isArray = true;
                    ValueData valueData = new ValueData(this, dataType, typeName, valueName, isArray, isInstanceValue, subOption);
                    valueDatas.Add(valueName, valueData);
                    return valueData;
                }
                return null;
            }
        }
    }
}