using ColossalFramework.UI;
using Klyte.Commons.Utils;
using System;
using UnityEngine;

namespace Klyte.Commons.UI
{

    public class KlyteModsPanel : UICustomControl
    {
        private static KlyteModsPanel m_instance;

        internal UIPanel ControlContainer { get; private set; }
        public UIPanel MainPanel { get; private set; }

        private UITabstrip m_stripMain;

        public static KlyteModsPanel Instance
        {
            get {
                if (m_instance == null)
                {
                    LogUtils.DoLog("Creating KlyteModsPanel");
                    UIComponent view = KlyteCommonsMod.Instance.KcPanelContainer;
                    KlyteUiUtils.CreateUIElement(out UIPanel panelObj, view.transform);

                    m_instance = panelObj.gameObject.AddComponent<KlyteModsPanel>();
                }
                return m_instance;
            }
        }

        #region Awake
        public void Awake()
        {
            ControlContainer = GetComponent<UIPanel>();
            ControlContainer.name = "KCPanel";

            KlyteUiUtils.CreateUIElement(out UIPanel _mainPanel, ControlContainer.transform, "KCListPanel", new Vector4(395, 58, 875, 550));
            MainPanel = _mainPanel;

            KlyteUiUtils.CreateUIElement(out m_stripMain, MainPanel.transform, "KCTabstrip", new Vector4(10, -40, MainPanel.width - 20, 40));

            KlyteUiUtils.CreateUIElement(out UITabContainer tabContainer, MainPanel.transform, "KCTabContainer", new Vector4(0, 0, MainPanel.width, MainPanel.height));
            m_stripMain.tabPages = tabContainer;
            m_stripMain.selectedIndex = 0;
            m_stripMain.selectedIndex = -1;
        }

        public void Update()
        {
            ControlContainer.absolutePosition = new Vector3(0, 0, 0);
        }

        internal void AddTab(ModTab cat, Type customControl, UITextureAtlas atlas, string fgTexture, string tooltip, PropertyChangedEventHandler<bool> onVisibilityChanged, float? width = null)
        {
            if (m_stripMain.Find<UIComponent>(cat.ToString()) != null)
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

            KlyteUiUtils.CreateUIElement(out UIPanel content, null);
            content.name = "Container";
            content.area = new Vector4(0, 0, width ?? MainPanel.width, MainPanel.height);

            m_stripMain.AddTab(cat.ToString(), superTab.gameObject, content.gameObject, customControl);

            content.eventVisibilityChanged += onVisibilityChanged;
        }

        public void OpenAt(ModTab cat)
        {
            m_stripMain.selectedIndex = m_stripMain.Find<UIComponent>(cat.ToString())?.zOrder ?? -1;
            KlyteCommonsMod.OpenKCPanel();
        }

        private static UIButton CreateTabTemplate()
        {
            KlyteUiUtils.CreateUIElement(out UIButton tabTemplate, null, "KCTabTemplate");
            KlyteUiUtils.InitButton(tabTemplate, false, "GenericTab");
            tabTemplate.autoSize = false;
            tabTemplate.width = 40;
            tabTemplate.height = 40;
            tabTemplate.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            return tabTemplate;
        }
        #endregion


    }
    public enum ModTab
    {
        KlyteCommons,
        TransportLinesManager,
        ServiceVehiclesManager,
        Addresses,
        VehicleWealthizer,
        TouchThis,
        DynamicTextBoards,
        SuburbStyler
    }
}
