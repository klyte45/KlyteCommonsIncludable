using ICities;
using Klyte.Commons.Extensors;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("1.1.4.0")]
namespace Klyte.Commons
{
    public class KlyteCommonsMod : IUserMod, ILoadingExtension
    {

        public string Name => "Klyte Commons " + KlyteCommonsMod.version;
        public string Description => "Base mod for Klyte45 mods. Required dependency";

        public void OnCreated(ILoading loading) { }

        public void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario)
            {
                return;
            }
            KCController.Ensure();


            GameObject gameObject = new GameObject
            {
                name = "KlyteCommons"
            };

            var typeTarg = typeof(Redirector<>);
            var instances = from t in Assembly.GetAssembly(typeof(KlyteCommonsMod)).GetTypes()
                            let y = t.BaseType
                            where t.IsClass && !t.IsAbstract && y != null && y.IsGenericType && y.GetGenericTypeDefinition() == typeTarg
                            select t;

            foreach (Type t in instances)
            {
                gameObject.AddComponent(t);
            }

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
