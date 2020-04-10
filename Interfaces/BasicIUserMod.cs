using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.UI.SpriteNames;
using Klyte.Commons.Utils;
using UnityEngine;

namespace Klyte.Commons.Interfaces
{
    public abstract class BasicIUserMod<U, C, T> : BasicIUserModSimplified<U, C>
        where U : BasicIUserMod<U, C, T>, new()
        where C : BaseController<U, C>
        where T : BasicKPanel<U, C, T>
    {
        protected virtual float? TabWidth => null;

        private static UIButton m_modPanelButton;
        private static UITabstrip m_modsTabstrip;
        private static UIPanel m_modsPanel;
        private static UIPanel m_bg;

        protected virtual bool LoadUI => true;

        protected sealed override void OnLevelLoadedInherit(LoadMode mode)
        {
            base.OnLevelLoadedInherit(mode);
            if (LoadUI)
            {
                m_modsPanel = UIView.Find<UIPanel>("K45_ModsPanel");
                if (m_modsPanel == null)
                {
                    UIComponent uicomponent = UIView.Find("TSBar");
                    m_bg = uicomponent.AddUIComponent<UIPanel>();
                    m_bg.name = "K45_MB";
                    m_bg.absolutePosition = new Vector2(ButtonPosX.value, ButtonPosY.value);
                    m_bg.width = 40f;
                    m_bg.height = 40f;
                    m_bg.zOrder = 1;
                    UIButton doneButton = m_bg.AddUIComponent<UIButton>();
                    doneButton.normalBgSprite = "GenericPanel";
                    doneButton.width = 100f;
                    doneButton.height = 50f;
                    doneButton.relativePosition = new Vector2(0f, -52);
                    doneButton.text = "Done";
                    doneButton.hoveredTextColor = new Color32(0, byte.MaxValue, byte.MaxValue, 1);
                    doneButton.Hide();
                    doneButton.zOrder = 99;
                    UIDragHandle handle = m_bg.AddUIComponent<UIDragHandle>();
                    handle.name = "K45_DragHandle";
                    handle.relativePosition = Vector2.zero;
                    handle.width = m_bg.width - 5f;
                    handle.height = m_bg.height - 5f;
                    handle.zOrder = 0;
                    handle.target = m_bg;
                    handle.Start();
                    handle.enabled = false;
                    m_bg.zOrder = 9;

                    m_bg.isInteractive = false;
                    handle.zOrder = 10;
                    doneButton.eventClick += (component, ms) =>
                    {
                        doneButton.Hide();
                        handle.zOrder = 10000;
                        handle.enabled = false;
                        ButtonPosX.value = (int)m_bg.absolutePosition.x;
                        ButtonPosY.value = (int)m_bg.absolutePosition.y;
                    };
                    m_bg.color = new Color32(96, 96, 96, byte.MaxValue);
                    m_modPanelButton = m_bg.AddUIComponent<UIButton>();
                    m_modPanelButton.disabledTextColor = new Color32(128, 128, 128, byte.MaxValue);
                    KlyteMonoUtils.InitButton(m_modPanelButton, false, KlyteResourceLoader.GetDefaultSpriteNameFor(CommonsSpriteNames.K45_K45Button), false);
                    m_modPanelButton.relativePosition = new Vector3(10, 4f);
                    m_modPanelButton.size = new Vector2(32, 32);
                    m_modPanelButton.name = "K45_ModsButton";
                    m_modPanelButton.zOrder = 11;
                    m_modPanelButton.textScale = 1.3f;
                    m_modPanelButton.textVerticalAlignment = UIVerticalAlignment.Middle;
                    m_modPanelButton.textHorizontalAlignment = UIHorizontalAlignment.Center;
                    m_modPanelButton.tooltip = "Double click to move the button!";
                    m_modPanelButton.eventDoubleClick += (component, ms) =>
                    {
                        handle.zOrder = 13;
                        doneButton.Show();
                        handle.enabled = true;
                    };

                    m_modsPanel = m_bg.AddUIComponent<UIPanel>();
                    m_modsPanel.name = "K45_ModsPanel";
                    m_modsPanel.size = new Vector2(875, 550);
                    m_modsPanel.relativePosition = new Vector3(0f, 7f);
                    m_modsPanel.isInteractive = false;
                    m_modsPanel.Hide();

                    m_modPanelButton.eventClicked += TogglePanel;

                    KlyteMonoUtils.CreateTabsComponent(out m_modsTabstrip, out UITabContainer container, m_modsPanel.transform, "K45", new Vector4(52, -8, m_modsPanel.width - 52, 40), new Vector4(0, 32, m_modsPanel.width, m_modsPanel.height));
                    m_modsTabstrip.isInteractive = false;
                    container.isInteractive = false;
                }
                else
                {
                    m_modPanelButton = UIView.Find<UIButton>("K45_ModsButton");
                    m_modsTabstrip = UIView.Find<UITabstrip>("K45_Tabstrip");
                    m_modsTabstrip.isInteractive = false;
                    m_modsTabstrip.tabContainer.isInteractive = false;
                }

                AddTab();
            }
        }

        public override void Group9SettingsUI(UIHelperExtension group9)
        {
            base.Group9SettingsUI(group9);
            group9.AddButton("Reset <K> Button position", () =>
            {
                ButtonPosX.value = 5;
                ButtonPosY.value = 60;
                if (m_bg)
                {
                    m_bg.absolutePosition = new Vector3(5, 60);
                }
            });
        }

        internal void AddTab()
        {
            if (m_modsTabstrip.Find<UIComponent>(CommonProperties.Acronym) != null)
            {
                return;
            }

            UIButton superTab = CreateTabTemplate();
            superTab.normalFgSprite = IconName;
            superTab.color = Color.gray;
            superTab.focusedColor = Color.white;
            superTab.hoveredColor = Color.white;
            superTab.disabledColor = Color.black;
            superTab.playAudioEvents = true;
            superTab.tooltip = GeneralName;
            superTab.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;

            KlyteMonoUtils.CreateUIElement(out UIPanel content, null);
            content.name = "Container";
            content.size = new Vector4(TabWidth ?? m_modsPanel.width, m_modsPanel.height);

            m_modsTabstrip.AddTab(CommonProperties.Acronym, superTab.gameObject, content.gameObject, typeof(T));

            content.eventVisibilityChanged += (x, y) => { if (y) { ShowVersionInfoPopup(); } };
        }

        private static UIButton CreateTabTemplate()
        {
            KlyteMonoUtils.CreateUIElement(out UIButton tabTemplate, null, "KCTabTemplate");
            KlyteMonoUtils.InitButton(tabTemplate, false, "GenericTab");
            tabTemplate.autoSize = false;
            tabTemplate.width = 40;
            tabTemplate.height = 40;
            tabTemplate.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            return tabTemplate;
        }

        private void TogglePanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (m_modsPanel == null)
            {
                return;
            }

            m_modsPanel.isVisible = !m_modsPanel.isVisible;
            if (m_modsPanel.isVisible)
            {
                m_modPanelButton?.Focus();
            }
            else
            {
                m_modPanelButton?.Unfocus();
            }
        }

        public void ClosePanel()
        {
            if (m_modsPanel == null)
            {
                return;
            }

            m_modsPanel.isVisible = false;
            m_modPanelButton?.Unfocus();

        }

        public void OpenPanel()
        {
            if (m_modsPanel == null)
            {
                return;
            }

            m_modsPanel.isVisible = true;
            m_modPanelButton?.Focus();
        }

        public void OpenPanelAtModTab()
        {
            OpenPanel();
            m_modsTabstrip.ShowTab(CommonProperties.Acronym);
        }

        public static SavedFloat ButtonPosX { get; } = new SavedFloat("K45_ButtonPosX_v2", Settings.gameSettingsFile, 5, true);
        public static SavedFloat ButtonPosY { get; } = new SavedFloat("K45_ButtonPosY_v2", Settings.gameSettingsFile, 60, true);

        protected override void ExtraUnloadBinds()
        {
            base.ExtraUnloadBinds();
            ClosePanel();
        }
    }

}
