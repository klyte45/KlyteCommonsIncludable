using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.i18n;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Overrides;
using Klyte.Commons.TextureAtlas;
using Klyte.Commons.UI;
using Klyte.Commons.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("1.1.6.*")]
namespace Klyte.Commons
{
    public sealed class KlyteCommonsMod : BasicIUserMod<KlyteCommonsMod, KCLocaleUtils, KCResourceLoader, MonoBehaviour, KCCommonTextureAtlas, UICustomControl>
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

        public override void TopSettingsUI(UIHelperExtension ext)
        {
            LocaleManager.eventLocaleChanged += new LocaleManager.LocaleChangedHandler(autoLoadLocale);
        }

        public override void Group9SettingsUI(UIHelperExtension group9)
        {
            group9.AddDropdownLocalized("KCM_MOD_LANG", KCLocaleUtils.instance.getLanguageIndex(), KCLocaleUtils.currentLanguageId.value, delegate (int idx)
            {
                KCLocaleUtils.currentLanguageId.value = idx;
                LocaleManager.ForceReload();
            });
            group9.AddLabel(Locale.Get("KCM_LANG_NOTICE"));
        }

        public void autoLoadLocale()
        {
            loadLocale(true);
        }

        private UIButton m_openKCPanelButton;
        private UIPanel m_KCPanelContainer;


        internal UIPanel kcPanelContainer
        {
            get {
                if (m_KCPanelContainer == null)
                {
                    UITabstrip toolStrip = ToolsModifierControl.mainToolbar.GetComponentInChildren<UITabstrip>();
                    KlyteUtils.createUIElement(out m_openKCPanelButton, null);
                    m_openKCPanelButton.size = new Vector2(43f, 49f);
                    m_openKCPanelButton.tooltip = "Klyte45's Mods (v" + KlyteCommonsMod.version + ")";
                    m_openKCPanelButton.atlas = KCCommonTextureAtlas.instance.atlas;
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

                    KlyteUtils.createUIElement(out m_KCPanelContainer, null);

                    toolStrip.AddTab("Klyte45Button", m_openKCPanelButton.gameObject, m_KCPanelContainer.gameObject);

                    m_KCPanelContainer.absolutePosition = new Vector3();
                    m_KCPanelContainer.clipChildren = false;
                }

                return m_KCPanelContainer;
            }
        }


        public static void OpenKCPanel()
        {
            if (instance.m_openKCPanelButton.state != UIButton.ButtonState.Focused)
            {
                instance.m_openKCPanelButton.SimulateClick();
            }
        }
        public static void CloseKCPanel()
        {
            if (instance.m_openKCPanelButton.state == UIButton.ButtonState.Focused)
            {
                instance.m_openKCPanelButton.SimulateClick();
            }
        }
    }
}
