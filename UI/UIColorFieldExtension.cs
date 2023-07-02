using ColossalFramework.UI;
using Klyte.Commons.Extensions;
using Klyte.Commons.Utils;
using Klyte.Commons.Utils.StructExtensions;
using UnityEngine;

namespace Klyte.Commons.UI
{
    public class UIColorFieldExtension : UICustomControl
    {
        private bool alreadyOnHandler = false;
        private bool supportCleaning = false;
        public void Awake()
        {
            var colorField = GetComponent<UIColorField>();
            if (!colorField)
            {
                return;
            }
            if (GetComponents<UIColorFieldExtension>().Length > 1 && GetComponents<UIColorFieldExtension>()[0] != this)
            {
                Destroy(this);
                return;
            }
            colorField.eventColorPickerOpen += StartColorPicker;
            colorField.eventColorPickerOpen += BindColorPickerExtraEvents;
        }

        private void BindColorPickerExtraEvents(UIColorField colorField, UIColorPicker popup, ref bool overridden)
        {
            var textField = popup.Find<UITextField>("ColorText");
            var cleanButton = popup.Find<UIButton>("CleanUpColor");

            textField.text = ((Color32)popup.color).ToRGB();
            textField.eventTextChanged += (x, y) =>
            {
                if (Event.current.isKey && !alreadyOnHandler)
                {
                    try
                    {
                        alreadyOnHandler = true;
                        if (popup && textField.text.Length == 6)
                        {
                            try
                            {
                                Color32 targetColor = ColorExtensions.FromRGB(((UITextField)x).text);
                                if (popup.color != targetColor)
                                {
                                    popup.color = targetColor;
                                    var selStart = ((UITextField)x).selectionStart;
                                    var selEnd = ((UITextField)x).selectionEnd;
                                    colorField.selectedColor = targetColor;
                                    ((UITextField)x).textColor = Color.white;
                                    ((UITextField)x).text = targetColor.ToRGB();
                                    colorField.GetType().GetMethod("OnSelectedColorChanged", Patcher.allFlags).Invoke(colorField, new object[0]);
                                    ((UITextField)x).selectionStart = selStart;
                                    ((UITextField)x).selectionEnd = selEnd;
                                }
                            }
                            catch
                            {
                                ((UITextField)x).textColor = Color.red;
                            }
                        }
                        else
                        {
                            ((UITextField)x).textColor = Color.red;
                        }
                    }
                    finally
                    {
                        alreadyOnHandler = false;
                    }
                }
            };

            popup.eventColorUpdated += (x) => textField.text = ((Color32)x).ToRGB();
            cleanButton.eventClicked += (x, y) =>
            {
                colorField.selectedColor = Color.clear;
                textField.text = "";
            };
            cleanButton.isVisible = supportCleaning;
        }

        private void StartColorPicker(UIColorField colorField, UIColorPicker popup, ref bool overridden)
        {
            UIPanel panel = popup.GetComponent<UIPanel>();
            panel.height = 250;
            KlyteMonoUtils.CreateUIElement(out UITextField textField, panel.transform, "ColorText", new Vector4(15, 225, 200, 20));
            KlyteMonoUtils.UiTextFieldDefaults(textField);
            textField.normalBgSprite = "TextFieldPanel";
            textField.maxLength = 6;
            KlyteMonoUtils.InitCircledButton(panel, out UIButton clearButton, "Niet", (x, y) =>
            {
                colorField.selectedColor = Color.clear;
                textField.text = "";
            }, "CleanUpColor", 20);
            clearButton.relativePosition = new Vector3(220, 225);
            clearButton.color = Color.red;

            colorField.colorPicker = Instantiate(popup);

            colorField.eventColorPickerOpen -= StartColorPicker;
        }

        public void SetSupportCleaning(bool newVal) => supportCleaning = newVal;
    }
}
