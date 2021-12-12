using ColossalFramework.UI;
using Klyte.Commons.Interfaces;
using Klyte.Commons.UI.SpriteNames;
using Klyte.Commons.Utils;
using UnityEngine;

namespace Klyte.Commons.ModShared
{
    internal class BridgeUUIFallback : IBridgeUUI
    {
        private UITabstrip m_modsTabstrip;
        private UIPanel m_bg;
        private UIPanel m_modsPanel;
        private UIButton m_modPanelButton;
        public override bool IsUuiAvailable { get; } = false;
        public override void RegisterMod<U, C, T>(BasicIUserMod<U, C, T> modInstance) => AddModButton<U, C, T>(modInstance);

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

        internal override void ApplyButtonColor(bool useLowSaturationButton)
        {
            m_modPanelButton.color = useLowSaturationButton ? Color.gray : Color.white;
            m_modPanelButton.hoveredColor = Color.white;
            m_modPanelButton.focusedColor = useLowSaturationButton ? Color.gray : Color.white;
            m_modPanelButton.disabledColor = useLowSaturationButton ? Color.gray : Color.white;
        }
        private void AddModButton<U, C, T>(BasicIUserMod<U, C, T> modInstance)
          where U : BasicIUserMod<U, C, T>, new()
          where C : BaseController<U, C>
          where T : BasicKPanel<U, C, T>
        {

            m_modsPanel = UIView.Find<UIPanel>("K45_ModsPanel");
            if (m_modsPanel is null)
            {
                UIComponent uicomponent = UIView.Find("TSBar");
                m_bg = uicomponent.AddUIComponent<UIPanel>();
                m_bg.name = "K45_MB";
                m_bg.absolutePosition = new Vector2(BasicIUserMod<U, C, T>.ButtonPosX.value, BasicIUserMod<U, C, T>.ButtonPosY.value);
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
                    BasicIUserMod<U, C, T>.ButtonPosX.value = (int)m_bg.absolutePosition.x;
                    BasicIUserMod<U, C, T>.ButtonPosY.value = (int)m_bg.absolutePosition.y;
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
                m_modPanelButton.tooltip = "Ctrl + click to move the button!";
                m_modPanelButton.eventClicked += (component, ms) =>
                {
                    if (Event.current.control)
                    {
                        handle.zOrder = 13;
                        doneButton.Show();
                        handle.enabled = true;
                    }
                    else
                    {
                        TogglePanel(component, ms);
                    }
                };

                m_modsPanel = m_bg.AddUIComponent<UIPanel>();
                m_modsPanel.name = "K45_ModsPanel";
                m_modsPanel.size = new Vector2(875, 550);
                m_modsPanel.relativePosition = new Vector3(0f, 7f);
                m_modsPanel.isInteractive = false;
                m_modsPanel.Hide();


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

            if (m_modPanelButton.color != (BasicIUserMod<U, C, T>.UseLowSaturationButton ? Color.gray : Color.white))
            {
                ApplyButtonColor(BasicIUserMod<U, C, T>.UseLowSaturationButton);
            }

            AddTab(modInstance);
        }
        protected void AddTab<U, C, T>(BasicIUserMod<U, C, T> modInstance)
            where U : BasicIUserMod<U, C, T>, new()
            where C : BaseController<U, C>
            where T : BasicKPanel<U, C, T>
        {
            if (!(m_modsTabstrip.Find<UIComponent>(CommonProperties.Acronym) is null))
            {
                return;
            }

            UIButton superTab = CreateTabTemplate();
            superTab.normalFgSprite = modInstance.IconName;
            superTab.color = Color.gray;
            superTab.focusedColor = Color.white;
            superTab.hoveredColor = Color.white;
            superTab.disabledColor = Color.black;
            superTab.playAudioEvents = true;
            superTab.tooltip = modInstance.GeneralName;
            superTab.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;

            UIPanel content = CreateContainer(modInstance, modInstance.TabWidth ?? m_modsPanel.width, m_modsPanel.height);

            m_modsTabstrip.AddTab(CommonProperties.Acronym, superTab.gameObject, content.gameObject, typeof(T));
        }

        internal override void ResetPosition() => m_bg.absolutePosition = new Vector3(5, 60);
        internal override void Close()
        {
            if (m_modsPanel == null)
            {
                return;
            }

            m_modsPanel.isVisible = false;
            m_modPanelButton?.Unfocus();
        }

        internal override void Open()
        {
            if (m_modsPanel == null)
            {
                return;
            }

            m_modsPanel.isVisible = true;
            m_modPanelButton?.Focus();
        }

        internal override void SelectModTab() => m_modsTabstrip.selectedIndex = m_modsTabstrip.tabs.FindIndex(x => x.name == CommonProperties.Acronym);
        internal override void UnselectTab() => m_modsTabstrip.selectedIndex = -1;
    }
}