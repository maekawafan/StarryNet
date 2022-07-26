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

            Log.Info("파싱 시작");
            ParseDatas(dataFilePath);
            Log.Info("코드 생성 시작");
            CodeGenerate(exportPath, templatePath);
            Log.Info("작업 완료");
        }
    }
}
