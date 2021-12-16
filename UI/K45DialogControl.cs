using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using Klyte.Commons.Extensions;
using Klyte.Commons.i18n;
using Klyte.Commons.Redirectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    internal class K45DialogControl : UICustomControl
    {
        public const string PANEL_ID = "K45Dialog";
        public const string VERSION = "20211216";
        private const string TEXT_INPUT_ID = "TextInput";
        private const string DD_INPUT_ID = "DropDownInput";
        private const string TUTORIAL_FOLDER_NAME = "Tutorial";

        #region Panel composition
        public static UIDynamicPanels.DynamicPanelInfo CreatePanelInfo(UIView view)
        {

            KlyteMonoUtils.CreateUIElement(out UIPanel mainPanel, null, PANEL_ID);
            mainPanel.enabled = false;
            mainPanel.maximumSize = new Vector2(0, view.fixedHeight);
            mainPanel.minimumSize = new Vector2(800, 70);
            mainPanel.backgroundSprite = "MenuPanel2";
            mainPanel.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.CenterVertical;
            mainPanel.autoLayout = true;
            mainPanel.autoFitChildrenVertically = true;
            mainPanel.autoLayoutDirection = LayoutDirection.Vertical;
            mainPanel.autoLayoutStart = LayoutStart.TopLeft;
            mainPanel.padding = new RectOffset(5, 5, 0, 0);
            mainPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 10);


            #region Title
            KlyteMonoUtils.CreateUIElement(out UIPanel titleContainer, mainPanel.transform, "TitleContainer");
            titleContainer.size = new Vector2(mainPanel.width, 40);


            KlyteMonoUtils.CreateUIElement(out UILabel title, titleContainer.transform, "Title");
            title.text = "Klyte45";
            title.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.CenterVertical;
            title.minimumSize = new Vector3(titleContainer.width - 100, 0);
            title.textScale = 2;
            title.textAlignment = UIHorizontalAlignment.Center;



            KlyteMonoUtils.CreateUIElement(out UISprite modIcon, titleContainer.transform, "ModIcon", new Vector4(5, 5, 32, 32));
            modIcon.tooltip = $"v{VERSION}{CommonProperties.Acronym}";

            KlyteMonoUtils.CreateUIElement(out UIButton closeButton, titleContainer.transform, "CloseButton");
            closeButton.area = new Vector4(mainPanel.width - 37, 3, 32, 32);
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.pressedBgSprite = "buttonclosepressed";


            #endregion

            #region Texture area
            KlyteMonoUtils.CreateUIElement(out UIPanel textureContainer, mainPanel.transform, "TextureSupContainer");
            textureContainer.size = new Vector2(800, 0);
            textureContainer.autoFitChildrenVertically = true;

            KlyteMonoUtils.CreateUIElement(out UIPanel textureSubContainer, textureContainer.transform, "TextureContainer");
            textureSubContainer.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.Top;
            textureSubContainer.pivot = UIPivotPoint.TopCenter;
            textureSubContainer.autoFitChildrenVertically = true;
            textureSubContainer.autoFitChildrenHorizontally = true;
            textureSubContainer.autoLayout = true;
            textureSubContainer.relativePosition = new Vector3(0, 0);

            KlyteMonoUtils.CreateUIElement(out UITextureSprite textureSprite, textureSubContainer.transform, "TextureSprite");
            textureSprite.size = default;
            #endregion

            #region Text area
            KlyteMonoUtils.CreateUIElement(out UILabel boxText, mainPanel.transform, "BoxText");
            boxText.minimumSize = new Vector2(800, 60);
            boxText.maximumSize = new Vector2(0, view.fixedHeight * 0.8f);
            boxText.clipChildren = true;
            boxText.wordWrap = true;
            boxText.autoSize = false;
            boxText.autoHeight = true;
            boxText.processMarkup = true;
            boxText.padding = new RectOffset(10, 10, 5, 5);
            boxText.textAlignment = UIHorizontalAlignment.Center;
            boxText.verticalAlignment = UIVerticalAlignment.Middle;
            #endregion

            #region Inputs
            CreateInputs(mainPanel, boxText);
            #endregion

            #region Action Buttons
            KlyteMonoUtils.CreateUIElement(out UIPanel buttonContainer, mainPanel.transform, "ButtonSupContainer");
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
            button1.wordWrap = true;
            KlyteMonoUtils.InitButtonFull(button1, false, "ButtonMenu");
            KlyteMonoUtils.CreateUIElement(out UIButton button2, buttonSubContainer.transform, "ButtonAction2");
            button2.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.Top;
            button2.size = new Vector2(150, 60);
            button2.text = "BBBBB";
            button2.wordWrap = true;
            KlyteMonoUtils.InitButtonFull(button2, false, "ButtonMenu");
            KlyteMonoUtils.CreateUIElement(out UIButton button3, buttonSubContainer.transform, "ButtonAction3");
            button3.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.Top;
            button3.size = new Vector2(150, 60);
            button3.text = "CCCCC";
            button3.wordWrap = true;
            KlyteMonoUtils.InitButtonFull(button3, false, "ButtonMenu");
            KlyteMonoUtils.CreateUIElement(out UIButton button4, buttonSubContainer.transform, "ButtonAction4");
            button4.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.Top;
            button4.size = new Vector2(150, 60);
            button4.text = "DDDDD";
            button4.wordWrap = true;
            KlyteMonoUtils.InitButtonFull(button4, false, "ButtonMenu");
            KlyteMonoUtils.CreateUIElement(out UIButton button5, buttonSubContainer.transform, "ButtonAction5");
            button5.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.Top;
            button5.size = new Vector2(150, 60);
            button5.text = "EEEEE";
            button5.wordWrap = true;
            KlyteMonoUtils.InitButtonFull(button5, false, "ButtonMenu");
            #endregion

            #region Bindings creation
            mainPanel.gameObject.AddComponent<K45DialogControl>();
            BindPropertyByKey bindByKey = mainPanel.gameObject.AddComponent<BindPropertyByKey>();
            bindByKey.m_Bindings.AddRange(new List<BindPropertyByKey.BindingInfo>(){
                CreateBind("title"          ,title, "text"),
                CreateBind("icon"           ,modIcon, "spriteName"),
                CreateBind("showClose"          ,closeButton, "isVisible"),
                CreateBind("message"            ,boxText, "text"),
                CreateBind("messageAlign",boxText, "textAlignment"),
                CreateBind("showButton1"        ,button1,"isVisible"),
                CreateBind("showButton2"        ,button2,"isVisible"),
                CreateBind("showButton3"        ,button3,"isVisible"),
                CreateBind("showButton4"        ,button4,"isVisible"),
                CreateBind("showButton5"        ,button5,"isVisible"),
                CreateBind("textButton1"        ,button1,"text"),
                CreateBind("textButton2"        ,button2,"text"),
                CreateBind("textButton3"        ,button3,"text"),
                CreateBind("textButton4"        ,button4,"text"),
                CreateBind("textButton5"        ,button5,"text"),
            });
            #endregion

            #region Declare Dynamic Panel Info
            var panelTestInfo = new UIDynamicPanels.DynamicPanelInfo();
            panelTestInfo.GetType().GetField("m_Name", RedirectorUtils.allFlags).SetValue(panelTestInfo, PANEL_ID);
            panelTestInfo.GetType().GetField("m_PanelRoot", RedirectorUtils.allFlags).SetValue(panelTestInfo, mainPanel);
            panelTestInfo.GetType().GetField("m_IsModal", RedirectorUtils.allFlags).SetValue(panelTestInfo, true);
            #endregion

            return panelTestInfo;
        }

        private static void CreateInputs(UIPanel mainPanel, UILabel boxText)
        {
            KlyteMonoUtils.CreateUIElement(out UITextField textField, mainPanel.transform, TEXT_INPUT_ID);
            textField.minimumSize = new Vector2(boxText.minimumSize.x - 10, 25);
            textField.autoSize = false;
            textField.processMarkup = true;
            textField.padding = new RectOffset(10, 10, 5, 5);
            textField.verticalAlignment = UIVerticalAlignment.Middle;
            textField.horizontalAlignment = UIHorizontalAlignment.Left;
            KlyteMonoUtils.UiTextFieldDefaultsForm(textField);
        }

        private static BindPropertyByKey.BindingInfo CreateBind(string key, UIComponent component, string property) => new BindPropertyByKey.BindingInfo()
        {
            key = key,
            target = new BindingReference()
            {
                component = component,
                memberName = property
            }
        };
        #endregion

        public void Awake() => component.stringUserData = VERSION;
