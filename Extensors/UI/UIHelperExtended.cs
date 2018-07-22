using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICities;
using ColossalFramework.UI;
using ColossalFramework;
using ColossalFramework.Plugins;
using System.Threading;
using System;
using System.Linq;
using System.Reflection;

namespace Klyte.Commons.Extensors
{
    public class UIHelperExtension : UIHelperBase
    {

        //
        // Static Fields
        //
        public static readonly string kButtonTemplate = "OptionsButtonTemplate";
        public static readonly string kGroupTemplate = "OptionsGroupTemplate";
        public static readonly string kDropdownTemplate = "OptionsDropdownTemplate";
        public static readonly string kCheckBoxTemplate = "OptionsCheckBoxTemplate";
        public static readonly string kSliderTemplate = "OptionsSliderTemplate";
        public static readonly string kTextfieldTemplate = "OptionsTextfieldTemplate";

        public static readonly UIFont defaultFontCheckbox = ((UITemplateManager.GetAsGameObject(kCheckBoxTemplate)).GetComponent<UICheckBox>()).label.font;

        public static string version
        {
            get {
                return typeof(UIHelperExtension).Assembly.GetName().Version.Major + "." + typeof(UIHelperExtension).Assembly.GetName().Version.Minor + "." + typeof(UIHelperExtension).Assembly.GetName().Version.Build;
            }
        }

        //
        // Fields
        //
        private UIComponent m_Root;

        //
        // Properties
        //
        public UIComponent self
        {
            get {
                return this.m_Root;
            }
        }

        //
        // Methods
        //
        public object AddButton(string text, OnButtonClicked eventCallback)
        {
            if (eventCallback != null && !string.IsNullOrEmpty(text))
            {
                UIButton uIButton = this.m_Root.AttachUIComponent(UITemplateManager.GetAsGameObject(kButtonTemplate)) as UIButton;
                uIButton.text = text;
                uIButton.eventClick += delegate (UIComponent c, UIMouseEventParameter sel)
                {
                    eventCallback();
                };
                return uIButton;
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create button with no name or no event");
            return null;
        }

        public object AddCheckbox(string text, bool defaultValue, OnCheckChanged eventCallback)
        {
            if (eventCallback != null && !string.IsNullOrEmpty(text))
            {
                UICheckBox uICheckBox = this.m_Root.AttachUIComponent(UITemplateManager.GetAsGameObject(kCheckBoxTemplate)) as UICheckBox;
                uICheckBox.text = text;
                uICheckBox.isChecked = defaultValue;
                uICheckBox.eventCheckChanged += delegate (UIComponent c, bool isChecked)
                {
                    eventCallback(isChecked);
                };
                return uICheckBox;
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create checkbox with no name or no event");
            return null;
        }
        public UICheckBox AddCheckboxLocale(string text, bool defaultValue, OnCheckChanged eventCallback = null)
        {
            if (!string.IsNullOrEmpty(text))
            {
                UICheckBox uICheckBox = this.m_Root.AttachUIComponent(UITemplateManager.GetAsGameObject(kCheckBoxTemplate)) as UICheckBox;
                uICheckBox.label.isLocalized = true;
                uICheckBox.label.localeID = text;
                uICheckBox.isChecked = defaultValue;
                if (eventCallback != null)
                {
                    uICheckBox.eventCheckChanged += delegate (UIComponent c, bool isChecked)
                    {
                        eventCallback(isChecked);
                    };
                }
                return uICheckBox;
            }
            throw new NotSupportedException("Cannot create checkbox with no name");
        }

        public object AddDropdown(string text, string[] options, int defaultSelection, OnDropdownSelectionChanged eventCallback)
        {
            UIDropDown uIDropDown = AddDropdownBase(text, options, eventCallback);
            if (uIDropDown != null)
            {
                uIDropDown.selectedIndex = defaultSelection;                
                return uIDropDown;
            }
            else
            {
                return null;
            }
        }

        public void AddLabel(object p)
        {
            throw new NotImplementedException();
        }

