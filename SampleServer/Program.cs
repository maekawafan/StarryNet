using System;

using StarryNet.StarryLibrary;
using StarryNet.StarryData;
using StarryNet.ServerModule;

namespace SampleServer
{
    class SampleServer
    {
        static void Main(string[] args)
        {
            Log.Initialize(new ConsoleLog());

            StarryDataController.LoadAllData("../sample/SampleClient/Assets/Data/");

            Log.Write(ItemData.Get(1));
            var ins = new ItemInstance(3);
            Log.Write(ins.test.Length);
            ins.upgrade = 8;
            Log.Write(ins.upgrade);
            Console.ReadLine();
        }
    }
}
