using ColossalFramework.UI;
using Klyte.Commons.Utils;
using UnityEngine;

namespace Klyte.Commons.Interfaces
{
    public abstract class BasicKPanel<U, C, T> : UICustomControl
        where U : BasicIUserMod<U, C, T>, new()
        where C : BaseController<U, C>
        where T : BasicKPanel<U, C, T>
    {
        protected UIPanel m_controlContainer;

        public static T Instance { get; private set; }
        public UIPanel MainPanel { get; private set; }

        public virtual float PanelWidth { get; } = 825;
        public virtual float PanelHeight { get; } = 500;

        #region Awake
        public void Awake()
        {
            Instance = this as T;
            m_controlContainer = GetComponent<UIPanel>();
            m_controlContainer.area = new Vector4(0, 0, 0, 0);
            m_controlContainer.isVisible = false;
            m_controlContainer.isInteractive = false;
            m_controlContainer.name = $"{CommonProperties.Acronym}PanelParent";

            KlyteMonoUtils.CreateUIElement(out UIPanel _mainPanel, m_controlContainer.transform, $"{CommonProperties.Acronym}Panel", new Vector4(0, 0, PanelWidth, PanelHeight));
            MainPanel = _mainPanel;
            if (PanelWidth + PanelHeight > 0)
            {
                MainPanel.minimumSize = new Vector2(220, 50);
                MainPanel.backgroundSprite = "MenuPanel2";

                CreateTitleBar();
            }
            AwakeActions();
        }

        protected abstract void AwakeActions();

        private void CreateTitleBar()
        {
            KlyteMonoUtils.CreateUIElement(out UILabel titlebar, MainPanel.transform, $"{CommonProperties.Acronym}Title", new Vector4(0, 0, MainPanel.width - 150, 20));
            titlebar.position = default;
            titlebar.autoSize = false;
            titlebar.text = $"{BasicIUserMod<U, C, T>.Instance.SimpleName} v{BasicIUserMod<U, C, T>.FullVersion}";
            titlebar.textAlignment = UIHorizontalAlignment.Center;
            titlebar.relativePosition = new Vector3(75, 13);

            KlyteMonoUtils.CreateUIElement(out UIButton closeButton, MainPanel.transform, "CloseButton", new Vector4(MainPanel.width - 37, 5, 32, 32));
            KlyteMonoUtils.InitButton(closeButton, false, "buttonclose", true);
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.eventClick += (x, y) =>
            {
                BasicIUserMod<U, C, T>.Instance.ClosePanel();
            };

            KlyteMonoUtils.CreateUIElement(out UISprite logo, MainPanel.transform, $"{CommonProperties.Acronym}Logo", new Vector4(22, 5f, 32, 32));
            logo.spriteName = BasicIUserMod<U, C, T>.Instance.IconName;

            if (BasicIUserMod<U, C, T>.Instance.IsUui())
            {
                AddDragHandle(logo);
                AddDragHandle(titlebar);
            }
        }

        private void AddDragHandle(UIComponent src)
        {
            var dh = src.AddUIComponent<UIDragHandle>();
            dh.target = m_controlContainer;
            dh.size = src.size;
            dh.relativePosition = Vector3.zero;
        }
        #endregion
    }
}