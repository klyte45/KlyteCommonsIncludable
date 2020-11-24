using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Packaging;
using ColossalFramework.PlatformServices;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Klyte.Commons.Utils
{
    public abstract class PrefabIndexesAbstract<T, I> : Singleton<I> where T : PrefabInfo where I : PrefabIndexesAbstract<T, I>
    {
        private Dictionary<string, string> m_authorList;

        public Dictionary<string, string> AuthorList
        {
            get {
                if (m_authorList == null)
                {
                    m_authorList = LoadAuthors();
                }
                return m_authorList;
            }
        }
        private Dictionary<string, T> m_propsLoaded;
        public Dictionary<string, T> PrefabsLoaded
        {
            get {
                if (m_propsLoaded == null)
                {
                    m_propsLoaded = GetInfos().Where(x => x?.name != null).GroupBy(x => GetListName(x)).Select(x => Tuple.New(x.Key, x.FirstOrDefault())).ToDictionary(x => x.First, x => x.Second);
                }
                return m_propsLoaded;
            }
        }
        public static string GetListName(T x) => (x?.name?.EndsWith("_Data") ?? false) ? $"{x?.GetLocalizedTitle()}" : x?.name ?? "";

        private Dictionary<string, string> LoadAuthors()
        {
            var authors = new Dictionary<string, string>();
            foreach (Package.Asset current in PackageManager.FilterAssets(new Package.AssetType[] { UserAssetType.CustomAssetMetaData }))
            {
                PublishedFileId id = current.package.GetPublishedFileID();
                string publishedFileId = string.Concat(id.AsUInt64);
                if (!authors.ContainsKey(publishedFileId) && !current.package.packageAuthor.IsNullOrWhiteSpace() && current.isEnabled)
                {
                    if (ulong.TryParse(current.package.packageAuthor.Substring("steamid:".Length), out ulong authorID))
                    {
                        string author = new Friend(new UserID(authorID)).personaName;
                        authors.Add(publishedFileId, author);
                    }
                }
            }
            return authors;
        }

        private List<T> GetInfos()
        {
            var list = new List<T>();
            uint num = 0u;
            while (num < (ulong)PrefabCollection<T>.LoadedCount())
            {
                T prefabInfo = PrefabCollection<T>.GetLoaded(num);
                if (prefabInfo != null)
                {
                    list.Add(prefabInfo);
                }
                num += 1u;
            }
            return list;
        }

        public string[] BasicInputFiltering(string input) => PrefabsLoaded
            .ToList()
            .Where((x) => input.IsNullOrWhiteSpace() ? true : LocaleManager.cultureInfo.CompareInfo.IndexOf(x.Value + (AuthorList.TryGetValue(x.Value?.name.Split('.')[0], out string author) ? "\n" + author : ""), input, CompareOptions.IgnoreCase) >= 0)
            .Select(x => x.Key)
            .OrderBy((x) => x)
            .ToArray();
    }

    public class PropIndexes : PrefabIndexesAbstract<PropInfo, PropIndexes> { }
    public class BuildingIndexes : PrefabIndexesAbstract<BuildingInfo, BuildingIndexes> { }
    public class VehiclesIndexes : PrefabIndexesAbstract<VehicleInfo, VehiclesIndexes> { }
}

