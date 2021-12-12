extern alias UUI;
using ColossalFramework.UI;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using System;
using UnityEngine;
using UUI::UnifiedUI.Helpers;

namespace Klyte.Commons.ModShared
{
    internal class BridgeUUI : IBridgeUUI
    {
        public override bool IsUuiAvailable { get; } = true;

        private UUICustomButton m_modButton;
        private UIPanel m_container;


        private Func<bool> UseLowSaturationButtonFunc;

        public sealed override void RegisterMod<U, C, T>(BasicIUserMod<U, C, T> modInstance)
        {
            UseLowSaturationButtonFunc = () => BasicIUserMod<U, C, T>.UseLowSaturationButton;


            m_modButton = UUIHelpers.RegisterCustomButton(
             name: modInstance.SimpleName,
             groupName: "Klyte45",
             tooltip: modInstance.Name,
             onToggle: (value) => { if (value) { Open(); } else { Close(); } },
             onToolChanged: null,
             icon: KlyteResourceLoader.LoadTexture($"UI.Images.%{modInstance.IconName}.png"),
             hotkeys: new UUIHotKeys { });

            m_container = UIView.Find("TSBar").AttachUIComponent(CreateContainer(modInstance, 875, 550).gameObject) as UIPanel;
            m_container.gameObject.AddComponent<T>();
            Close();
        }

        protected override void TogglePanel(UIComponent component, UIMouseEventParameter eventParam) => throw new System.NotImplementedException();
        internal override void ApplyButtonColor(bool useLowSaturationButton) => m_modButton.Button.color = Color.Lerp(useLowSaturationButton ? Color.gray : Color.white, m_modButton.IsPressed ? Color.white : Color.black, 0.5f);
        internal override void ResetPosition() { }
        internal override void Close()
        {
            m_modButton.IsPressed = false;
            m_container.isVisible = false;
            ApplyButtonColor(UseLowSaturationButtonFunc());
        }

        internal override void Open()
        {
            m_modButton.IsPressed = true;
            m_container.isVisible = true;
            m_container.absolutePosition = new Vector3(25, 50);
            ApplyButtonColor(UseLowSaturationButtonFunc());
        }

        internal override void SelectModTab() => Open();
        internal override void UnselectTab() => Close();
    }
}