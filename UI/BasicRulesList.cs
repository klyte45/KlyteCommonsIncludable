using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Libraries;
using Klyte.Commons.UI.SpriteNames;
using Klyte.Commons.Utils;
using System;
using System.Linq;
using UnityEngine;
using static Klyte.Commons.UI.DefaultEditorUILib;

namespace Klyte.Commons.UI
{
    public abstract class BasicRulesList<D, L, I> : UICustomControl where D : ILibable, new() where L : LibBaseFile<L, I>, new() where I : ILibableAsContainer<D>, new()
    {
        private UIPanel MainContainer { get; set; }

        protected UIButton m_new;
        protected UIButton m_up;
        protected UIButton m_down;
        protected UIButton m_remove;
        protected UIButton m_import;
        protected UIButton m_export;
        protected UIButton m_help;

        private UIScrollablePanel m_orderedRulesList;
        private int m_selectedIndex;

        protected abstract ref D[] ReferenceData { get; }
        protected abstract void OnTabstripFix();
        protected abstract void Help_RulesList();

        protected abstract string LocaleRuleListTitle { get; }
        protected abstract string LocaleImport { get; }
        protected abstract string LocaleExport { get; }

        protected virtual string LocaleAddItem => "K45_CMNS_ADDRULE";
        protected virtual string LocaleRemoveItem => "K45_CMNS_REMOVERULE";
        protected virtual string LocaleUp => "K45_CMNS_MOVERULEUP";
        protected virtual string LocaleDown => "K45_CMNS_MOVERULEDOWN";

        protected virtual string LocaleExportTitle => $"{LocaleExport}_TITLE";
        protected virtual string LocaleExportMessage => $"{LocaleExport}_MESSAGE";
        protected virtual string LocaleImportTitle => $"{LocaleImport}_TITLE";
        protected virtual string LocaleImportMessage => $"{LocaleImport}_MESSAGE";
        protected virtual string LocaleExportConfirmOverwrite => $"{LocaleExportMessage}_CONFIRMOVERWRITE";
        protected virtual string LocaleImportNoEntryFound => $"{LocaleImportMessage}_NOENTRIESFOUND";
        protected virtual string LocaleExportSuccessSave => $"{LocaleExportMessage}_SUCCESSSAVEDATA";


        public int SelectedIndex
        {
            get => m_selectedIndex; private set {
                m_selectedIndex = value;
                EventSelectionChanged?.Invoke(value);
            }
        }

        public event Action<int> EventSelectionChanged;


        public void Awake()
        {

            MainContainer = GetComponent<UIPanel>();
            MainContainer.autoLayout = true;
            MainContainer.autoLayoutDirection = LayoutDirection.Vertical;
            MainContainer.autoLayoutPadding = new RectOffset(0, 0, 4, 4);


            KlyteMonoUtils.CreateUIElement(out UIPanel m_topPanel, MainContainer.transform, "topListPanel", new UnityEngine.Vector4(0, 0, MainContainer.width, 111));
            m_topPanel.autoLayout = true;
            m_topPanel.autoLayoutDirection = LayoutDirection.Horizontal;
            m_topPanel.wrapLayout = true;
            m_topPanel.autoLayoutPadding = new RectOffset(8, 8, 5, 5);

            KlyteMonoUtils.CreateUIElement(out UILabel m_topPanelTitle, m_topPanel.transform, "topListPanelTitle", new UnityEngine.Vector4(0, 0, m_topPanel.width - 16, 15));
            KlyteMonoUtils.LimitWidthAndBox(m_topPanelTitle, m_topPanel.width - 16, true);
            m_topPanelTitle.text = Locale.Get(LocaleRuleListTitle);
            m_topPanelTitle.textAlignment = UIHorizontalAlignment.Center;

            int btnSize = 36;
            KlyteMonoUtils.CreateUIElement<UILabel>(out UILabel spacing, m_topPanel.transform, "_", new Vector4(0, 0, btnSize / 2, btnSize));
            spacing.textScale = 0;
            spacing.width = btnSize / 4;
            KlyteMonoUtils.InitCircledButton(m_topPanel, out m_import, CommonsSpriteNames.K45_Import, OnImportData, LocaleImport, btnSize);
            KlyteMonoUtils.InitCircledButton(m_topPanel, out m_export, CommonsSpriteNames.K45_Export, (x, y) => OnExportData(), LocaleExport, btnSize);
            KlyteMonoUtils.InitCircledButton(m_topPanel, out m_help, CommonsSpriteNames.K45_QuestionMark, (x, y) => Help_RulesList(), "K45_CMNS_HELP", btnSize);
            KlyteMonoUtils.InitCircledButton(m_topPanel, out m_new, CommonsSpriteNames.K45_New, OnAddItemOnList, LocaleAddItem, btnSize);
            KlyteMonoUtils.InitCircledButton(m_topPanel, out m_up, CommonsSpriteNames.K45_Up, OnMoveItemUpOnList, LocaleUp, btnSize);
            KlyteMonoUtils.InitCircledButton(m_topPanel, out m_down, CommonsSpriteNames.K45_Down, OnMoveItemDownOnList, LocaleDown, btnSize);
            KlyteMonoUtils.InitCircledButton(m_topPanel, out m_remove, CommonsSpriteNames.K45_X, OnRemoveItem, LocaleRemoveItem, btnSize);

            KlyteMonoUtils.CreateUIElement(out UIPanel m_listContainer, MainContainer.transform, "previewPanel", new Vector4(0, 0, MainContainer.width, MainContainer.height - 126));
            KlyteMonoUtils.CreateScrollPanel(m_listContainer, out m_orderedRulesList, out _, m_listContainer.width - 20, m_listContainer.height);
            m_orderedRulesList.backgroundSprite = "OptionsScrollbarTrack";
            m_orderedRulesList.autoLayout = true;
            m_orderedRulesList.autoLayoutDirection = LayoutDirection.Vertical;

        }

