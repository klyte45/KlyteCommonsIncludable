using ColossalFramework.Packaging;
using System.Linq;
using static ColossalFramework.Packaging.Package;

namespace Klyte.Commons.Utils
{
    public static class PrefabUtils
    {
        public static Asset GetAssetFromPrefab<T>(T info) where T : PrefabInfo => PackageManager.allPackages.SelectMany(x => x.Cast<Asset>().Where(y => y.name == info.name || (info.name.StartsWith(x.packageName) && info.name.EndsWith(y.name)))).FirstOrDefault();
    }
}

