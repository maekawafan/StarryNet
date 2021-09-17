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
                        text += $"{space}{value.valueName} = data.{value.valueName};\n";
                    }
                    RemoveLineAndInsert(ref code, "[InstanceInitialize]", text);
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
                }
                RemoveLineAndInsert(ref resolverTemplate, "[DataFormatter]", resolver);

                File.WriteAllText($"{exportPath}/../Resolver.cs", resolverTemplate);
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