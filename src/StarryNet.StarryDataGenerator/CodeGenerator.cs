using System.Collections.Generic;
using System.IO;
using StarryNet.StarryLibrary;

namespace StarryNet.StarryDataGenerator
{
    public static partial class ClassGenerator
    {
        public static void CodeGenerate(string exportPath, string templatePath)
        {
            string dataClassTemplate = File.ReadAllText(@$"{templatePath}StarryDataClass.txt");
            string resolverTemplate = File.ReadAllText(@$"{templatePath}StarryDataResolver.txt");

            DirectoryInfo directory = new DirectoryInfo(exportPath);
            directory.DeleteAllFiles();

            foreach (var classData in classDictionary.Values)
            {
                string code = dataClassTemplate;
                code = code.Replace("\t", tabSpace);
                code = code.Replace("[DataName]", classData.className);
                code = code.Replace("[LowerDataName]", classData.className.ToCamelCase());

                {
                    string space = ' '.Fill(SpaceCount(code, "[LocalEnums]"));
                    string text = string.Empty;
                    foreach (ValueData value in classData.valueDatas.Values)
                    {
                        text += value.GetEnumCode(space);
                    }
                    RemoveLineAndInsert(ref code, "[LocalEnums]", text);
                }

                {
                    string space = ' '.Fill(SpaceCount(code, "[Values]"));
                    string text = string.Empty;
                    foreach (ValueData value in classData.valueDatas.Values)
                    {
                        if (value.IsDefaultValue())
                            continue;
                        if (value.isInstanceValue)
                            text += $"{space}public readonly {value.GetValueCodeTypeName()} {value.valueName};\n";
                        else
                            text += $"{space}public readonly {value.GetValueCodeTypeName()} {value.valueName};\n";
                    }
                    RemoveLineAndInsert(ref code, "[Values]", text);
                }

                {
                    string space = ' '.Fill(SpaceCount(code, "[InstanceValues]"));
                    string text = string.Empty;
                    foreach (ValueData value in classData.valueDatas.Values)
                    {
                        if (value.isInstanceValue)
                            text += $"{space}public {value.GetValueCodeTypeName(true)} {value.valueName};\n";
                        else
                            text += $"{space}public {value.GetValueCodeTypeName(true)} {value.valueName} {{ get {{ return data.{value.valueName}; }} }}\n";
                    }
                    RemoveLineAndInsert(ref code, "[InstanceValues]", text);
                }

                {
                    string space = ' '.Fill(SpaceCount(code, "[InstanceInitialize]"));
                    string text = string.Empty;
                    foreach (ValueData value in classData.valueDatas.Values)
                    {
                        if (!value.isInstanceValue)
                            continue;
                        if (value.dataType == DataType.Reference)
                            text += $"{space}{value.valueName} = {value.rawTypeName}Data.GetInstanceValue(data.{value.valueName});\n";
                        else
                            text += $"{space}{value.valueName} = data.{value.valueName};\n";
                    }
                    RemoveLineAndInsert(ref code, "[InstanceInitialize]", text);
                }

                {
                    string space = ' '.Fill(SpaceCount(code, "[InstanceValuesCount]"));
                    int valueCount = classData.valueDatas.Values.ConditionCount((o) => { return o.isInstanceValue; }) + 1;
                    Insert(ref code, "[InstanceValuesCount]", valueCount.ToString());
                }

                {
                    string space = ' '.Fill(SpaceCount(code, "[InstanceSerialize]"));
                    string text = string.Empty;
                    foreach (ValueData value in classData.valueDatas.Values)
                    {
                        if (!value.isInstanceValue)
                            continue;
                        text += $"{space}writer.Write(value.{value.valueName});\n";
                    }
                    RemoveLineAndInsert(ref code, "[InstanceSerialize]", text);
                }

                {
                    string space = ' '.Fill(SpaceCount(code, "[InstanceDeserializeInitialize]"));
                    string text = string.Empty;
                    foreach (ValueData value in classData.valueDatas.Values)
                    {
                        if (!value.isInstanceValue)
                            continue;
                        text += $"{space}{value.GetValueCodeTypeName(true)} __{value.valueName}__ = default({value.GetValueCodeTypeName(true)});\n";
                    }
                    RemoveLineAndInsert(ref code, "[InstanceDeserializeInitialize]", text);
                }

                {
                    string space = ' '.Fill(SpaceCount(code, "[InstanceDeserialize]"));
                    string text = string.Empty;
                    int index = 1;
                    foreach (ValueData value in classData.valueDatas.Values)
                    {
                        if (!value.isInstanceValue)
                            continue;
                        text += $"{space}case {index++}:\n";
                        text += $"{space}    reader.Read(out __{value.valueName}__);\n";
                        text += $"{space}    break;\n";
                    }
                    RemoveLineAndInsert(ref code, "[InstanceDeserialize]", text);
                }

                {
                    string space = ' '.Fill(SpaceCount(code, "[InstanceDeserializeSet]"));
                    string text = string.Empty;
                    foreach (ValueData value in classData.valueDatas.Values)
                    {
                        if (!value.isInstanceValue)
                            continue;
                        text += $"{space}result.{value.valueName} = __{value.valueName}__;\n";
                    }
                    RemoveLineAndInsert(ref code, "[InstanceDeserializeSet]", text);
                }

                File.WriteAllText($"{exportPath}/{classData.className}.cs", code);
            }

            {
                string resolver = string.Empty;
                string space = ' '.Fill(SpaceCount(resolverTemplate, "[DataFormatter]"));

                foreach (var classData in classDictionary.Values)
                {
                    resolver += $"{space}{{ typeof({classData.className}Data), new {classData.className}DataFormatter() }},\n";
                    resolver += $"{space}{{ typeof({classData.className}Data[]), new ArrayFormatter<{classData.className}Data>() }},\n";
                    resolver += $"{space}{{ typeof(List<{classData.className}Data>), new ListFormatter<{classData.className}Data>() }},\n";
                    resolver += $"{space}{{ typeof({classData.className}Instance), new {classData.className}InstanceFormatter() }},\n";
                    resolver += $"{space}{{ typeof({classData.className}Instance[]), new ArrayFormatter<{classData.className}Instance>() }},\n";
                    resolver += $"{space}{{ typeof(List<{classData.className}Instance>), new ListFormatter<{classData.className}Instance>() }},\n";
                }
                RemoveLineAndInsert(ref resolverTemplate, "[DataFormatter]", resolver);

                File.WriteAllText($"{exportPath}/../Resolver.cs", resolverTemplate);
            }

            void Insert(ref string code, string origin, string replace)
            {
                code = code.Replace(origin, replace);
            }

            void RemoveLineAndInsert(ref string code, string origin, string replace)
            {
                int index = code.IndexOf(origin);
                do
                {
                    var lineTuple = code.GetLineIndex(index);

                    code = code.Remove(lineTuple.index, lineTuple.length);
                    code = code.Insert(lineTuple.index, replace);
                    index = code.IndexOf(origin);
                } while (index > 0);
            }

            int SpaceCount(string code, string text)
            {
                int textStartIndex = code.IndexOf(text);
                int lineStartIndex = code.Substring(0, textStartIndex).LastIndexOf('\n') + 1;
                return textStartIndex - lineStartIndex;
            }
        }
    }
}