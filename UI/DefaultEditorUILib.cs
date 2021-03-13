using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using Klyte.Commons.Extensors;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Libraries;
using Klyte.Commons.UI.SpriteNames;
using Klyte.Commons.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace Klyte.Commons.UI
{

    internal static class DefaultEditorUILib
    {
        #region UI Utils
        public static void AddColorField(UIHelperExtension helper, string text, out UIColorField m_colorEditor, PropertyChangedEventHandler<Color> onSelectedColorChanged)
        {
            m_colorEditor = helper.AddColorPicker(text, Color.white, (x) => { });
            KlyteMonoUtils.LimitWidthAndBox(m_colorEditor.parent.GetComponentInChildren<UILabel>(), helper.Self.width / 2, true);
            m_colorEditor.eventSelectedColorChanged += onSelectedColorChanged;
        }
        public static void AddIntField(string label, out UITextField field, UIHelperExtension parentHelper, Action<int> onChange, bool acceptNegative)
        {
            field = parentHelper.AddIntField(label, 0, onChange, acceptNegative);
            KlyteMonoUtils.LimitWidthAndBox(field.parent.GetComponentInChildren<UILabel>(), (parentHelper.Self.width / 2) - 10, true);
            field.eventMouseWheel += RollInteger;
        }

        public static UIButton ConfigureActionButton(UIComponent parent, CommonsSpriteNames sprite, MouseEventHandler onClicked, string tooltipLocale, float size = 40)
        {
            KlyteMonoUtils.InitCircledButton(parent, out UIButton actionButton, sprite, onClicked, tooltipLocale, size);
            return actionButton;
        }

        public static void AddSlider(string label, out UISlider slider, UIHelperExtension parentHelper, OnValueChanged onChange, float min, float max, float step, Func<float, string> valueLabelFunc)
        {
            UILabel labelValue = null;
            slider = (UISlider)parentHelper.AddSlider(label, min, max, step, min, (x) =>
            {
                onChange(x);
                labelValue.text = valueLabelFunc(x);
            });
            slider.GetComponentInParent<UIPanel>().autoLayoutDirection = LayoutDirection.Horizontal;
            slider.GetComponentInParent<UIPanel>().autoFitChildrenVertically = true;
            KlyteMonoUtils.LimitWidthAndBox(slider.parent.GetComponentInChildren<UILabel>(), (parentHelper.Self.width / 2) - 10, true);
            labelValue = slider.GetComponentInParent<UIPanel>().AddUIComponent<UILabel>();
            labelValue.textAlignment = UIHorizontalAlignment.Center;
            labelValue.padding = new RectOffset(4, 4, 0, 0);
            KlyteMonoUtils.LimitWidthAndBox(labelValue, (parentHelper.Self.width / 2) - slider.width, true);
            labelValue.text = valueLabelFunc(slider.value);
        }
        public static void AddVector2Field(string label, out UITextField[] fieldArray, UIHelperExtension parentHelper, Action<Vector2> onChange, bool addRollEvent = true, bool integerOnly = false, bool allowNegative = true)
        {
            fieldArray = parentHelper.AddVector2Field(label, Vector3.zero, onChange, integerOnly);
            KlyteMonoUtils.LimitWidthAndBox(fieldArray[0].parent.GetComponentInChildren<UILabel>(), (parentHelper.Self.width / 2) - 10, true);
            if (addRollEvent)
            {
                fieldArray.ForEach(x =>
                {
                    if (integerOnly)
                    {
                        x.eventMouseWheel += RollInteger;
                    }
                    else
                    {
                        x.eventMouseWheel += RollFloat;
                    }
                    x.tooltip = Locale.Get("K45_CMNS_FLOAT_EDITOR_TOOLTIP_HELP");
                });
            }
            fieldArray[0].zOrder = 1;
            fieldArray[1].zOrder = 2;
            if (integerOnly)
            {
                fieldArray.ForEach(x => x.allowFloats = false);
            }
            fieldArray.ForEach(x => x.allowNegative = allowNegative);

        }

        public static void AddVector3Field(string label, out UITextField[] fieldArray, UIHelperExtension parentHelper, Action<Vector3> onChange)
        {
            fieldArray = parentHelper.AddVector3Field(label, Vector3.zero, onChange);
            KlyteMonoUtils.LimitWidthAndBox(fieldArray[0].parent.GetComponentInChildren<UILabel>(), (parentHelper.Self.width / 2) - 10, true);
            fieldArray.ForEach(x =>
            {
                x.eventMouseWheel += RollFloat;
                x.tooltip = Locale.Get("K45_CMNS_FLOAT_EDITOR_TOOLTIP_HELP");
            });
            fieldArray[0].zOrder = 1;
            fieldArray[1].zOrder = 2;
            fieldArray[2].zOrder = 3;
        }

        public static void AddVector4Field(string label, out UITextField[] fieldArray, UIHelperExtension parentHelper, Action<Vector4> onChange)
        {
            fieldArray = parentHelper.AddVector4Field(label, Vector4.zero, onChange);
            KlyteMonoUtils.LimitWidthAndBox(fieldArray[0].parent.GetComponentInChildren<UILabel>(), (parentHelper.Self.width / 2) - 10, true);
            fieldArray.ForEach(x =>
            {
                x.eventMouseWheel += RollFloat;
                x.tooltip = Locale.Get("K45_CMNS_FLOAT_EDITOR_TOOLTIP_HELP");
            });
            fieldArray[0].zOrder = 1;
            fieldArray[1].zOrder = 2;
            fieldArray[2].zOrder = 3;
            fieldArray[3].zOrder = 4;
        }


        private static readonly MethodInfo m_submitField = typeof(UITextField).GetMethod("OnSubmit", RedirectorUtils.allFlags);
        public static void RollFloat(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (component is UITextField tf && tf.numericalOnly && float.TryParse(tf.text, out float currentValue))
            {
                bool ctrlPressed = Event.current.control;
                bool shiftPressed = Event.current.shift;
                bool altPressed = Event.current.alt;
                tf.text = Mathf.Max(tf.allowNegative ? float.MinValue : 0, currentValue + 0.0003f + (eventParam.wheelDelta * (altPressed && ctrlPressed ? 0.001f : ctrlPressed ? 0.1f : altPressed ? 0.01f : shiftPressed ? 10 : 1))).ToString("F3");
                m_submitField.Invoke(tf, new object[0]);
                eventParam.Use();
            }
        }
        public static void RollInteger(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (component is UITextField tf && tf.numericalOnly && float.TryParse(tf.text, out float currentValue))
            {
                bool shiftPressed = Event.current.shift;
                tf.text = Mathf.Max(tf.allowNegative ? float.MinValue : 0, currentValue + 0.0003f + (eventParam.wheelDelta * (shiftPressed ? 10 : 1))).ToString("F0");
                m_submitField.Invoke(tf, new object[0]);
                eventParam.Use();
            }
        }
        public static void AddFloatField(string label, out UITextField field, UIHelperExtension parentHelper, Action<float> onChange, bool acceptNegative)
        {
            field = parentHelper.AddFloatField(label, 0, onChange, acceptNegative);
            field.width = 90;
            KlyteMonoUtils.LimitWidthAndBox(field.parent.GetComponentInChildren<UILabel>(), (parentHelper.Self.width / 2) - 10, true);
            field.eventMouseWheel += RollFloat;
        }
        public static void AddDropdown(string title, out UIDropDown dropdown, UIHelperExtension parentHelper, string[] options, OnDropdownSelectionChanged onChange) => AddDropdown(title, out dropdown, out UILabel label, parentHelper, options, onChange);
        public static void AddDropdown(string title, out UIDropDown dropdown, out UILabel label, UIHelperExtension parentHelper, string[] options, OnDropdownSelectionChanged onChange)
        {
            dropdown = (UIDropDown)parentHelper.AddDropdown(title, options, 0, onChange);
            dropdown.width = (parentHelper.Self.width / 2) - 10;
            dropdown.GetComponentInParent<UIPanel>().autoLayoutDirection = LayoutDirection.Horizontal;
            dropdown.GetComponentInParent<UIPanel>().autoFitChildrenVertically = true;
            label = dropdown.parent.GetComponentInChildren<UILabel>();
            KlyteMonoUtils.LimitWidthAndBox(label, (parentHelper.Self.width / 2) - 10);
            label.padding.top = 10;
        }
        public static void AddTextField(string title, out UITextField textField, UIHelperExtension parentHelper, OnTextSubmitted onSubmit, OnTextChanged onChanged = null) => AddTextField(title, out textField, out UILabel label, parentHelper, onSubmit, onChanged);
        public static void AddTextField(string title, out UITextField textField, out UILabel label, UIHelperExtension parentHelper, OnTextSubmitted onSubmit, OnTextChanged onChanged = null)
        {
            textField = parentHelper.AddTextField(title, onChanged, "", onSubmit);
            textField.width = (parentHelper.Self.width / 2) - 10;
            textField.GetComponentInParent<UIPanel>().autoLayoutDirection = LayoutDirection.Horizontal;
            textField.GetComponentInParent<UIPanel>().autoFitChildrenVertically = true;
            label = textField.parent.GetComponentInChildren<UILabel>();
            KlyteMonoUtils.LimitWidthAndBox(label, (parentHelper.Self.width / 2) - 10, true);
        }
        public static UIListBox CreatePopup(UIPanel rootContainer)
        {
            UIListBox popup;
            UIDropDown refDD = UITemplateUtils.GetTemplateDict()[UIHelperExtension.kDropdownTemplate].GetComponentInChildren<UIDropDown>();
            Vector2 size2 = CalculatePopupSize(rootContainer, 6, refDD.itemHeight, refDD.itemPadding.vertical);
            popup = rootContainer.AddUIComponent<UIListBox>();
            popup.builtinKeyNavigation = refDD.builtinKeyNavigation;
            popup.name = " - List";
            popup.gameObject.hideFlags = HideFlags.DontSave;
            popup.atlas = refDD.atlas;
            popup.anchor = UIAnchorStyle.None;
            popup.font = refDD.font;
            popup.pivot = UIPivotPoint.TopLeft;
            popup.size = size2;
            popup.itemHeight = refDD.itemHeight;
            popup.itemHighlight = refDD.itemHighlight;
            popup.itemHover = refDD.itemHover;
            popup.itemPadding = refDD.itemPadding;
            popup.color = refDD.popupColor;
            popup.itemTextColor = refDD.popupTextColor;
            popup.textScale = refDD.textScale;
            popup.listPadding = refDD.listPadding;
            popup.normalBgSprite = refDD.listBackground;
            popup.useDropShadow = refDD.useDropShadow;
            popup.dropShadowColor = refDD.dropShadowColor;
            popup.dropShadowOffset = refDD.dropShadowOffset;
            popup.useGradient = refDD.useGradient;
            popup.bottomColor = refDD.bottomColor;
            popup.useOutline = refDD.useOutline;
            popup.outlineColor = refDD.outlineColor;
            popup.outlineSize = refDD.outlineSize;
            popup.zOrder = int.MaxValue;
            popup.EnsureVisible(popup.selectedIndex);

            return popup;
        }

        public static void AddLabel(string text, UIHelperExtension parentHelper, out UILabel label, out UIPanel cbPanel, bool autoSize = true)
        {
            label = parentHelper.AddLabel(text);
            label.autoSize = autoSize;
            KlyteMonoUtils.CreateUIElement(out cbPanel, parentHelper.Self.transform);
            cbPanel.autoLayoutDirection = LayoutDirection.Horizontal;
            cbPanel.wrapLayout = false;
            cbPanel.autoLayout = true;
            cbPanel.autoFitChildrenHorizontally = true;
            cbPanel.autoFitChildrenVertically = true;
            cbPanel.width = parentHelper.Self.width;
            label.width = parentHelper.Self.width - 10;
            cbPanel.AttachUIComponent(label.gameObject);
        }

        private static Vector2 CalculatePopupSize(UIPanel root, int itemCount, float itemHeight, float listPaddingVertical)
        {
            float num = root.size.x - root.padding.horizontal;
            float b = itemCount * itemHeight + listPaddingVertical;
            if (itemCount == 0)
            {
                b = itemHeight / 2 + listPaddingVertical;
            }
            return new Vector2(num, b);
        }

        public delegate ref T GetItemReference<T>();

        public static UIDropDown AddLibBox<LIB, DESC>(UIHelperExtension parentHelper, out UIButton copyButton,
            Action actionCopy, out UIButton pasteButton,
            Action actionPaste, out UIButton deleteButton,
            Action actionDelete, Action<string> onLoad,
            Func<string> getContentToSave) where LIB : LibBaseFile<LIB, DESC>, new() where DESC : ILibable

        {
            AddLibBox<LIB, DESC>(parentHelper,
           out copyButton, actionCopy,
           out pasteButton, actionPaste,
           out deleteButton, actionDelete,
           out UIDropDown result, out _, out _,
           out _, out _, out _,
            onLoad, getContentToSave);
            return result;
        }

        public static void AddLibBox<LIB, DESC>(UIHelperExtension parentHelper,
            out UIButton copyButton, Action actionCopy,
            out UIButton pasteButton, Action actionPaste,
            out UIButton deleteButton, Action actionDelete,
            out UIDropDown libFilesDD, out UIButton libLoadButton, out UIButton libDeleteButton,
            out UITextField libSaveNameField, out UIButton libSaveButton, out UIButton goToFileButton,
            Action<string> onLoad, Func<string> getContentToSave) where LIB : LibBaseFile<LIB, DESC>, new() where DESC : ILibable
        {
            KlyteMonoUtils.CreateUIElement(out UIPanel cbPanel, parentHelper.Self.transform);
            UILabel label = UIHelperExtension.AddLabel(cbPanel, Locale.Get("K45_CMNS_CLIPBOARD_TITLE"), parentHelper.Self.width / 2);
            cbPanel.autoLayoutDirection = LayoutDirection.Horizontal;
            cbPanel.wrapLayout = false;
            cbPanel.autoLayout = true;
            cbPanel.autoFitChildrenHorizontally = true;
            cbPanel.autoFitChildrenVertically = true;
            label.verticalAlignment = UIVerticalAlignment.Middle;
            label.minimumSize = new Vector2(parentHelper.Self.width / 2, 40);

            if (actionCopy != null)
            {
                copyButton = ConfigureActionButton(cbPanel, CommonsSpriteNames.K45_Copy, (x, y) => actionCopy(), "EXCEPTION_COPY");
            }
            else
            {
                copyButton = null;
            }
            if (actionPaste != null)
            {
                pasteButton = ConfigureActionButton(cbPanel, CommonsSpriteNames.K45_Paste, (x, y) => actionPaste(), "K45_CMNS_PASTE");
            }
            else
            {
                pasteButton = null;
            }
            if (actionDelete != null)
            {
                deleteButton = ConfigureActionButton(cbPanel, CommonsSpriteNames.K45_RemoveIcon, (x, y) => actionDelete(), "K45_CMNS_DELETE_CURRENT_ITEM");
                deleteButton.color = Color.red;
            }
            else
            {
                deleteButton = null;
            }


            AddDropdown(Locale.Get("K45_CMNS_LOAD_FROM_LIB"), out libFilesDD, parentHelper, LibBaseFile<LIB, DESC>.Instance.List().ToArray(), (x) => { });
            libFilesDD.width -= 80;
            var locDD = libFilesDD;
            UIPanel parent = libFilesDD.GetComponentInParent<UIPanel>();
            libLoadButton = ConfigureActionButton(parent, CommonsSpriteNames.K45_Load, (x, t) =>
             {
                 DESC groupInfo = LibBaseFile<LIB, DESC>.Instance.Get(locDD.selectedValue);
                 if (groupInfo != null)
                 {
                     onLoad(XmlUtils.DefaultXmlSerialize(groupInfo));
                 }

             }, "LOAD");

            libDeleteButton = ConfigureActionButton(parent, CommonsSpriteNames.K45_X, (x, t) =>
             {
                 DESC groupInfo = LibBaseFile<LIB, DESC>.Instance.Get(locDD.selectedValue);
                 if (groupInfo != null)
                 {
                     LibBaseFile<LIB, DESC>.Instance.Remove(locDD.selectedValue);
                     locDD.items = LibBaseFile<LIB, DESC>.Instance.List().ToArray();
                 }

             }, "CONTENT_DELETE");

            AddTextField(Locale.Get("K45_CMNS_SAVE_TO_LIB"), out UITextField saveTxt, parentHelper, (x) => { });
            saveTxt.width -= 30;
            parent = saveTxt.GetComponentInParent<UIPanel>();
            libSaveButton = ConfigureActionButton(parent, CommonsSpriteNames.K45_Save, (x, t) =>
             {
                 if (!saveTxt.text.IsNullOrWhiteSpace())
                 {
                     DESC newEntry = XmlUtils.DefaultXmlDeserialize<DESC>(getContentToSave() ?? "");
                     if (newEntry != default)
                     {
                         LibBaseFile<LIB, DESC>.Instance.Add(saveTxt.text, ref newEntry);
                         locDD.items = LibBaseFile<LIB, DESC>.Instance.List().ToArray();
                         locDD.selectedValue = saveTxt.text;
                     }
                 }
             }, "SAVE", 30);
            libSaveNameField = saveTxt;
            LibBaseFile<LIB, DESC>.Instance.EnsureFileExists();
            goToFileButton = parentHelper.AddButton(Locale.Get("K45_CMNS_GOTO_LIBFILE"), () => ColossalFramework.Utils.OpenInFileBrowser(LibBaseFile<LIB, DESC>.Instance.DefaultXmlFileBaseFullPath)) as UIButton;
            KlyteMonoUtils.LimitWidthAndBox(goToFileButton, parentHelper.Self.width - 20, true);

        }
        public static void SetIcon(UIButton copyButton, CommonsSpriteNames spriteName, Color color)
        {
            UISprite icon = copyButton.AddUIComponent<UISprite>();
            icon.relativePosition = new Vector3(2, 2);
            icon.width = 36;
            icon.height = 36;
            icon.spriteName = KlyteResourceLoader.GetDefaultSpriteNameFor(spriteName);
            icon.color = color;
        }

        public static UIButton AddButtonInEditorRow(UIComponent component, CommonsSpriteNames icon, Action onClick, string tooltip = null, bool reduceSize = true, int width = 40)
        {
            if (reduceSize)
            {
                component.minimumSize -= new Vector2(0, width);
                component.width -= width;
            }
            var result = ConfigureActionButton(component.GetComponentInParent<UIPanel>(), icon, (x, y) => onClick(), tooltip, width);
            result.zOrder = component.zOrder + 1;
            result.canFocus = false;
            return result;
        }
        public static UISprite AddSpriteInEditorRow(UIComponent component, bool reduceSize = true, float width = 40)
        {
            if (reduceSize)
            {
                component.minimumSize -= new Vector2(0, width);
                component.width -= width;
            }
            var sprite = component.GetComponentInParent<UIPanel>().AddUIComponent<UISprite>();
            sprite.width = width;
            sprite.height = component.height;
            sprite.zOrder = component.zOrder + 1;
            return sprite;

        }

        public static void AddCheckboxLocale(string localeId, out UICheckBox checkbox, UIHelperExtension helper, OnCheckChanged onCheckChanged)
        {
            checkbox = helper.AddCheckboxLocale(localeId, false, onCheckChanged);
            KlyteMonoUtils.LimitWidthAndBox(checkbox.label, helper.Self.width - 50);
        }

        public static void InitTabButton(UIComponent parent, out UIButton tabTemplate, string text, Vector2 size, MouseEventHandler onClicked)
        {
            InitTabButton(parent.gameObject, out tabTemplate, text, size, onClicked);
            tabTemplate.group = parent;
        }
        public static void InitTabButton(GameObject go, out UIButton tabTemplate, string text, Vector2 size, MouseEventHandler onClicked)
        {
            KlyteMonoUtils.CreateUIElement(out tabTemplate, go.transform, text, new UnityEngine.Vector4(0, 0, 40, 40));
            KlyteMonoUtils.InitButton(tabTemplate, false, "GenericTab");
            tabTemplate.autoSize = false;
            tabTemplate.size = size;
            tabTemplate.text = text;
            if (onClicked != null)
            {
                tabTemplate.eventClicked += onClicked;
            }
        }

        public static UIListBox ConfigureListSelectionPopupForUITextField(UITextField textField, Func<string, string[]> FilterFunc, Func<string, int, string[], string> OnSelectItem)
        {
            var selectorPanel = textField.parent as UIPanel;
            selectorPanel.autoLayout = true;
            selectorPanel.width = selectorPanel.parent.width;
            selectorPanel.autoFitChildrenHorizontally = false;
            selectorPanel.autoFitChildrenVertically = true;
            selectorPanel.width = selectorPanel.parent.width;
            selectorPanel.wrapLayout = true;

            UIListBox result = CreatePopup(selectorPanel);

            result.isVisible = false;
            textField.eventGotFocus += (x, t) =>
            {
                var items = FilterFunc(textField.text);
                if (items == null)
                {
                    result.isVisible = false;
                }
                else
                {
                    result.isVisible = true;
                    result.items = items;
                    result.selectedIndex = Array.IndexOf(result.items, textField.text);
                    result.EnsureVisible(result.selectedIndex);
                    textField.SelectAll();
                }
            };
            textField.eventLostFocus += (x, t) =>
            {
                if (result.selectedIndex >= 0)
                {
                    textField.text = OnSelectItem(textField.text, result.selectedIndex, result.items) ?? "";
                }
                else if (result.items.Contains(textField.text))
                {
                    textField.text = OnSelectItem(textField.text, Array.IndexOf(result.items, textField.text), result.items) ?? "";
                }
                else
                {
                    textField.text = OnSelectItem(textField.text, result.selectedIndex, result.items) ?? "";
                }
                result.isVisible = false;
            };
            textField.eventKeyUp += (x, y) =>
            {
                if (textField.hasFocus)
                {
                    var items = FilterFunc(textField.text);
                    if (items == null)
                    {
                        result.isVisible = false;
                    }
                    else
                    {
                        result.isVisible = true;
                        result.items = items;
                        result.Invalidate();
                    }
                }
            };
            result.eventItemMouseUp += (x, y) =>
            {
                textField.text = OnSelectItem(textField.text, result.selectedIndex, result.items) ?? "";
                result.isVisible = false;
            };
            result.eventMouseWheel += (x, y) => y.Use();
            return result;
        }

        public static void AddFilterableInput(string name, UIHelperExtension helper, out UITextField inputField, out UIListBox listPopup, Func<string, string[]> OnFilterChanged, Func<string, int, string[], string> OnValueChanged, float popupHeight =290)
        {
            AddTextField(name, out inputField, helper, null);
            inputField.submitOnFocusLost = true;

            KlyteMonoUtils.UiTextFieldDefaultsForm(inputField);
            listPopup = ConfigureListSelectionPopupForUITextField(inputField, OnFilterChanged, OnValueChanged);
            listPopup.height = popupHeight;
            listPopup.width -= 20;
            listPopup.itemHeight = 15;
            listPopup.textScale = 0.7f;
            listPopup.itemPadding = new RectOffset(5, 5, 1, 1);
        }


        public static void AddMultistateButton(string title, UIComponent parent, out UIMultiStateButton multiStateButton, out UILabel label, out UIPanel container, float width, string[] options, PropertyChangedEventHandler<int> OnActiveStateChanged, Vector2? spriteSizeP = null)
        {
            var spriteSize = spriteSizeP ?? Vector2.one * 25;
            if (options == null || options.Length == 0)
            {
                multiStateButton = null;
                label = null;
                container = null;
                return;
            }

            KlyteMonoUtils.CreateUIElement(out container, parent.transform, "MSBContainer", new Vector4(0, 0, width, 25));
            container.autoLayout = true;
            container.autoLayoutDirection = LayoutDirection.Horizontal;
            container.autoLayoutPadding = new RectOffset(5, 0, 0, 0);
            KlyteMonoUtils.CreateUIElement(out multiStateButton, container.transform, "MultistateButton", new Vector4(0, 0, spriteSize.x, spriteSize.y));
            multiStateButton.textPadding = new RectOffset(0, 0, 0, 0);
            multiStateButton.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            multiStateButton.foregroundSprites[0].normal = options[0];

            for (int i = 1; i < options.Length; i++)
            {
                multiStateButton.backgroundSprites.AddState();
                multiStateButton.foregroundSprites.AddState();
                multiStateButton.foregroundSprites[i].normal = options[i];
            }
            multiStateButton.spritePadding = new RectOffset();

            multiStateButton.eventActiveStateIndexChanged += OnActiveStateChanged;


            KlyteMonoUtils.CreateUIElement(out label, container.transform, "MultistateButtonLabel", new Vector4(0, 0, width - 15 - spriteSize.x, spriteSize.y));
            label.text = title;
            label.padding.top = Mathf.RoundToInt(spriteSize.y / 2 - 7.5f);
            KlyteMonoUtils.LimitWidthAndBox(label, width - 15 - spriteSize.x);
        }
        #endregion
    }

}
