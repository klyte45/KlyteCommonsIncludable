﻿using ColossalFramework;
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

        public static void AddVector2Field(string label, out UITextField[] fieldArray, UIHelperExtension parentHelper, Action<Vector2> onChange, bool addRollEvent = true)
        {
            fieldArray = parentHelper.AddVector2Field(label, Vector3.zero, onChange);
            KlyteMonoUtils.LimitWidthAndBox(fieldArray[0].parent.GetComponentInChildren<UILabel>(), (parentHelper.Self.width / 2) - 10, true);
            if (addRollEvent)
            {
                fieldArray.ForEach(x =>
                    {
                        x.eventMouseWheel += RollFloat;
                        x.tooltip = Locale.Get("K45_CMNS_FLOAT_EDITOR_TOOLTIP_HELP");
                    });
            }
            fieldArray[0].zOrder = 1;
            fieldArray[1].zOrder = 2;
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
            //m_Popup.items = refDD.items;
            //m_Popup.filteredItems = (int[])m_FilteredItems.GetValue(refDD);
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
            Func<string> getContentToSave, Action<UIHelperExtension> doWithLibGroup = null) where LIB : LibBaseFile<LIB, DESC>, new() where DESC : ILibable
        {
            KlyteMonoUtils.CreateUIElement(out UIPanel cbPanel, parentHelper.Self.transform);
            UILabel label = UIHelperExtension.AddLabel(cbPanel, Locale.Get("K45_CMNS_CLIPBOARD_TITLE"), parentHelper.Self.width);
            cbPanel.autoLayoutDirection = LayoutDirection.Horizontal;
            cbPanel.wrapLayout = false;
            cbPanel.autoLayout = true;
            cbPanel.autoFitChildrenHorizontally = true;
            cbPanel.autoFitChildrenVertically = true;
            label.verticalAlignment = UIVerticalAlignment.Middle;
            label.minimumSize = new Vector2(parentHelper.Self.width / 2, 40);
            KlyteMonoUtils.LimitWidthAndBox(label);

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


            AddDropdown(Locale.Get("K45_CMNS_LOAD_FROM_LIB"), out UIDropDown loadDD, parentHelper, LibBaseFile<LIB, DESC>.Instance.List().ToArray(), (x) => { });
            loadDD.width -= 80;
            UIPanel parent = loadDD.GetComponentInParent<UIPanel>();
            ConfigureActionButton(parent, CommonsSpriteNames.K45_Load, (x, t) =>
            {
                DESC groupInfo = LibBaseFile<LIB, DESC>.Instance.Get(loadDD.selectedValue);
                if (groupInfo != null)
                {
                    onLoad(XmlUtils.DefaultXmlSerialize(groupInfo));
                }

            }, "LOAD");

            ConfigureActionButton(parent, CommonsSpriteNames.K45_RemoveIcon, (x, t) =>
            {
                DESC groupInfo = LibBaseFile<LIB, DESC>.Instance.Get(loadDD.selectedValue);
                if (groupInfo != null)
                {
                    LibBaseFile<LIB, DESC>.Instance.Remove(loadDD.selectedValue);
                    loadDD.items = LibBaseFile<LIB, DESC>.Instance.List().ToArray();
                }

            }, "CONTENT_DELETE");

            AddTextField(Locale.Get("K45_CMNS_SAVE_TO_LIB"), out UITextField saveTxt, parentHelper, (x) => { });
            saveTxt.width -= 30;
            parent = saveTxt.GetComponentInParent<UIPanel>();
            ConfigureActionButton(parent, CommonsSpriteNames.K45_Save, (x, t) =>
            {
                if (!saveTxt.text.IsNullOrWhiteSpace())
                {
                    DESC newEntry = XmlUtils.DefaultXmlDeserialize<DESC>(getContentToSave() ?? "");
                    if (newEntry != default)
                    {
                        LibBaseFile<LIB, DESC>.Instance.Add(saveTxt.text, ref newEntry);
                        loadDD.items = LibBaseFile<LIB, DESC>.Instance.List().ToArray();
                        loadDD.selectedValue = saveTxt.text;
                    }
                }
            }, "SAVE", 30);
            doWithLibGroup?.Invoke(parentHelper);
            return loadDD;
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

        public static void AddButtonInEditorRow(UIComponent component, CommonsSpriteNames icon, Action onClick, bool reduceSize = true)
        {
            if (reduceSize)
            {
                component.minimumSize -= new Vector2(0, 40);
                component.width -= 40;
            }
            ConfigureActionButton(component.GetComponentInParent<UIPanel>(), icon, (x, y) => onClick(), null);
        }

        public static void AddCheckboxLocale(string localeId, out UICheckBox checkbox, UIHelperExtension helper, OnCheckChanged onCheckChanged)
        {
            checkbox = helper.AddCheckboxLocale(localeId, false, onCheckChanged);
            KlyteMonoUtils.LimitWidthAndBox(checkbox.label, helper.Self.width - 50);
        }

        public static void InitTabButton(UIComponent parent, out UIButton tabTemplate, string text, Vector2 size, MouseEventHandler onClicked)
        {
            KlyteMonoUtils.CreateUIElement(out tabTemplate, parent.transform, text, new UnityEngine.Vector4(0, 0, 40, 40));
            KlyteMonoUtils.InitButton(tabTemplate, false, "GenericTab");
            tabTemplate.autoSize = false;
            tabTemplate.size = size;
            tabTemplate.text = text;
            tabTemplate.group = parent;
            if (onClicked != null)
            {
                tabTemplate.eventClicked += onClicked;
            }
        }

        public static UIListBox ConfigureListSelectionPopupForUITextField(UITextField textField, Func<string[]> FilterFunc, Action<int> OnSelectItem, Func<string> GetCurrentSelectionName)
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
                result.isVisible = true;
                result.items = FilterFunc();
                result.selectedIndex = Array.IndexOf(result.items, textField.text);
                result.EnsureVisible(result.selectedIndex);
                textField.SelectAll();
            };
            textField.eventLostFocus += (x, t) =>
            {
                if (result.selectedIndex >= 0)
                {
                    textField.text = result.items[result.selectedIndex];
                    OnSelectItem(result.selectedIndex);
                }
                else
                {
                    textField.text = GetCurrentSelectionName();
                }
                result.isVisible = false;
            };
            textField.eventKeyUp += (x, y) =>
            {
                if (textField.hasFocus)
                {
                    result.items = FilterFunc();
                    result.Invalidate();
                }
            };
            result.eventSelectedIndexChanged += (x, y) =>
            {
                if (!textField.hasFocus)
                {
                    if (result.selectedIndex >= 0)
                    {
                        textField.text = result.items[result.selectedIndex];
                        OnSelectItem(result.selectedIndex);
                    }
                    else
                    {
                        textField.text = "";
                    }
                }
            };
            return result;
        }
        #endregion
    }

}
