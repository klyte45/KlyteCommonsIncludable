using UnityEngine;

namespace Klyte.Commons.LiteUI
{
    internal static class GUIIntField
    {
        private static string lastFocusedFieldId;
        private static string lastValue;

        private static bool EnterPressed()
        {
            var keycode = Event.current.keyCode;
            return keycode == KeyCode.KeypadEnter || keycode == KeyCode.Return;
        }

        public static int IntField(string id, int value, int min = int.MinValue, int max = int.MaxValue, float? fieldWidth = 50)
        {
            var focusedFieldId = GUI.GetNameOfFocusedControl();
            GUILayoutOption widthOption = null;
            if (fieldWidth is float w)
            {
                widthOption = GUILayout.Width(w);
            }

            if (lastValue != null)
            {
                if (string.IsNullOrEmpty(focusedFieldId))
                {
                    if (id == lastFocusedFieldId)
                    {
                        if (int.TryParse(lastValue, out int val))
                        {
                            value = Mathf.Min(max, Mathf.Max(min, val));
                        }
                        lastValue = null;
                    }
                }
                else if (EnterPressed() && id == lastFocusedFieldId)
                {
                    if (int.TryParse(lastValue, out int val))
                    {
                        value = Mathf.Min(max, Mathf.Max(min, val));
                    }
                    lastValue = null;
                    GUI.FocusControl(null);
                }
                else if (lastFocusedFieldId != focusedFieldId || string.IsNullOrEmpty(focusedFieldId))
                {
                    // discard last value if user did not use enter to submit results
                    lastValue = null;
                }
            }

            if (id == focusedFieldId)
            {
                lastValue = lastValue ?? value.ToString("0");
                GUI.SetNextControlName(id);
                lastValue = GUILayout.TextField(lastValue, widthOption);
                lastFocusedFieldId = focusedFieldId;
            }
            else
            {
                GUI.SetNextControlName(id);
                GUILayout.TextField(value.ToString("0"), widthOption);
            }
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.isScrollWheel)
            {
                if (lastFocusedFieldId == focusedFieldId)
                {
                    lastValue = null;
                    GUI.FocusControl(null);
                }
                var deltaVal = (int)Mathf.Sign(Event.current.delta.y);
                if (Event.current.control)
                {
                    if (Event.current.shift)
                    {
                        deltaVal *= 100;
                    }
                }
                else if (Event.current.shift)
                {
                    deltaVal *= 10;
                }
                value = Mathf.Min(max, Mathf.Max(min, value - deltaVal));
            }

            return value;
        }
    }
}