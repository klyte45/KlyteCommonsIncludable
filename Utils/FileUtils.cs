using ColossalFramework.IO;
using ColossalFramework.Packaging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Klyte.Commons.Utils
{
    public class FileUtils
    {
        #region File & Prefab Utils
        public static readonly string BASE_FOLDER_PATH = DataLocation.localApplicationData + Path.DirectorySeparatorChar + "Klyte45Mods" + Path.DirectorySeparatorChar;

        public static FileInfo EnsureFolderCreation(string folderName)
        {
            if (File.Exists(folderName) && (File.GetAttributes(folderName) & FileAttributes.Directory) != FileAttributes.Directory)
            {
                File.Delete(folderName);
            }
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            return new FileInfo(folderName);
        }
        public static bool IsFileCreated(string fileName) => File.Exists(fileName);

        public static void ScanPrefabsFolders(string filenameToSearch, Action<FileStream> action)
        {
            List<string> list = new List<string>();
            ForEachLoadedPrefab<BuildingInfo>((loaded) =>
            {
                Package.Asset asset = PackageManager.FindAssetByName(loaded.name);
                if (!(asset == null) && !(asset.package == null))
                {
                    string packagePath = asset.package.packagePath;
                    if (packagePath != null)
                    {
                        string filePath = Path.Combine(Path.GetDirectoryName(packagePath), filenameToSearch);
                        if (!list.Contains(filePath))
                        {
                            list.Add(filePath);
                            if (File.Exists(filePath))
                            {
                                using FileStream stream = File.OpenRead(filePath);
                                action(stream);
                            }
                        }
                    }
                }
            });
        }

        public static void ForEachLoadedPrefab<PI>(Action<PI> action) where PI : PrefabInfo
        {
            for (uint i = 0; i < PrefabCollection<PI>.LoadedCount(); i++)
            {
                PI loaded = PrefabCollection<PI>.GetLoaded(i);
                if (!(loaded == null))
                {
                    action(loaded);
                }
            }
        }
        #endregion
    }
}
