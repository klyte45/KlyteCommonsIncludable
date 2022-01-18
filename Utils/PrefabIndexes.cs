using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Packaging;
using ColossalFramework.PlatformServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Klyte.Commons.Utils
{
    public interface IPrefabIndexes
    {
        Dictionary<string, IIndexedPrefabData> PrefabsData { get; }
    }
    public abstract class PrefabIndexesAbstract<T, I> : Singleton<I>, IPrefabIndexes where T : PrefabInfo where I : PrefabIndexesAbstract<T, I>
    {
        private Dictionary<string, string> m_authorList;

        public Dictionary<string, string> AuthorList
        {
            get
            {
                if (m_authorList == null)
                {
                    m_authorList = LoadAuthors();
                }
                return m_authorList;
            }
        }
        private Dictionary<string, T> m_propsLoaded;
        [Obsolete("Use the list with full details")]
        public Dictionary<string, T> PrefabsLoaded
        {
            get
            {
                if (m_propsLoaded == null)
                {
                    m_propsLoaded = GetInfos().Where(x => x?.name != null).GroupBy(x => GetListName(x)).Select(x => Tuple.New(x.Key, x.FirstOrDefault())).ToDictionary(x => x.First, x => x.Second);
                }
                return m_propsLoaded;
            }
        }

        private Dictionary<string, IIndexedPrefabData> m_prefabsData;
        public Dictionary<string, IIndexedPrefabData> PrefabsData
        {
            get
            {
                if (m_prefabsData is null)
                {
                    var prefabMapping = PackageManager.FilterAssets(new Package.AssetType[] { UserAssetType.CustomAssetMetaData }).Select(x => Tuple.New(x.fullName, x)).ToDictionary(x => x.First, x => x.Second);
                    m_prefabsData = GetInfos().Where(x => x?.name != null).Select(x => new IndexedPrefabData<T>(prefabMapping.TryGetValue(x.name, out Package.Asset asset) ? asset : null, x)).ToDictionary(x => x.PrefabName, x => (IIndexedPrefabData)x);
                }
                return m_prefabsData;
            }
        }

        public static string GetListName(T x) => x?.GetUncheckedLocalizedTitle();

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
            while (num < (ulong)PrefabCollection<T>.PrefabCount())
            {
                T prefabInfo = PrefabCollection<T>.GetPrefab(num);
                if (prefabInfo != null)
                {
                    list.Add(prefabInfo);
                }
                num += 1u;
            }
            return list;
        }

        public IEnumerator BasicInputFiltering(string input, Wrapper<string[]> result)
        {
            yield return result.Value = PrefabsData.Values
              .Where((x) => input.IsNullOrWhiteSpace() ? true : LocaleManager.cultureInfo.CompareInfo.IndexOf($"{x.DisplayName}\n{x.Author?.personaName}\n{x.PrefabName}", input, CompareOptions.IgnoreCase) >= 0)
              .Select(x => x.DisplayName)
              .OrderBy((x) => x)
              .ToArray();
        }

        public IEnumerator BasicInputFilteringDetailed(string input, Wrapper<IIndexedPrefabData[]> result)
        {
            yield return result.Value = PrefabsData.Values
              .Where((x) => input.IsNullOrWhiteSpace() ? true : LocaleManager.cultureInfo.CompareInfo.IndexOf($"{x.DisplayName}\n{x.Author?.personaName}\n{x.PrefabName}", input, CompareOptions.IgnoreCase) >= 0)
              .ToArray();
        }



    }

    public interface IIndexedPrefabData
    {
        string DisplayName { get; }
        string PrefabName { get; }
        Friend Author { get; }
        ulong WorkshopId { get; }
        Type GetPrefabType();
    }
    public class IndexedPrefabData<T> : IIndexedPrefabData where T : PrefabInfo
    {
        public IndexedPrefabData(Package.Asset src, T prefab)
        {
            if (prefab is null)
            {
                return;
            }

            Prefab = prefab;
            PrefabName = prefab.name;
            DisplayName = prefab.GetUncheckedLocalizedTitle();
            if (!(src is null))
            {
                WorkshopId = src.package.GetPublishedFileID().AsUInt64;
                if (ulong.TryParse(src.package.packageAuthor.Substring("steamid:".Length), out ulong authorID))
                {
                    Author = new Friend(new UserID(authorID));
                }
            }
            else
            {
                WorkshopId = ~0ul;
            }
        }

        public string DisplayName { get; private set; }
        public string PrefabName { get; private set; }
        public Friend Author { get; private set; }
        public ulong WorkshopId { get; private set; }

        public T Prefab { get; private set; }

        public Type GetPrefabType() => Prefab.GetType();
    }

    public class PropIndexes : PrefabIndexesAbstract<PropInfo, PropIndexes> { }
    public class NetIndexes : PrefabIndexesAbstract<NetInfo, NetIndexes> { }
    public class TransportIndexes : PrefabIndexesAbstract<TransportInfo, TransportIndexes> { }
    public class BuildingIndexes : PrefabIndexesAbstract<BuildingInfo, BuildingIndexes> { }
    public class VehiclesIndexes : PrefabIndexesAbstract<VehicleInfo, VehiclesIndexes> { }
    public class TreeIndexes : PrefabIndexesAbstract<TreeInfo, TreeIndexes> { }
}

