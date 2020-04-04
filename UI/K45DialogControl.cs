using ColossalFramework.UI;
using Klyte.Commons.Extensors;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    internal class K45DialogControl : UICustomControl
    {
        public const string PANEL_ID = "K45Dialog";

        public static UIDynamicPanels.DynamicPanelInfo CreatePanelInfo(UIView view)
        {

            KlyteMonoUtils.CreateUIElement(out UIPanel uiPanelTest, null, PANEL_ID);
            uiPanelTest.enabled=false;
            uiPanelTest.maximumSize = new Vector2(800, view.fixedHeight - 300);
            uiPanelTest.minimumSize = new Vector2(800, 70);
            uiPanelTest.backgroundSprite = "MenuPanel2";
            uiPanelTest.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.CenterVertical;
            uiPanelTest.autoLayout = true;
            uiPanelTest.autoFitChildrenVertically = true;
            uiPanelTest.autoLayoutDirection = LayoutDirection.Vertical;
            uiPanelTest.autoLayoutStart = LayoutStart.TopLeft;
            uiPanelTest.autoLayoutPadding = new RectOffset(0, 0, 0, 10);

            #region Title
            KlyteMonoUtils.CreateUIElement(out UIPanel titleContainer, uiPanelTest.transform, "TitleContainer");
            titleContainer.size = new Vector2(800, 40);


            KlyteMonoUtils.CreateUIElement(out UILabel title, titleContainer.transform, "Title");
            title.text = "<k45symbol K45_HexagonIcon_NOBORDER,5e35b1,K> Klyte45";
            title.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.CenterVertical;
            title.minimumSize = new Vector3(titleContainer.width - 100, 0);
            title.textScale = 2;
            title.textAlignment = UIHorizontalAlignment.Center;



            KlyteMonoUtils.CreateUIElement(out UISprite modIcon, titleContainer.transform, "ModIcon", new Vector4(5, 5, 32, 32));
            modIcon.spriteName = "K45_TLM_Icon";

            KlyteMonoUtils.CreateUIElement(out UIButton closeButton, titleContainer.transform, "CloseButton");
            closeButton.pivot = UIPivotPoint.TopRight;
            closeButton.area = new Vector4(uiPanelTest.width - 5, 3, 32, 32);
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.pressedBgSprite = "buttonclosepressed";


            #endregion

            KlyteMonoUtils.CreateUIElement(out UILabel boxText, uiPanelTest.transform, "BoxText");
            boxText.minimumSize = new Vector2(800, 60);
            boxText.wordWrap = true;
            boxText.autoSize = false;
            boxText.autoHeight = true;
            boxText.processMarkup = true;
            boxText.textAlignment = UIHorizontalAlignment.Center;
            boxText.verticalAlignment = UIVerticalAlignment.Middle;
            boxText.text = "OIAHJdkojhdjkahikjah akjdhakjdhakjdhaiu diua\n<k45symbol K45_HexagonIcon_NOBORDER,5e35b1,K> Klyte45\n\nydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ydiuahdiua  iuda iuahsd iua diuahuidh auid yauid ";


            KlyteMonoUtils.CreateUIElement(out UIPanel buttonContainer, uiPanelTest.transform, "ButtonSupContainer");
            buttonContainer.size = new Vector2(800, 70);
            buttonContainer.autoFitChildrenVertically = true;

            KlyteMonoUtils.CreateUIElement(out UIPanel buttonSubContainer, buttonContainer.transform, "ButtonContainer");
            buttonSubContainer.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.Top;
            buttonSubContainer.pivot = UIPivotPoint.TopCenter;
            buttonSubContainer.autoFitChildrenVertically = true;
            buttonSubContainer.autoFitChildrenHorizontally = true;
            buttonSubContainer.autoLayout = true;
            buttonSubContainer.autoLayoutPadding = new RectOffset(5, 5, 0, 0);
            buttonSubContainer.relativePosition = new Vector3(0, 0);

            KlyteMonoUtils.CreateUIElement(out UIButton button1, buttonSubContainer.transform, "ButtonAction1");
            button1.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.Top;
            button1.size = new Vector2(150, 60);
            button1.text = "AAAAA";
            KlyteMonoUtils.InitButtonFull(button1, false, "ButtonMenu");
            KlyteMonoUtils.CreateUIElement(out UIButton button2, buttonSubContainer.transform, "ButtonAction2");
            button2.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.Top;
            button2.size = new Vector2(150, 60);
            button2.text = "BBBBB";
            KlyteMonoUtils.InitButtonFull(button2, false, "ButtonMenu");
            KlyteMonoUtils.CreateUIElement(out UIButton button3, buttonSubContainer.transform, "ButtonAction3");
            button3.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.Top;
            button3.size = new Vector2(150, 60);
            button3.text = "CCCCC";
            KlyteMonoUtils.InitButtonFull(button3, false, "ButtonMenu");
            KlyteMonoUtils.CreateUIElement(out UIButton button4, buttonSubContainer.transform, "ButtonAction4");
            button4.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.Top;
            button4.size = new Vector2(150, 60);
            button4.text = "DDDDD";
            KlyteMonoUtils.InitButtonFull(button4, false, "ButtonMenu");

            uiPanelTest.gameObject.AddComponent<K45DialogControl>();

            var panelTestInfo = new UIDynamicPanels.DynamicPanelInfo();
            panelTestInfo.GetType().GetField("m_Name", RedirectorUtils.allFlags).SetValue(panelTestInfo, PANEL_ID);
            panelTestInfo.GetType().GetField("m_PanelRoot", RedirectorUtils.allFlags).SetValue(panelTestInfo, uiPanelTest);
            panelTestInfo.GetType().GetField("m_IsModal", RedirectorUtils.allFlags).SetValue(panelTestInfo, true);

            return panelTestInfo;
        }
        public void Awake()
        {
            m_mainPanel = GetComponent<UIPanel>();
            m_titleContainer = m_mainPanel.Find<UIPanel>("TitleContainer");
            m_title = m_titleContainer.Find<UILabel>("Title");
            m_modIcon = m_titleContainer.Find<UISprite>("ModIcon");
            m_closeButton = m_titleContainer.Find<UIButton>("CloseButton");
            m_boxText = m_mainPanel.Find<UILabel>("BoxText");

            m_button1 = m_mainPanel.Find<UIButton>("ButtonAction1");
            m_button2 = m_mainPanel.Find<UIButton>("ButtonAction2");
            m_button3 = m_mainPanel.Find<UIButton>("ButtonAction3");
            m_button4 = m_mainPanel.Find<UIButton>("ButtonAction4");


            m_mainPanel.enabled = false;

            KlyteMonoUtils.LimitWidthAndBox(m_title, out UIPanel boxContainerTitle);
            boxContainerTitle.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.Top;
            boxContainerTitle.relativePosition = new Vector3(0, 2);

            m_closeButton.eventClicked += (x,y) => UIView.library.Hide(PANEL_ID);

            m_mainPanel.enabled = true;

        }
        //UIView.library.ShowModal("K45Dialog");

        private UIPanel m_mainPanel;
        private UIPanel m_titleContainer;
        private UILabel m_title;
        private UISprite m_modIcon;
        private UIButton m_closeButton;
        private UILabel m_boxText;
        private UIButton m_button1;
        private UIButton m_button2;
        private UIButton m_button3;
        private UIButton m_button4;
    }
}
