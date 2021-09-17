using System.IO;
using System.Linq;

namespace StarryNet.StarryLibrary
{
    public static class DirectoryEx
    {
        public static string[] GetFiles(string sourceFolder, string filters, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return filters.Split('|')
                          .SelectMany(filter => Directory.GetFiles(sourceFolder, filter, searchOption))
                          .ToArray();
        }

        public static void DeleteAllFiles(this DirectoryInfo directoryInfo, bool includeFolder = false)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
                file.Delete();
            if (includeFolder)
                foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
                    subDirectory.Delete(true);
        }

        public static void GenerateFolder(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path).Parent;

            if (info.Exists == false)
                info.Create();
        }
    }
}