using ColossalFramework.Packaging;
using HarmonyLib;
using Klyte.Commons.Extensions;
using Klyte.Commons.Utils;
using System.IO;
using System.Linq;
using static Klyte.Commons.Extensions.Patcher;

namespace Klyte.Commons.Redirectors
{
    public class UIWorkshopAssetRedirector : Patcher, IPatcher
    {

        [HarmonyPatch(typeof(WorkshopAssetUploadPanel), "PrepareStagingArea")]
        [HarmonyPostfix]
        public static void AfterPrepareStagingArea(WorkshopAssetUploadPanel __instance)
        {
            if (CommonProperties.AssetExtraFileNames?.Length > 0 || CommonProperties.AssetExtraDirectoryNames?.Length > 0)
			{
                var m_ContentPath = __instance.GetType().GetField("m_ContentPath", Patcher.allFlags).GetValue(__instance) as string;
                var m_TargetAsset = __instance.GetType().GetField("m_TargetAsset", Patcher.allFlags).GetValue(__instance) as Package.Asset;
                var rootAssetFolder = Path.GetDirectoryName(m_TargetAsset.package.packagePath);
                LogUtils.DoErrorLog($"rootAssetFolder2: {rootAssetFolder}; ");
                bool bundledAnyFile = false;
                if (!(CommonProperties.AssetExtraFileNames is null))
                {
                    foreach (string filename in CommonProperties.AssetExtraFileNames)
                    {
                        var targetFilename = Path.Combine(rootAssetFolder, filename);
                        if (File.Exists(targetFilename))
                        {
                            File.Copy(targetFilename, Path.Combine(m_ContentPath, filename));
                            bundledAnyFile = true;
                        }
                    }
                }
                if (!(CommonProperties.AssetExtraDirectoryNames is null))
                {
                    foreach (string directory in CommonProperties.AssetExtraDirectoryNames)
                    {
                        var targetFolder = Path.Combine(rootAssetFolder, directory);
                        if (Directory.Exists(targetFolder))
                        {
                            WorkshopHelper.DirectoryCopy(targetFolder, Path.Combine(m_ContentPath, directory), true);
                            bundledAnyFile = true;
                        }
                    }
                }

                if (bundledAnyFile)
                {
                    var tagsField = __instance.GetType().GetField("m_Tags", Patcher.allFlags);
                    tagsField.SetValue(__instance, (tagsField.GetValue(__instance) as string[]).Concat(new string[] { CommonProperties.ModName, $"K45 {CommonProperties.Acronym}" }).Distinct().ToArray());
                }
			}
        }
    }
}