        public UIDropDown AddDropdownLocalized(string text, string[] options, int defaultSelection, OnDropdownSelectionChanged eventCallback)
        {
            UIDropDown uIDropDown = AddDropdownBaseLocalized(text, options, eventCallback, defaultSelection);
            if (uIDropDown != null)
            {
                return uIDropDown;
            }
            else
            {
                return null;
            }
        }

        public UIDropDown AddDropdown(string text, string[] options, string defaultSelection, OnDropdownSelectionChanged eventCallback)
        {
            UIDropDown uIDropDown = AddDropdownBase(text, options, eventCallback);
            if (uIDropDown != null)
            {
                bool hasIdx = options.Contains(defaultSelection);
                if (hasIdx)
                {
                    uIDropDown.selectedIndex = options.ToList().IndexOf(defaultSelection);
                }
                return uIDropDown;
            }
            else
            {
                return null;
            }
        }

        private UIDropDown AddDropdownBase(string text, string[] options, OnDropdownSelectionChanged eventCallback)
        {
            return CloneBasicDropDown(text, options, eventCallback, this.m_Root);
        }

        public static UIDropDown CloneBasicDropDown(string text, string[] options, OnDropdownSelectionChanged eventCallback, UIComponent parent, out UILabel label)
        {
            if (eventCallback != null && !string.IsNullOrEmpty(text))
            {
                UIPanel uIPanel = parent.AttachUIComponent(UITemplateManager.GetAsGameObject(kDropdownTemplate)) as UIPanel;
                label = uIPanel.Find<UILabel>("Label");
                label.text = text;
                UIDropDown uIDropDown = uIPanel.Find<UIDropDown>("Dropdown");
                uIDropDown.items = options;
                uIDropDown.eventSelectedIndexChanged += delegate (UIComponent c, int sel)
                {
                    eventCallback(sel);
                };
                return uIDropDown;
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create dropdown with no name or no event");
            label = null;
            return null;
        }
        public static UIDropDown CloneBasicDropDown(string text, string[] options, OnDropdownSelectionChanged eventCallback, UIComponent parent)
        {
            return CloneBasicDropDown(text, options, eventCallback, parent, out UILabel l);
        }


        private UIDropDown AddDropdownBaseLocalized(string text, string[] options, OnDropdownSelectionChanged eventCallback, int defaultSelection)
        {
            return CloneBasicDropDownLocalized(text, options, eventCallback, defaultSelection, this.m_Root);
        }

        public static UIDropDown CloneBasicDropDownLocalized(string text, string[] options, OnDropdownSelectionChanged eventCallback, int defaultSelection, UIComponent parent)
        {
            if (eventCallback != null && !string.IsNullOrEmpty(text))
            {
                UIPanel uIPanel = parent.AttachUIComponent(UITemplateManager.GetAsGameObject(kDropdownTemplate)) as UIPanel;
                uIPanel.Find<UILabel>("Label").localeID = text;
                uIPanel.Find<UILabel>("Label").isLocalized = true;
                UIDropDown uIDropDown = uIPanel.Find<UIDropDown>("Dropdown");
                uIDropDown.items = options;
                uIDropDown.selectedIndex = defaultSelection;
                uIDropDown.eventSelectedIndexChanged += delegate (UIComponent c, int sel)
                {
                    eventCallback(sel);
                };
                return uIDropDown;
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create dropdown with no name or no event");
            return null;
        }

        public static UIDropDown CloneBasicDropDownNoLabel(string[] options, OnDropdownSelectionChanged eventCallback, UIComponent parent)
        {
            if (eventCallback != null)
            {
                UIDropDown uIDropDown = GameObject.Instantiate(UITemplateManager.GetAsGameObject(kDropdownTemplate).GetComponentInChildren<UIDropDown>().gameObject, parent.transform).GetComponent<UIDropDown>();
                uIDropDown.items = options;
                uIDropDown.eventSelectedIndexChanged += delegate (UIComponent c, int sel)
                {
                    eventCallback(sel);
                };
                return uIDropDown;
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create dropdown with no name or no event");
            return null;
        }

        public object AddSlider(string text, float min, float max, float step, float defaultValue, OnValueChanged eventCallback)
        {
            if (eventCallback != null && !string.IsNullOrEmpty(text))
            {
                UIPanel uIPanel = this.m_Root.AttachUIComponent(UITemplateManager.GetAsGameObject(kSliderTemplate)) as UIPanel;
                uIPanel.Find<UILabel>("Label").text = text;
                UISlider uISlider = uIPanel.Find<UISlider>("Slider");
                uISlider.minValue = min;
                uISlider.maxValue = max;
                uISlider.stepSize = step;
                uISlider.value = defaultValue;
                uISlider.eventValueChanged += delegate (UIComponent c, float val)
                {
                    eventCallback(val);
                };
                return uISlider;
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create slider with no name or no event");
            return null;
        }

        public object AddSpace(int height)
        {
            if (height > 0)
            {
                UIPanel uIPanel = this.m_Root.AddUIComponent<UIPanel>();
                uIPanel.name = "Space";
                uIPanel.isInteractive = false;
                uIPanel.height = (float)height;
                return uIPanel;
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create space of " + height + " height");
            return null;
        }

        public UIHelperExtension(UIComponent panel)
        {
            this.m_Root = panel;
        }

        public UIHelperExtension(UIHelper panel)
        {
            this.m_Root = (UIComponent)panel.self;
        }

        public UIHelperExtension AddGroupExtended(string text)
        {
            return (UIHelperExtension)AddGroup(text);
        }

        public UIHelperBase AddGroup(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                UIPanel uIPanel = this.m_Root.AttachUIComponent(UITemplateManager.GetAsGameObject(UIHelperExtension.kGroupTemplate)) as UIPanel;
                uIPanel.Find<UILabel>("Label").text = text;
                return new UIHelperExtension(uIPanel.Find("Content"));
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create group with no name");
            return null;
        }
                
        public object AddTextfield(string text, string defaultContent, OnTextChanged eventChangedCallback, OnTextSubmitted eventSubmittedCallback)
        {
            if ((eventChangedCallback != null || eventSubmittedCallback != null) && !string.IsNullOrEmpty(text))
            {
                UIPanel uIPanel = this.m_Root.AttachUIComponent(UITemplateManager.GetAsGameObject(kTextfieldTemplate)) as UIPanel;
                uIPanel.Find<UILabel>("Label").text = text;
                UITextField uITextField = uIPanel.Find<UITextField>("Text Field");
                uITextField.text = defaultContent;
                uITextField.eventTextChanged += delegate (UIComponent c, string sel)
                {
                    eventChangedCallback?.Invoke(sel);
                };
                uITextField.eventTextSubmitted += delegate (UIComponent c, string sel)
                {
                    eventSubmittedCallback?.Invoke(sel);
                };
                return uITextField;
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create dropdown with no name or no event");
            return null;
        }

        public UITextField AddTextField(string name, OnTextChanged eventCallback, string defaultValue = "", OnTextSubmitted eventSubmit = null)
        {
            return (UITextField)AddTextfield(name, defaultValue, eventCallback, eventSubmit);
        }

        public UITextField AddPasswordField(string name, OnTextChanged eventCallback)
        {
            if (eventCallback != null && !string.IsNullOrEmpty(name))
            {
                UITextField uITextField = (UITextField)AddTextfield(name, "", eventCallback, null);
                uITextField.isPasswordField = true;
                return uITextField;
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create textField with no name or no event");
            return null;
        }

        public UILabel AddLabel(string name)
        {

            UIPanel uIPanel = m_Root.AttachUIComponent(UITemplateManager.GetAsGameObject(UIHelperExtension.kDropdownTemplate)) as UIPanel;
            uIPanel.autoFitChildrenVertically = true;
            UILabel label = uIPanel.Find<UILabel>("Label");
            label.text = name;
            label.maximumSize = new Vector2(700, 9999);
            label.minimumSize = new Vector2(700, 0);
            label.wordWrap = true;
            GameObject.Destroy(uIPanel.Find<UIDropDown>("Dropdown").gameObject);

            return label;

        }

        public UITextureSprite AddNamedTexture(string name)
        {
            UIPanel uIPanel = m_Root.AttachUIComponent(UITemplateManager.GetAsGameObject(UIHelperExtension.kDropdownTemplate)) as UIPanel;
            uIPanel.Find<UILabel>("Label").text = name;
            GameObject.Destroy(uIPanel.Find<UIDropDown>("Dropdown").gameObject);


            UITextureSprite uITextureSprite = uIPanel.AddUIComponent<UITextureSprite>();
            uITextureSprite.isVisible = true;
            uITextureSprite.name = "TextureSprite";
            return uITextureSprite;

        }

        public DropDownColorSelector AddColorField(string name, Color defaultValue, OnColorChanged eventCallback, OnButtonClicked eventRemove)
        {
            if (eventCallback != null && !string.IsNullOrEmpty(name))
            {
                UIPanel uIPanel = m_Root.AttachUIComponent(UITemplateManager.GetAsGameObject(UIHelperExtension.kDropdownTemplate)) as UIPanel;
                uIPanel.name = "DropDownColorSelector";
                uIPanel.Find<UILabel>("Label").text = name;
                GameObject.Destroy(uIPanel.Find<UIDropDown>("Dropdown").gameObject);
                DropDownColorSelector ddcs = new DropDownColorSelector(uIPanel, defaultValue);

                ddcs.eventColorChanged += (Color32 value) =>
                {
                    eventCallback(value);
                };

                ddcs.eventOnRemove += () =>
                {
                    eventRemove?.Invoke();
                };
                return ddcs;
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create colorPicker with no name or no event");
            return null;
        }

        public NumberedColorList AddNumberedColorList(string name, List<Color32> defaultValues, OnButtonSelect<int> eventCallback, UIComponent addButtonContainer, OnButtonClicked eventAdd)
        {
            if (eventCallback != null)
            {
                UIPanel uIPanel = m_Root.AttachUIComponent(UITemplateManager.GetAsGameObject(UIHelperExtension.kDropdownTemplate)) as UIPanel;
                uIPanel.name = "NumberedColorList";
                if (string.IsNullOrEmpty(name))
                {
                    uIPanel.Find<UILabel>("Label").text = "";
                }
                else
                {
                    uIPanel.Find<UILabel>("Label").text = name;
                }
                GameObject.Destroy(uIPanel.Find<UIDropDown>("Dropdown").gameObject);
                NumberedColorList ddcs = new NumberedColorList(uIPanel, defaultValues, addButtonContainer);

                ddcs.eventOnClick += (int value) =>
                {
                    eventCallback(value);
                };

                ddcs.eventOnAdd += () =>
                {
                    eventAdd?.Invoke();
                };
                return ddcs;
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create colorPicker with no name or no event");
            return null;
        }


        public TextList<T> AddTextList<T>(string name, Dictionary<T, string> defaultValues, OnButtonSelect<T> eventCallback, int width, int height)
        {
            if (eventCallback != null)
            {
                UIPanel uIPanel = m_Root.AttachUIComponent(UITemplateManager.GetAsGameObject(UIHelperExtension.kDropdownTemplate)) as UIPanel;
                uIPanel.name = "NumberedColorList";
                if (string.IsNullOrEmpty(name))
                {
                    uIPanel.Find<UILabel>("Label").text = "";
                }
                else
                {
                    uIPanel.Find<UILabel>("Label").text = name;
                }
                GameObject.Destroy(uIPanel.Find<UIDropDown>("Dropdown").gameObject);
                TextList<T> ddcs = new TextList<T>(uIPanel, defaultValues, width, height, name);

                ddcs.EventOnSelect += (T value) =>
                {
                    eventCallback(value);
                };

                return ddcs;
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Cannot create colorPicker with no name or no event");
            return null;
        }

    }

    public delegate void OnColorChanged(Color val);

    public delegate void OnMultipleColorChanged(List<Color32> val);
}