using ColossalFramework.UI;
using Klyte.Commons.Utils;
using System;
using UnityEngine;

namespace Klyte.Commons.UI
{

    public class KlyteModsPanel : UICustomControl
    {
        private const int NUM_SERVICES = 0;
        private static KlyteModsPanel m_instance;

        internal UIPanel controlContainer { get; private set; }
        public UIPanel mainPanel { get; private set; }

        private UITabstrip m_StripMain;

        public static KlyteModsPanel instance
        {
            get {
                if (m_instance == null)
                {
                    KlyteUtils.doLog("Creating KlyteModsPanel");
                    UIComponent view = KCController.instance.kcPanelContainer;
                    KlyteUtils.createUIElement(out UIPanel panelObj, view.transform);

                    m_instance = panelObj.gameObject.AddComponent<KlyteModsPanel>();
                }
                return m_instance;
            }
        }

        #region Awake
        private void Awake()
        {
            controlContainer = GetComponent<UIPanel>();
            controlContainer.name = "KCPanel";

            KlyteUtils.createUIElement(out UIPanel _mainPanel, controlContainer.transform, "KCListPanel", new Vector4(395, 58, 875, 550));
            mainPanel = _mainPanel;

            KlyteUtils.createUIElement(out m_StripMain, mainPanel.transform, "KCTabstrip", new Vector4(10, -40, mainPanel.width - 20, 40));

            KlyteUtils.createUIElement(out UITabContainer tabContainer, mainPanel.transform, "KCTabContainer", new Vector4(0, 0, mainPanel.width, mainPanel.height));
            m_StripMain.tabPages = tabContainer;
            m_StripMain.selectedIndex = 0;
            m_StripMain.selectedIndex = -1;
        }

        private void Update()
        {
            controlContainer.absolutePosition = new Vector3(0, 0, 0);
        }

        public void AddTab(ModTab cat, Type customControl, UITextureAtlas atlas, string fgTexture, string tooltip)
        {
            if (m_StripMain.Find<UIComponent>(cat.ToString()) != null)
            {
                return;
            }

            UIButton superTab = CreateTabTemplate();
            superTab.atlas = atlas;
            superTab.normalFgSprite = fgTexture;
            superTab.color = Color.gray;
            superTab.focusedColor = Color.white;
            superTab.hoveredColor = Color.white;
            superTab.disabledColor = Color.black;
            superTab.playAudioEvents = true;
            superTab.tooltip = tooltip;
            superTab.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;

            KlyteUtils.createUIElement(out UIPanel content, null);
            content.name = "Container";
            content.area = new Vector4(0, 0, mainPanel.width, mainPanel.height);

            m_StripMain.AddTab(cat.ToString(), superTab.gameObject, content.gameObject, customControl);
        }

        public void OpenAt(ModTab cat)
        {
            m_StripMain.selectedIndex = m_StripMain.Find<UIComponent>(cat.ToString())?.zOrder ?? -1;
            KCController.instance.OpenKCPanel();
        }

        private static UIButton CreateTabTemplate()
        {
            KlyteUtils.createUIElement(out UIButton tabTemplate, null, "KCTabTemplate");
            KlyteUtils.initButton(tabTemplate, false, "GenericTab");
            tabTemplate.autoSize = false;
            tabTemplate.width = 40;
            tabTemplate.height = 40;
            tabTemplate.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            return tabTemplate;
        }
        #endregion


        private void SetActiveTab(int idx)
        {
            this.m_StripMain.selectedIndex = idx;
        }

    }
    public enum ModTab
    {
        TransportLinesManager,
        ServiceVehiclesManager,
        Addresses
    }
}
