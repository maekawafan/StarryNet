using ExcelDataReader;
using System;
using System.Data;
using System.IO;
using StarryNet.StarryLibrary;

namespace StarryNet.StarryDataGenerator
{
    public static partial class ClassGenerator
    {
        public static void ParseDatas(string dataFilePath)
        {
            string[] dataFiles = DirectoryEx.GetFiles(dataFilePath, "*.xlsx|*.xlsm", SearchOption.AllDirectories);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            foreach (string path in dataFiles)
            {
                if (path.Contains("~$"))
                    continue;
                Log.Info("파싱", path);
                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (stream)
                {
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    DataSet dataSet = excelReader.AsDataSet();
                    ParseWorkbook(dataSet, Path.GetFullPath(path));
                }
            }
        }

        public static void ParseWorkbook(DataSet dataSet, string path)
        {
            foreach (DataTable table in dataSet.Tables)
            {
                Log.Info("데이터", table.TableName);
                ParseWorksheet(table);
            }
        }

        public static ClassData GetClassData(string className)
        {
            if (classDictionary.TryGetValue(className, out var classData))
                return classData;
            ClassData result = new ClassData(className);
            classDictionary.Add(className, result);
            return result;
        }

        public static void ParseWorksheet(DataTable table)
        {
            if (table.Rows.Count < 2)
                return;

            ClassData classData = GetClassData(table.TableName);

            for (int x = 0; x < table.Columns.Count; x++)
            {
                object typeCell = table.Rows[0][x];
                object nameCell = table.Rows[1][x];
                if (nameCell == null)
                    continue;

                string typeName = typeCell?.ToString();
                string valueName = nameCell.ToString();

                if (!ValueData.ValidateData(typeName?.ToLower(), valueName))
                {
                    Console.WriteLine($"[{table.TableName}] {x}번 열의 데이터 파싱에 실패했습니다.");
                    continue;
                }

                ValueData valueData = classData.AppendValueData(typeName, valueName);
                if (valueData != null)
                {
                    if (valueData.dataType == DataType.LocalEnum)
                    {
                        for (int y = 2; y < table.Rows.Count; y++)
                            valueData.AppendEnumData(table.Rows[y][x]?.ToString());
                    }
                    else if (valueData.dataType == DataType.Vector)
                    {
                        for (int y = 2; y < table.Rows.Count; y++)
                            valueData.AppendVectorData(table.Rows[y][x]?.ToString());
                    }
                }
            }
        }
    }
}