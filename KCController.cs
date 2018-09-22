using ColossalFramework;
using ColossalFramework.UI;
using Klyte.Commons.Extensors;
using Klyte.Commons.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Klyte.Commons
{
    public class KCController : Singleton<KCController>
    {

        private static UITextureAtlas _taKC = null;
        public static UITextureAtlas taKC
        {
            get {
                if (_taKC == null)
                {
                    _taKC = KCResourceLoader.instance.CreateTextureAtlas("UI.Images.sprites.png", "KlyteCommonsSprites", GameObject.FindObjectOfType<UIView>().FindUIComponent<UIPanel>("InfoPanel").atlas.material, 43, 49, new string[] {
                     "ToolbarIconGroup6Hovered",    "ToolbarIconGroup6Focused",   "ToolbarIconGroup6Pressed",    "KlyteMenuIcon"
                    });
                }
                return _taKC;
            }
        }

        public void Awake()
        {
            if (gameObject != null)
            {
                var typeTarg = typeof(Redirector<>);
                var instances = from t in Assembly.GetAssembly(typeof(KCController)).GetTypes()
                                let y = t.BaseType
                                where t.IsClass && !t.IsAbstract && y != null && y.IsGenericType && y.GetGenericTypeDefinition() == typeTarg
                                select t;

                foreach (Type t in instances)
                {
                    gameObject.AddComponent(t);
                }
            }
        }

        //------------------------------------
        private UIButton m_openKCPanelButton;
        private UIPanel m_KCPanelContainer;

        public void Destroy()
        {
            Destroy(m_openKCPanelButton);
            Destroy(m_KCPanelContainer);
        }

        internal UIPanel kcPanelContainer
        {
            get {
                if (m_KCPanelContainer == null)
                {
                    UITabstrip toolStrip = ToolsModifierControl.mainToolbar.GetComponentInChildren<UITabstrip>();
                    KlyteUtils.createUIElement(out m_openKCPanelButton, null);
                    m_openKCPanelButton.size = new Vector2(43f, 49f);
                    m_openKCPanelButton.tooltip = "Klyte45's Mods (v" + KlyteCommonsMod.version + ")";
                    m_openKCPanelButton.atlas = taKC;
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

        public void OpenKCPanel()
        {
            if (m_openKCPanelButton.state != UIButton.ButtonState.Focused)
            {
                m_openKCPanelButton.SimulateClick();
            }
        }
        public void CloseKCPanel()
        {
            if (m_openKCPanelButton.state == UIButton.ButtonState.Focused)
            {
                m_openKCPanelButton.SimulateClick();
            }
        }


    }


}