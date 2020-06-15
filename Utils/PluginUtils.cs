using ColossalFramework;
using ColossalFramework.Plugins;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public static class PluginUtils
    {

        public static Dictionary<ulong, string> VerifyModsEnabled(List<ulong> modIds, List<string> modsDlls) => Singleton<PluginManager>.instance.GetPluginsInfo().Where((PluginManager.PluginInfo pi) =>
            pi.assemblyCount > 0
            && pi.isEnabled
            && (
                (modIds?.Contains(pi.publishedFileID.AsUInt64) ?? false)
             || (modsDlls != null && pi.GetAssemblies().Where(x => modsDlls.Contains(x.GetName().Name)).Count() > 0)
            )
        ).ToDictionary(x => x.publishedFileID.AsUInt64, x => ((IUserMod)x.userModInstance).Name);


        public static I GetImplementationTypeForMod<O, F, I>(GameObject objTarget, string dllName, string dllMinVersion) where O : MonoBehaviour, I where F : MonoBehaviour, I
        {
            if (Singleton<PluginManager>.instance.GetPluginsInfo().Where((PluginManager.PluginInfo pi) =>
            pi.assemblyCount > 0
            && pi.isEnabled
            && (pi.GetAssemblies().Where(x => (x.GetName().Name == dllName) && x.GetName().Version.CompareTo(new Version(dllMinVersion)) >= 0).Count() > 0)

        ).Count() > 0)
            {
                return objTarget.AddComponent<O>();
            }
            else
            {
                return objTarget.AddComponent<F>();
            }
        }
    }
}