        private void OnExportData(string defaultText = null)
        {
            K45DialogControl.ShowModalPromptText(new K45DialogControl.BindProperties
            {
                defaultTextFieldContent = defaultText,
                title = Locale.Get(LocaleExportTitle),
                message = Locale.Get(LocaleExportMessage),
                showButton1 = true,
                textButton1 = Locale.Get("SAVE"),
                showButton2 = true,
                textButton2 = Locale.Get("CANCEL"),
            }, (ret, text) =>
             {
                 if (ret == 1)
                 {
                     if (text.IsNullOrWhiteSpace())
                     {
                         K45DialogControl.UpdateCurrentMessage($"<color #FFFF00>{Locale.Get($"{LocaleExportMessage}_INVALIDNAME")}</color>\n\n{Locale.Get(LocaleExportMessage)}");
                         return false;
                     }
                     LibBaseFile<L, I>.Reload();
                     I currentData = LibBaseFile<L, I>.Instance.Get(text);
                     if (currentData == null)
                     {
                         AddCurrentListToLibrary(text);
                     }
                     else
                     {
                         K45DialogControl.ShowModal(new K45DialogControl.BindProperties
                         {
                             title = Locale.Get(LocaleExportTitle),
                             message = string.Format(Locale.Get(LocaleExportConfirmOverwrite), text),
                             showButton1 = true,
                             textButton1 = Locale.Get("YES"),
                             showButton2 = true,
                             textButton2 = Locale.Get("NO"),
                         }, (x) =>
                         {
                             if (x == 1)
                             {
                                 AddCurrentListToLibrary(text);
                             }
                             else
                             {
                                 OnExportData(text);
                             }
                             return true;
                         });
                     }
                 }
                 return true;
             });

        }


        private void AddCurrentListToLibrary(string text)
        {
            LibBaseFile<L, I>.Reload();
            var newItem = new I
            {
                m_dataArray = ReferenceData
            };
            LibBaseFile<L, I>.Instance.Add(text,  newItem);
            K45DialogControl.ShowModal(new K45DialogControl.BindProperties
            {
                title = Locale.Get(LocaleExportTitle),
                message = string.Format(Locale.Get(LocaleExportSuccessSave), LibBaseFile<L, I>.Instance.DefaultXmlFileBaseFullPath),
                showButton1 = true,
                textButton1 = Locale.Get("EXCEPTION_OK"),
                showButton2 = true,
                textButton2 = Locale.Get("K45_CMNS_GOTO_FILELOC"),
            }, (x) =>
            {
                if (x == 2)
                {
                    ColossalFramework.Utils.OpenInFileBrowser(LibBaseFile<L, I>.Instance.DefaultXmlFileBaseFullPath);
                    return false;
                }
                return true;
            });
        }


