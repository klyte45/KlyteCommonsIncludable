using UnityEngine;

namespace Klyte.Commons.LiteUI.BaseElements
{
    internal static class GUITextField
    {
        private static string lastFocusedFieldId;
        private static string lastValue;

        private static bool EnterPressed()
        {
            var keycode = Event.current.keyCode;
            return keycode == KeyCode.KeypadEnter || keycode == KeyCode.Return;
        }

        public static string TextField(string id, string value, bool saveOnUnfocus = true)
        {
            var focusedFieldId = GUI.GetNameOfFocusedControl();

            if (lastValue != null)
            {
                if (string.IsNullOrEmpty(focusedFieldId) && (saveOnUnfocus || EnterPressed()))
                {
                    if (id == lastFocusedFieldId)
                    {
                        value = lastValue;
                        lastValue = null;
                    }
                }
                else if (lastFocusedFieldId != focusedFieldId || string.IsNullOrEmpty(focusedFieldId))
                {
                    // discard last value if user did not use enter to submit results
                    lastValue = null;
                }
                else if (EnterPressed())
                {
                    value = lastValue;
                    lastValue = null;
                    GUI.FocusControl(null);
                }
            }

            if (id == focusedFieldId)
            {
                lastValue = lastValue ?? value.ToString();
                GUI.SetNextControlName(id);
                lastValue = GUILayout.TextField(lastValue);
                lastFocusedFieldId = focusedFieldId;
            }
            else
            {
                GUI.SetNextControlName(id);
                GUILayout.TextField(value.ToString());
            }

            return value;
        }
    }
}