public void Start()
        {
            try
            {
                if (gameObject is null || !gameObject.name.StartsWith("(Library)"))
                {
                    Destroy(this);
                    return;
                }
            }
            catch
            {
                Destroy(this);
                return;
            }
            LogUtils.DoWarnLog($"Starting panel at version {VERSION}");
            BindControls();

            m_properties = m_mainPanel.GetComponent<BindPropertyByKey>();

            #region Events bindings
            BindEvents();

            KlyteMonoUtils.LimitWidthAndBox(m_title, out UIPanel boxContainerTitle);
            boxContainerTitle.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.Top;
            boxContainerTitle.relativePosition = new Vector3(0, 2);

            //This action allow centralize all calls to single object, coming from any mod
            m_mainPanel.objectUserData = new Action<Dictionary<string, object>, Func<int, bool>>((Dictionary<string, object> properties, Func<int, bool> callback) => StartCoroutine(Enqueue(BindProperties.FromDictionary(properties), callback)));
            m_mainPanel.stringUserData = VERSION;


            m_closeButton.eventClicked += (x, y) => Close(0);

            m_mainPanel.enabled = true;

            FontHack();
            #endregion
        }

        private void FontHack()
        {
            var labels = Component.FindObjectsOfType<UILabel>().GroupBy(x => x.font).Select(x => x.First()).ToList();
            var testString = string.Join("", new string[0x500].Select((x, i) =>
            {
                try
                {
                    return char.ConvertFromUtf32(i);
                }
                catch
                {
                    return "";
                }
            }).ToArray());
            foreach (var label in labels)
            {
                var orText = label.text;
                Debug.LogWarning("Font: " + label.font.baseFont.name + " | Original text: " + orText);
                try
                {
                    label.text = testString;
                }
                finally
                {
                    label.text = orText;
                }
            }
        }

        private void BindEvents()
        {
            m_mainPanel.enabled = false;

            m_button1.eventClicked += (x, y) => OnButton1();
            m_button2.eventClicked += (x, y) => OnButton2();
            m_button3.eventClicked += (x, y) => OnButton3();
            m_button4.eventClicked += (x, y) => OnButton4();
            m_button5.eventClicked += (x, y) => OnButton5();
        }

        private void BindControls()
        {
            m_mainPanel = GetComponent<UIPanel>();

            m_titleContainer = m_mainPanel.Find<UIPanel>("TitleContainer");
            m_title = m_titleContainer.Find<UILabel>("Title");
            m_modIcon = m_titleContainer.Find<UISprite>("ModIcon");
            m_closeButton = m_titleContainer.Find<UIButton>("CloseButton");
            m_boxText = m_mainPanel.Find<UILabel>("BoxText");
            m_buttonSupContainer = m_mainPanel.Find<UIPanel>("ButtonSupContainer");

            m_button1 = m_mainPanel.Find<UIButton>("ButtonAction1");
            m_button2 = m_mainPanel.Find<UIButton>("ButtonAction2");
            m_button3 = m_mainPanel.Find<UIButton>("ButtonAction3");
            m_button4 = m_mainPanel.Find<UIButton>("ButtonAction4");
            m_button5 = m_mainPanel.Find<UIButton>("ButtonAction5");

            m_textField = m_mainPanel.Find<UITextField>(TEXT_INPUT_ID);

            m_textureSupContainer = m_mainPanel.Find<UIPanel>("TextureSupContainer");
            m_textureSprite = m_mainPanel.Find<UITextureSprite>("TextureSprite");
        }

        public void Update()
        {
            if (UIView.GetModalComponent()?.GetComponent<K45DialogControl>() != null)
            {
                m_mainPanel.zOrder = UIView.GetModalComponent().zOrder + 1;
            }
        }

        private void OnButton1() => Close(1);

        private void OnButton2() => Close(2);

        private void OnButton3() => Close(3);

        private void OnButton4() => Close(4);
        private void OnButton5() => Close(5);

        private IEnumerator Enqueue(BindProperties properties, Func<int, bool> callback)
        {
            yield return 0;
            lock (this)
            {
                if (m_currentCallback == null)
                {
                    UIView.library.ShowModal(PANEL_ID);
                    SetProperties(properties, callback);
                }
                else
                {
                    string str = properties.ToString();
                    if (m_modalQueue.Where(x => x.First == str).Count() == 0)
                    {
                        m_modalQueue.Enqueue(Tuple.NewRef(ref str, ref properties, ref callback));
                    }
                }
            }
        }

        private void Close(int result)
        {
            if (m_currentCallback?.Invoke(result) ?? true)
            {
                m_currentCallback = null;
                UIView.library.Hide(PANEL_ID);
                if (m_modalQueue.Count > 0)
                {
                    TupleRef<string, BindProperties, Func<int, bool>> next = m_modalQueue.Dequeue();
                    UIView.library.ShowModal(PANEL_ID);
                    SetProperties(next.Second, next.Third);
                }
            }
        }

        private void SetProperties(BindProperties propertiesToSet, Func<int, bool> callback)
        {
            m_mainPanel.autoLayout = true;
            if (propertiesToSet.help_isArticle)
            {
                if (!Directory.Exists(propertiesToSet.help_fullPathName))
                {
                    LogUtils.DoErrorLog($"Invalid tutorial path! {propertiesToSet.help_fullPathName}");
                    Close(-1);
                    return;
                }

                string fullText;
                if (File.Exists($"{propertiesToSet.help_fullPathName}{Path.DirectorySeparatorChar}texts_{KlyteLocaleManager.CurrentLanguageId}.txt"))
                {
                    fullText = File.ReadAllText($"{propertiesToSet.help_fullPathName}{Path.DirectorySeparatorChar}texts_{KlyteLocaleManager.CurrentLanguageId}.txt");
                }
                else if (File.Exists($"{propertiesToSet.help_fullPathName}{Path.DirectorySeparatorChar}texts.txt"))
                {
                    fullText = File.ReadAllText($"{propertiesToSet.help_fullPathName}{Path.DirectorySeparatorChar}texts.txt");
                }
                else
                {
                    LogUtils.DoErrorLog($"Corrupted tutorial path! File \"texts.txt\" not found at folder {propertiesToSet.help_fullPathName}.");
                    Close(-1);
                    return;
                }
                string[] tutorialEntries = Regex.Split(fullText, "<BR>", RegexOptions.Multiline | RegexOptions.ECMAScript);

                int lastPage = tutorialEntries.Length - 1;
                int currentPage = Math.Max(0, Math.Min(lastPage, propertiesToSet.help_currentPage));
                string targetImg = $"{propertiesToSet.help_fullPathName}{Path.DirectorySeparatorChar}{currentPage.ToString("D3")}.jpg";
                string textureImagePath = File.Exists(targetImg) ? targetImg : null;
                string path = propertiesToSet.help_fullPathName;
                string feature = propertiesToSet.help_featureName;
                string[] formatEntries = propertiesToSet.help_formatsEntries;
                LogUtils.DoLog($"IMG: {targetImg}");
                propertiesToSet = new BindProperties
                {
                    icon = propertiesToSet.icon,
                    title = string.Format(Locale.Get("K45_CMNS_HELP_FORMAT"), propertiesToSet.help_featureName, currentPage + 1, lastPage + 1),
                    message = string.Format(tutorialEntries[currentPage], formatEntries),
                    imageTexturePath = textureImagePath,

                    showClose = true,
                    showButton1 = currentPage != 0,
                    textButton1 = "<<<\n" + Locale.Get("K45_CMNS_PREV"),
                    showButton2 = true,
                    textButton2 = Locale.Get("EXCEPTION_OK"),
                    showButton3 = currentPage != lastPage,
                    textButton3 = ">>>\n" + Locale.Get("K45_CMNS_NEXT"),
                };
                callback = (x) =>
                {
                    if (x == 1)
                    {
                        ShowModalHelpAbsolutePath(path, feature, currentPage - 1, formatEntries);
                    }
                    if (x == 3)
                    {
                        ShowModalHelpAbsolutePath(path, feature, currentPage + 1, formatEntries);
                    }
                    return true;
                };

            }

            m_currentCallback = callback;

            m_properties.FindBinding("title").property.value = propertiesToSet.title;
            m_properties.FindBinding("icon").property.value = propertiesToSet.icon ?? CommonProperties.ModIcon;
            m_properties.FindBinding("showClose").property.value = propertiesToSet.showClose || !(propertiesToSet.showButton1 || propertiesToSet.showButton2 || propertiesToSet.showButton3 || propertiesToSet.showButton4);
            m_properties.FindBinding("message").property.value = propertiesToSet.message;
            m_properties.FindBinding("messageAlign").property.value = propertiesToSet.messageAlign;
            m_properties.FindBinding("showButton1").property.value = propertiesToSet.showButton1;
            m_properties.FindBinding("showButton2").property.value = propertiesToSet.showButton2;
            m_properties.FindBinding("showButton3").property.value = propertiesToSet.showButton3;
            m_properties.FindBinding("showButton4").property.value = propertiesToSet.showButton4;
            m_properties.FindBinding("showButton5").property.value = propertiesToSet.showButton5;
            m_properties.FindBinding("textButton1").property.value = propertiesToSet.textButton1 ?? "";
            m_properties.FindBinding("textButton2").property.value = propertiesToSet.textButton2 ?? "";
            m_properties.FindBinding("textButton3").property.value = propertiesToSet.textButton3 ?? "";
            m_properties.FindBinding("textButton4").property.value = propertiesToSet.textButton4 ?? "";
            m_properties.FindBinding("textButton5").property.value = propertiesToSet.textButton5 ?? "";

            m_boxText.textScale = propertiesToSet.smallFont ? 0.75f : 1;
            m_textField.isVisible = propertiesToSet.showTextField;
            m_textField.text = propertiesToSet.defaultTextFieldContent ?? "";

            if (m_dropDown is null)
            {
                KlyteMonoUtils.CreateUIElement(out UIPanel DDpanel, m_mainPanel.transform);
                DDpanel.maximumSize = new Vector2(m_boxText.minimumSize.x - 10, 40);
                DDpanel.anchor = UIAnchorStyle.CenterHorizontal;
                DDpanel.zOrder = m_textField.zOrder + 1;
                DDpanel.autoLayout = true;
                m_dropDown = UIHelperExtension.CloneBasicDropDownNoLabel(new string[0], (x) => { }, DDpanel);
                m_dropDown.name = DD_INPUT_ID;
                m_dropDown.minimumSize = new Vector2(m_boxText.minimumSize.x - 10, 25);
                m_dropDown.size = new Vector2(m_boxText.minimumSize.x - 10, 40);
                m_dropDown.autoSize = false;
                m_dropDown.processMarkup = true;
                m_dropDown.verticalAlignment = UIVerticalAlignment.Middle;
                m_dropDown.horizontalAlignment = UIHorizontalAlignment.Left;
            }
            m_dropDown.parent.isVisible = propertiesToSet.showDropDown;
            m_dropDown.items = propertiesToSet.dropDownOptions?.Split(BindProperties.DD_OPTIONS_SEPARATOR.ToCharArray()) ?? new string[0];
            m_dropDown.selectedIndex = propertiesToSet.dropDownCurrentSelection;


            m_textureSprite.size = default;
            if (!propertiesToSet.imageTexturePath.IsNullOrWhiteSpace())
            {
                if (File.Exists(propertiesToSet.imageTexturePath))
                {
                    byte[] fileData = File.ReadAllBytes(propertiesToSet.imageTexturePath);
                    var tex = new Texture2D(2, 2);
                    if (tex.LoadImage(fileData))
                    {
                        m_textureSupContainer.isVisible = true;
                        m_textureSprite.texture = tex;
                        m_textureSprite.size = new Vector2(tex.width, tex.height);
                        if (m_textureSprite.height > 400)
                        {
                            float proportion = m_textureSprite.width / m_textureSprite.height;
                            m_textureSprite.height = 400;
                            m_textureSprite.width = proportion * 400;
                        }
                        m_textureSupContainer.height = m_textureSprite.size.y;
                    }
                    else
                    {
                        LogUtils.DoWarnLog($"Failed loading image: {propertiesToSet.imageTexturePath}");
                        m_textureSupContainer.isVisible = false;
                    }
                }
            }
            else
            {
                m_textureSprite.texture = null;
                m_textureSupContainer.isVisible = false;
            }

            float width;
            if (propertiesToSet.useFullWindowWidth || m_textureSprite.width > 800)
            {
                width = UIView.GetAView().fixedWidth - 100;
                if (width < m_textureSprite.width)
                {
                    float proportion = m_textureSprite.width / m_textureSprite.height;
                    m_textureSprite.width = width;
                    m_textureSprite.height = width / proportion;
                }
            }
            else
            {
                width = 800;
            }
            m_mainPanel.width = width;
            m_closeButton.area = new Vector4(width - 37, 3, 32, 32);
            width -= m_mainPanel.padding.horizontal;
            m_titleContainer.width = width;
            m_boxText.width = width;
            m_buttonSupContainer.width = width;
            m_textureSupContainer.width = width;
            m_mainPanel.autoLayout = !propertiesToSet.showDropDown;
        }

        #region Field Declaration
        private Func<int, bool> m_currentCallback;
        //queue to store the modal order
        private readonly Queue<TupleRef<string, BindProperties, Func<int, bool>>> m_modalQueue = new Queue<TupleRef<string, BindProperties, Func<int, bool>>>();

        private UIPanel m_mainPanel;
        private UIPanel m_titleContainer;
        private UILabel m_title;
        private UISprite m_modIcon;
        private UIButton m_closeButton;
        private UILabel m_boxText;
        private UIPanel m_buttonSupContainer;
        private UIButton m_button1;
        private UIButton m_button2;
        private UIButton m_button3;
        private UIButton m_button4;
        private UIButton m_button5;
        private UITextField m_textField;
        private UIDropDown m_dropDown;
        private UITextureSprite m_textureSprite;
        private UIPanel m_textureSupContainer;

        private BindPropertyByKey m_properties;
        #endregion

        #region Modal calls
        public static void ShowModal(BindProperties properties, Func<int, bool> action)
        {
            properties.showTextField = false;
            if (Dispatcher.mainSafe != Dispatcher.currentSafe)
            {
                ThreadHelper.dispatcher.Dispatch(() => ShowModalInternal(properties, action));
            }
            else
            {
                ShowModalInternal(properties, action);
            }
        }
        public static void ShowModalError(string title, string message, bool showGitHubButton = false)
        {
            BindProperties properties = new BindProperties
            {
                title = "Error!",
                message = $"<color red>{title}</color>" + (message is null ? "" : $"\nDetails:\n\n{message}"),
                showButton1 = true,
                showButton2 = showGitHubButton && !CommonProperties.GitHubRepoPath.IsNullOrWhiteSpace(),
                showClose = true,
                textButton1 = Locale.Get("EXCEPTION_OK"),
                textButton2 = "GitHub: open an issue to check this",
                showTextField = false,
                useFullWindowWidth = true,
                smallFont = true
            };
            bool action(int x)
            {
                if (x == 2)
                {
                    FileSystemUtils.OpenURLInOverlayOrBrowser($"https://github.com/{CommonProperties.GitHubRepoPath}/issues/new");
                    return false;
                }
                return true;
            }
            if (Dispatcher.mainSafe != Dispatcher.currentSafe)
            {
                ThreadHelper.dispatcher.Dispatch(() => ShowModalInternal(properties, action));
            }
            else
            {
                ShowModalInternal(properties, action);
            }
        }

        private static void ShowModalInternal(BindProperties properties, Func<int, bool> action)
        {
            UIComponent uIComponent = UIView.library.Get(PANEL_ID);
            if (!(uIComponent is null) && uIComponent.objectUserData is Action<Dictionary<string, object>, Func<int, bool>> addAction)
            {
                addAction(properties.ToDictionary(), action);
            }
            else
            {
                LogUtils.DoErrorLog($"Panel wasn't found! {Environment.StackTrace}");
            }
        }

        public static void UpdateCurrentMessage(string newText)
        {
            UIComponent uIComponent = UIView.library.Get(PANEL_ID);
            if (!(uIComponent is null) && uIComponent.GetComponent<BindPropertyByKey>() is BindPropertyByKey properties && properties != null)
            {
                properties.FindBinding("message").property.value = newText;
            }
            else
            {
                LogUtils.DoErrorLog($"Panel wasn't found! {Environment.StackTrace}");
            }
        }

        public static void ShowModalPromptText(BindProperties properties, Func<int, string, bool> action)
        {
            properties.showTextField = true;
            if (Dispatcher.mainSafe != Dispatcher.currentSafe)
            {
                ThreadHelper.dispatcher.Dispatch(() => ShowModalPromptTextInternal(properties, action));
            }
            else
            {
                ShowModalPromptTextInternal(properties, action);
            }
        }

        private static void ShowModalPromptTextInternal(BindProperties properties, Func<int, string, bool> action)
        {
            UIComponent uIComponent = UIView.library.Get(PANEL_ID);
            if (!(uIComponent is null) && uIComponent.objectUserData is Action<Dictionary<string, object>, Func<int, bool>> addAction)
            {
                bool targetAction(int x)
                {
                    string result = uIComponent.Find<UITextField>(TEXT_INPUT_ID)?.text;
                    return action(x, result);
                }

                addAction(properties.ToDictionary(), targetAction);
            }
            else
            {
                LogUtils.DoErrorLog($"Panel wasn't found! {Environment.StackTrace}");
            }
        }

        public static void ShowModalHelp(string pathName, string featureName, int startPage, params string[] formatsEntries)
        {
            string fullPathName = $"{CommonProperties.ModDllRootFolder}{Path.DirectorySeparatorChar}{TUTORIAL_FOLDER_NAME}{Path.DirectorySeparatorChar}{pathName}";

            var properties = new BindProperties
            {
                icon = CommonProperties.ModIcon,
                help_isArticle = true,
                help_currentPage = startPage,
                help_fullPathName = fullPathName,
                help_featureName = featureName,
                help_formatsEntries = new string[] { featureName }.Union(formatsEntries).ToArray(),
            };
            if (Dispatcher.mainSafe != Dispatcher.currentSafe)
            {
                ThreadHelper.dispatcher.Dispatch(() => ShowModalInternal(properties, null));
            }
            else
            {
                ShowModalInternal(properties, null);
            }
        }
        private static void ShowModalHelpAbsolutePath(string fullPathName, string featureName, int startPage, params string[] formatsEntries)
        {

            var properties = new BindProperties
            {
                icon = CommonProperties.ModIcon,
                help_isArticle = true,
                help_currentPage = startPage,
                help_fullPathName = fullPathName,
                help_featureName = featureName,
                help_formatsEntries = formatsEntries,
            };
            if (Dispatcher.mainSafe != Dispatcher.currentSafe)
            {
                ThreadHelper.dispatcher.Dispatch(() => ShowModalInternal(properties, null));
            }
            else
            {
                ShowModalInternal(properties, null);
            }
        }



        public static void ShowModalPromptDropDown(BindProperties properties, string[] options, int selIdx, Func<int, int, string, bool> action)
        {
            if (options == null || options.Length == 0)
            {
                LogUtils.DoErrorLog("ShowModalPromptDropDown: INVALID OPTIONS LIST!!!");
                return;
            }
            properties.showDropDown = true;
            properties.dropDownOptions = string.Join(BindProperties.DD_OPTIONS_SEPARATOR, options);
            properties.dropDownCurrentSelection = selIdx;
            if (Dispatcher.mainSafe != Dispatcher.currentSafe)
            {
                ThreadHelper.dispatcher.Dispatch(() => ShowModalPromptDropDownInternal(properties, action));
            }
            else
            {
                ShowModalPromptDropDownInternal(properties, action);
            }
        }

        private static void ShowModalPromptDropDownInternal(BindProperties properties, Func<int, int, string, bool> action)
        {
            UIComponent uIComponent = UIView.library.Get(PANEL_ID);
            if (uIComponent != null && uIComponent.objectUserData is Action<Dictionary<string, object>, Func<int, bool>> addAction)
            {
                bool targetAction(int x)
                {
                    UIDropDown dd = uIComponent.Find<UIDropDown>(DD_INPUT_ID);
                    string result = dd?.selectedValue;
                    int selIdx = dd?.selectedIndex ?? -2;
                    return action(x, selIdx, result);
                }
                addAction(properties.ToDictionary(), targetAction);
            }
            else
            {
                LogUtils.DoErrorLog($"Panel wasn't found! {Environment.StackTrace}");
            }
        }
        #endregion

        #region Extra Classes
        internal struct BindProperties
        {
            public const string DD_OPTIONS_SEPARATOR = "∫";

            public string title;
            public string icon;
            public bool showClose;
            public string message;
            public UIHorizontalAlignment messageAlign;
            public bool showButton1;
            public bool showButton2;
            public bool showButton3;
            public bool showButton4;
            public bool showButton5;
            public string textButton1;
            public string textButton2;
            public string textButton3;
            public string textButton4;
            public string textButton5;
            public bool useFullWindowWidth;
            public bool showTextField;
            public bool showDropDown;
            public string dropDownOptions;
            public int dropDownCurrentSelection;
            public string defaultTextFieldContent;
            public string imageTexturePath;
            public bool smallFont;

            public bool help_isArticle;
            public string help_fullPathName;
            public int help_currentPage;
            public string help_featureName;
            public string[] help_formatsEntries;



            public static BindProperties FromDictionary(Dictionary<string, object> dict)
            {
                var result = new BindProperties();
                foreach (KeyValuePair<string, object> kv in dict)
                {
                    switch (kv.Key)
                    {
                        case "title": result.title = (string)kv.Value; break;
                        case "icon": result.icon = (string)kv.Value; break;
                        case "showClose": result.showClose = (bool)kv.Value; break;
                        case "message": result.message = (string)kv.Value; break;
                        case "messageAlign": result.messageAlign = (UIHorizontalAlignment)kv.Value; break;
                        case "showButton1": result.showButton1 = (bool)kv.Value; break;
                        case "showButton2": result.showButton2 = (bool)kv.Value; break;
                        case "showButton3": result.showButton3 = (bool)kv.Value; break;
                        case "showButton4": result.showButton4 = (bool)kv.Value; break;
                        case "showButton5": result.showButton5 = (bool)kv.Value; break;
                        case "textButton1": result.textButton1 = (string)kv.Value; break;
                        case "textButton2": result.textButton2 = (string)kv.Value; break;
                        case "textButton3": result.textButton3 = (string)kv.Value; break;
                        case "textButton4": result.textButton4 = (string)kv.Value; break;
                        case "textButton5": result.textButton5 = (string)kv.Value; break;
                        case "useFullWindowWidth": result.useFullWindowWidth = (bool)kv.Value; break;
                        case "showTextField": result.showTextField = (bool)kv.Value; break;
                        case "showDropDown": result.showDropDown = (bool)kv.Value; break;
                        case "dropDownOptions": result.dropDownOptions = (string)kv.Value; break;
                        case "dropDownCurrentSelection": result.dropDownCurrentSelection = (int)kv.Value; break;
                        case "defaultTextFieldContent": result.defaultTextFieldContent = (string)kv.Value; break;
                        case "imageTexturePath": result.imageTexturePath = (string)kv.Value; break;
                        case "smallFont": result.smallFont = (bool)kv.Value; break;

                        case "help_isArticle": result.help_isArticle = (bool)kv.Value; break;
                        case "help_fullPathName": result.help_fullPathName = (string)kv.Value; break;
                        case "help_currentPage": result.help_currentPage = (int)kv.Value; break;
                        case "help_featureName": result.help_featureName = (string)kv.Value; break;
                        case "help_formatsEntries": result.help_formatsEntries = (string[])kv.Value; break;

                    }
                }
                return result;
            }

            public Dictionary<string, object> ToDictionary() => new Dictionary<string, object>()
            {
                ["title"] = title ?? CommonProperties.ModName,
                ["icon"] = icon ?? CommonProperties.ModIcon,
                ["showClose"] = showClose,
                ["message"] = message,
                ["messageAlign"] = messageAlign,
                ["showButton1"] = showButton1,
                ["showButton2"] = showButton2,
                ["showButton3"] = showButton3,
                ["showButton4"] = showButton4,
                ["showButton5"] = showButton5,
                ["textButton1"] = textButton1,
                ["textButton2"] = textButton2,
                ["textButton3"] = textButton3,
                ["textButton4"] = textButton4,
                ["textButton5"] = textButton5,
                ["useFullWindowWidth"] = useFullWindowWidth,
                ["showTextField"] = showTextField,
                ["showDropDown"] = showDropDown,
                ["dropDownOptions"] = dropDownOptions,
                ["dropDownCurrentSelection"] = dropDownCurrentSelection,
                ["defaultTextFieldContent"] = defaultTextFieldContent,
                ["imageTexturePath"] = imageTexturePath,
                ["smallFont"] = smallFont,

                ["help_isArticle"] = help_isArticle,
                ["help_fullPathName"] = help_fullPathName,
                ["help_currentPage"] = help_currentPage,
                ["help_featureName"] = help_featureName,
                ["help_formatsEntries"] = help_formatsEntries,
            };

            public override string ToString() => string.Join(",", ToDictionary().ToList().Select(x => $"{x.Key}≠{x.Value}").ToArray());
        }


        #endregion

        public void OnDestroy()
        {
            LogUtils.DoWarnLog($"K45 PANEL REMOVED @ {Environment.StackTrace}");
            UIDynamicPanelsRedirector.RemovePanel();
    }
    }
}