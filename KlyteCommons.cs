using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.i18n;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Overrides;
using Klyte.Commons.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("1.1.6.9999")]
namespace Klyte.Commons
{
    public class KlyteCommonsMod : BasicIUserMod<KlyteCommonsMod, KCLocaleUtils, KCResourceLoader>
    {
        public override string SimpleName => "Klyte Commons";
        public override string Description => "Base mod for Klyte45 mods. Required dependency";


        public override void doErrorLog(string fmt, params object[] args)
        {
            KlyteUtils.doErrorLog(fmt, args);
        }

        public override void doLog(string fmt, params object[] args)
        {
            KlyteUtils.doLog(fmt, args);
        }

        public override void LoadSettings()
        {

        }

        public KlyteCommonsMod()
        {
            Construct();
        }

        public override void OnCreated(ILoading loading)
        {
            GameObject gameObject = new GameObject
            {
                name = "KlyteCommons"
            };
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario)
            {
                return;
            }
            KCController.Ensure();
        }

        public override void OnLevelUnloading()
        {
        }

        public override void OnReleased()
        {
        }

        public override void TopSettingsUI(UIHelperExtension ext)
        {

        }
    }
}
