using ColossalFramework;
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
    public sealed class KlyteCommonsMod : BasicIUserMod<KlyteCommonsMod, KCLocaleUtils, KCResourceLoader>
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
            LocaleManager.eventLocaleChanged += new LocaleManager.LocaleChangedHandler(autoLoadLocale);
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

        public override void Group9SettingsUI(UIHelperExtension group9)
        {
            group9.AddDropdownLocalized("KCM_MOD_LANG", KCLocaleUtils.instance.getLanguageIndex(), KCLocaleUtils.currentLanguageId.value, delegate (int idx)
            {
                KCLocaleUtils.currentLanguageId.value = idx;
                loadLocale(true);
            });
            group9.AddLabel(Locale.Get("KCM_LANG_NOTICE"));
        }

        public void autoLoadLocale()
        {
            loadLocale(false);
        }
    }
}
