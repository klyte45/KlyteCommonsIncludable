using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.i18n;
using Klyte.Commons.Interfaces;
using Klyte.Commons.TextureAtlas;
using Klyte.Commons.UI;
using Klyte.Commons.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static Klyte.Commons.TextureAtlas.KCCommonTextureAtlas;

[assembly: AssemblyVersion("0.0.0.*")]
namespace Klyte.Commons
{
    public sealed class KlyteCommonsMod : BasicIUserMod<KlyteCommonsMod, KCResourceLoader, MonoBehaviour, KCCommonTextureAtlas, UICustomControl, SpriteNames>
    {
        public override string SimpleName => "Klyte Commons REWORK";
        public override string Description => "Base mod for Klyte45 mods. Required dependency";

        public override void DoErrorLog(string fmt, params object[] args)
        {
            LogUtils.DoErrorLog(fmt, args);
        }

        public override void DoLog(string fmt, params object[] args)
        {
            LogUtils.DoLog(fmt, args);
        }

        public override void LoadSettings()
        {

        }

        public KlyteCommonsMod()
        {
            Construct();

        }

        public override void TopSettingsUI(UIHelperExtension ext)
        {
            LocaleManager.eventLocaleChanged += () =>
            {
                var localeManager = GameObject.FindObjectOfType<KlyteLocaleManager>();
                if (localeManager != null)
                {
                    localeManager.ReloadLanguage();
                }
            };
        }

        public override void Group9SettingsUI(UIHelperExtension group9)
        {
            var localeManager = GameObject.FindObjectOfType<KlyteLocaleManager>();
            var dd = group9.AddDropdownLocalized("K45_MOD_LANG", (new string[] { "K45_GAME_DEFAULT_LANGUAGE" }.Concat(KlyteLocaleManager.locales.Select(x => $"K45_LANG_{x}")).Select(x => Locale.Get(x))).ToArray(), localeManager.LoadedLanguageIdx, delegate (int idx)
             {
                 localeManager = GameObject.FindObjectOfType<KlyteLocaleManager>();
                 if (localeManager != null)
                 {
                     localeManager.LoadedLanguageIdx = idx;
                 }
             });
            LocaleManager.eventLocaleChanged += () => dd.items = new string[] { "K45_GAME_DEFAULT_LANGUAGE" }.Concat(KlyteLocaleManager.locales.Select(x => $"K45_LANG_{x}")).Select(x => Locale.Get(x)).ToArray();
            group9.AddLabel(Locale.Get("K45_LANG_NOTICE"));
        }


        private UIButton m_openKCPanelButton;
        private UIPanel m_kCPanelContainer;


        internal UIPanel KcPanelContainer
        {
            get {
                if (m_kCPanelContainer == null)
                {
                    UITabstrip toolStrip = ToolsModifierControl.mainToolbar.GetComponentInChildren<UITabstrip>();
                    KlyteMonoUtils.CreateUIElement(out m_openKCPanelButton, null);
                    m_openKCPanelButton.size = new Vector2(43f, 49f);
                    m_openKCPanelButton.tooltip = "Klyte45's Mods (v" + KlyteCommonsMod.Version + ")";
                    m_openKCPanelButton.atlas = KCCommonTextureAtlas.instance.Atlas;
                    m_openKCPanelButton.focusedColor = new Color32(128, 183, 240, 255);
                    m_openKCPanelButton.hoveredColor = new Color32(128, 240, 183, 255);
                    m_openKCPanelButton.disabledColor = new Color32(0, 0, 0, 255);
                    m_openKCPanelButton.normalBgSprite = "KlyteMenuIcon";
                    m_openKCPanelButton.focusedBgSprite = "KlyteMenuIcon";
                    m_openKCPanelButton.hoveredBgSprite = "KlyteMenuIcon";
                    m_openKCPanelButton.pressedBgSprite = "KlyteMenuIcon";
                    m_openKCPanelButton.disabledBgSprite = "KlyteMenuIcon";
                    m_openKCPanelButton.focusedFgSprite = "ToolbarIconGroup6Focused";
                    m_openKCPanelButton.hoveredFgSprite = "ToolbarIconGroup6Hovered";
                    m_openKCPanelButton.normalFgSprite = "";
                    m_openKCPanelButton.pressedFgSprite = "ToolbarIconGroup6Pressed";
                    m_openKCPanelButton.disabledFgSprite = "";
                    m_openKCPanelButton.playAudioEvents = true;
                    m_openKCPanelButton.tabStrip = true;

                    KlyteMonoUtils.CreateUIElement(out m_kCPanelContainer, null);

                    toolStrip.AddTab("Klyte45Button", m_openKCPanelButton.gameObject, m_kCPanelContainer.gameObject);

                    m_kCPanelContainer.absolutePosition = new Vector3();
                    m_kCPanelContainer.clipChildren = false;
                }

                return m_kCPanelContainer;
            }
        }


        public static void OpenKCPanel()
        {
            if (Instance.m_openKCPanelButton.state != UIButton.ButtonState.Focused)
            {
                Instance.m_openKCPanelButton.SimulateClick();
            }
        }
        public static void CloseKCPanel()
        {
            if (Instance.m_openKCPanelButton.state == UIButton.ButtonState.Focused)
            {
                Instance.m_openKCPanelButton.SimulateClick();
            }
        }
    }
}
