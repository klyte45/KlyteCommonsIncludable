using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.Overrides;
using Klyte.Commons.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("1.1.6.0")]
namespace Klyte.Commons
{
    public class KlyteCommonsMod : IUserMod, ILoadingExtension
    {
        public string Name => "Klyte Commons " + KlyteCommonsMod.version;
        public string Description => "Base mod for Klyte45 mods. Required dependency";

        public void OnCreated(ILoading loading)
        {
            GameObject gameObject = new GameObject
            {
                name = "KlyteCommons"
            };
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario)
            {
                return;
            }
            KCController.Ensure();
        }

        public void OnLevelUnloading()
        {
        }

        public void OnReleased()
        {
        }

        public static string minorVersion
        {
            get {
                return majorVersion + "." + typeof(KlyteCommonsMod).Assembly.GetName().Version.Build;
            }
        }
        public static string majorVersion
        {
            get {
                return typeof(KlyteCommonsMod).Assembly.GetName().Version.Major + "." + typeof(KlyteCommonsMod).Assembly.GetName().Version.Minor;
            }
        }
        public static string fullVersion
        {
            get {
                return minorVersion + " r" + typeof(KlyteCommonsMod).Assembly.GetName().Version.Revision;
            }
        }
        public static string version
        {
            get {
                if (typeof(KlyteCommonsMod).Assembly.GetName().Version.Minor == 0 && typeof(KlyteCommonsMod).Assembly.GetName().Version.Build == 0)
                {
                    return typeof(KlyteCommonsMod).Assembly.GetName().Version.Major.ToString();
                }
                if (typeof(KlyteCommonsMod).Assembly.GetName().Version.Build > 0)
                {
                    return minorVersion;
                }
                else
                {
                    return majorVersion;
                }
            }
        }
    }
}