        private void OnImportData(UIComponent component, UIMouseEventParameter eventParam)
        {
            LibBaseFile<L, I>.Reload();
            string[] optionList = LibBaseFile<L, I>.Instance?.List()?.ToArray();
            if (optionList == null)
            {
                return;
            }

            if (optionList.Length > 0)
            {
                K45DialogControl.ShowModalPromptDropDown(new K45DialogControl.BindProperties
                {
                    title = Locale.Get(LocaleImportTitle),
                    message = Locale.Get(LocaleImportMessage),
                    showButton1 = true,
                    textButton1 = Locale.Get("LOAD"),
                    showButton2 = true,
                    textButton2 = Locale.Get("CANCEL"),
                }, optionList, 0, (ret, idx, selText) =>
                {
                    if (ret == 1)
                    {
                        I newConfig = LibBaseFile<L, I>.Instance.Get(selText);
                        ReferenceData = newConfig.m_dataArray;
                        FixTabstrip();
                        SelectedIndex = -1;
                    }
                    return true;
                });
            }
            else
            {
                K45DialogControl.ShowModal(new K45DialogControl.BindProperties
                {
                    title = Locale.Get(LocaleImportTitle),
                    message = string.Format(Locale.Get(LocaleImportNoEntryFound), LibBaseFile<L, I>.Instance.DefaultXmlFileBaseFullPath),
                    showButton1 = true,
                    textButton1 = Locale.Get("EXCEPTION_OK"),
                    showButton2 = true,
                    textButton2 = Locale.Get("K45_CMNS_GOTO_FILELOC"),
                }, (x) =>
                {
                    if (x == 2)
                    {
                        LibBaseFile<L, I>.Instance.EnsureFileExists();
                        ColossalFramework.Utils.OpenInFileBrowser(LibBaseFile<L, I>.Instance.DefaultXmlFileBaseFullPath);
                        return false;
                    }
                    return true;
                });
            }

        }

        public void Start() => FixTabstrip();


        private UIButton AddTabButton(string tabName)
        {
            InitTabButton(m_orderedRulesList, out UIButton button, tabName, new Vector2(m_orderedRulesList.size.x, 30), null);
            button.text = tabName;
            return button;
        }
        public void FixTabstrip()
        {

            while (m_orderedRulesList.components.Count > ReferenceData.Length)
            {
                Destroy(m_orderedRulesList.components[ReferenceData.Length]);
                m_orderedRulesList.RemoveUIComponent(m_orderedRulesList.components[m_orderedRulesList.components.Count - 1]);
            }
            while (m_orderedRulesList.components.Count < ReferenceData.Length)
            {
                AddTabButton("!!!").eventClicked += (x, y) =>
                {
                    SelectedIndex = x.zOrder;
                    FixTabstrip();
                };
            }
            for (int i = 0; i < ReferenceData.Length; i++)
            {
                (m_orderedRulesList.components[i] as UIButton).text = ReferenceData[i].SaveName;
            }
            OnTabstripFix();
            if (SelectedIndex < 1)
            {
                m_up.Disable();
            }
            else
            {
                m_up.Enable();
            }
            if (SelectedIndex <= -1 || SelectedIndex >= ReferenceData.Length - 1)
            {
                m_down.Disable();
            }
            else
            {
                m_down.Enable();
            }
            if (SelectedIndex < 0 || SelectedIndex >= ReferenceData.Length)
            {
                m_remove.Disable();
            }
            else
            {
                m_remove.Enable();
            }
        }

        private void OnRemoveItem(UIComponent component, UIMouseEventParameter eventParam)
        {
            ReferenceData = ReferenceData.Where((x, y) => y != SelectedIndex).ToArray();
            SelectedIndex = Math.Min(SelectedIndex, ReferenceData.Length - 1);
            FixTabstrip();
        }
        private void OnMoveItemUpOnList(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (SelectedIndex > 0 && ReferenceData.Length > 1)
            {
                D temp = ReferenceData[SelectedIndex];
                ReferenceData[SelectedIndex] = ReferenceData[SelectedIndex - 1];
                ReferenceData[SelectedIndex - 1] = temp;
                SelectedIndex--;
                FixTabstrip();
            }
        }
        private void OnMoveItemDownOnList(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (SelectedIndex < ReferenceData.Length && ReferenceData.Length > 1)
            {
                D temp = ReferenceData[SelectedIndex];
                ReferenceData[SelectedIndex] = ReferenceData[SelectedIndex + 1];
                ReferenceData[SelectedIndex + 1] = temp;
                SelectedIndex++;
                FixTabstrip();
            }
        }
        private void OnAddItemOnList(UIComponent component, UIMouseEventParameter eventParam)
        {
            ReferenceData = ReferenceData.Concat(new D[] { new D
            {
                SaveName = "New rule",
            } }).ToArray();
            SelectedIndex = ReferenceData.Length - 1;
            FixTabstrip();
        }
        public void Update()
        {

            if (MainContainer.isVisible)
            {
                foreach (UIButton btn in m_orderedRulesList.GetComponentsInChildren<UIButton>())
                {
                    if (btn.zOrder == SelectedIndex)
                    {
                        btn.state = UIButton.ButtonState.Focused;
                    }
                    else
                    {
                        btn.state = UIButton.ButtonState.Normal;
                    }
                }
            }
        }
    }

}
