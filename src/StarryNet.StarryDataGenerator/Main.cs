using StarryNet.StarryLibrary;

using System;

namespace StarryNet.StarryDataGenerator
{
    public static partial class ClassGenerator
    {
        static void Main(string[] args)
        {
            ServerTime.StartTimer(false);
            Log.Initialize(new ConsoleLog());
            string dataFilePath = args[0];
            string exportPath = args[1];
            string templatePath = args[2];

            ParseDatas(dataFilePath);
            CodeGenerate(exportPath, templatePath);
            Console.WriteLine("파싱 완료");
        }
    }
}
