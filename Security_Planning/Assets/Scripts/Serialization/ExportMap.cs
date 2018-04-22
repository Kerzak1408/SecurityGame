using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using Assets.Scripts.Helpers;
using Boo.Lang;
using SFB;

namespace Assets.Scripts.Serialization
{
    public static class ExportMap
    {
        public static void Export(string directoryPath, string mapName)
        {
            string path = StandaloneFileBrowser.SaveFilePanel("Export map", "", mapName, "map");

            if (path.Length != 0)
            {
                CompressDirectory(directoryPath, path);
            }
        }

        public static void ExportToFolder(string[] mapPaths, string[] mapNames)
        {
            Debug.Assert(mapPaths.Length == mapNames.Length);
            string[] paths = StandaloneFileBrowser.OpenFolderPanel("Choose folder", "", false);

            if (paths.Length != 0)
            {
                string path = paths[0];
                if (path.Length != 0)
                {
                    for (int i = 0; i < mapPaths.Length; i++)
                    {
                        CompressDirectory(mapPaths[i], FileHelper.JoinPath(path, mapNames[i]));
                    }
                }
            }
        }

        public static string[] Import(string mapsPath)
        {
            var importedNames = new List<string>();
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Import map", "", "map", true);
            foreach (string path in paths)
            {
                string name = FileHelper.GetFileNameOnly(path);
                string potentialName = name;
                int i = 1;
                string potentialPath = FileHelper.JoinPath(mapsPath, name);
                while (Directory.Exists(potentialPath))
                {
                    potentialName = name + "_(" + i + ")";
                    potentialPath = FileHelper.JoinPath(mapsPath, potentialName);
                    i++;
                }
                DecompressToDirectory(path, potentialPath);
                importedNames.Add(potentialName);
            }

            return importedNames.ToArray();
        }

        static void CompressDirectory(string sInDir, string sOutFile)
        {
            string[] sFiles = Directory.GetFiles(sInDir, "*.*", SearchOption.AllDirectories);
            int iDirLen = sInDir[sInDir.Length - 1] == Path.DirectorySeparatorChar ? sInDir.Length : sInDir.Length + 1;

            using (FileStream outFile = new FileStream(sOutFile, FileMode.Create, FileAccess.Write, FileShare.None))
            using (GZipStream str = new GZipStream(outFile, CompressionMode.Compress))
                foreach (string sFilePath in sFiles)
                {
                    string sRelativePath = sFilePath.Substring(iDirLen);
                    CompressFile(sInDir, sRelativePath, str);
                }
        }

        static void DecompressToDirectory(string sCompressedFile, string sDir)
        {
            using (FileStream inFile = new FileStream(sCompressedFile, FileMode.Open, FileAccess.Read, FileShare.None))
            using (GZipStream zipStream = new GZipStream(inFile, CompressionMode.Decompress, true))
                while (DecompressFile(sDir, zipStream)) ;
        }

        static void CompressFile(string sDir, string sRelativePath, GZipStream zipStream)
        {
            //Compress file name
            char[] chars = sRelativePath.ToCharArray();
            zipStream.Write(BitConverter.GetBytes(chars.Length), 0, sizeof(int));
            foreach (char c in chars)
                zipStream.Write(BitConverter.GetBytes(c), 0, sizeof(char));

            //Compress file content
            byte[] bytes = File.ReadAllBytes(Path.Combine(sDir, sRelativePath));
            zipStream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
            zipStream.Write(bytes, 0, bytes.Length);
        }

        static bool DecompressFile(string sDir, GZipStream zipStream)
        {
            //Decompress file name
            byte[] bytes = new byte[sizeof(int)];
            int Readed = zipStream.Read(bytes, 0, sizeof(int));
            if (Readed < sizeof(int))
                return false;

            int iNameLen = BitConverter.ToInt32(bytes, 0);
            bytes = new byte[sizeof(char)];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < iNameLen; i++)
            {
                zipStream.Read(bytes, 0, sizeof(char));
                char c = BitConverter.ToChar(bytes, 0);
                sb.Append(c);
            }
            string sFileName = sb.ToString();

            //Decompress file content
            bytes = new byte[sizeof(int)];
            zipStream.Read(bytes, 0, sizeof(int));
            int iFileLen = BitConverter.ToInt32(bytes, 0);

            bytes = new byte[iFileLen];
            zipStream.Read(bytes, 0, bytes.Length);

            string sFilePath = Path.Combine(sDir, sFileName);
            string sFinalDir = Path.GetDirectoryName(sFilePath);
            if (!Directory.Exists(sFinalDir))
                Directory.CreateDirectory(sFinalDir);

            using (FileStream outFile = new FileStream(sFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                outFile.Write(bytes, 0, iFileLen);

            return true;
        }

    }
}