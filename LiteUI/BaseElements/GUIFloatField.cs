using UnityEngine;

namespace Klyte.Commons.LiteUI.BaseElements
{
    internal static class GUIFloatField
    {
        private static string lastFocusedFieldId;
        private static string lastValue;

        private static bool EnterPressed()
        {
            var keycode = Event.current.keyCode;
            return keycode == KeyCode.KeypadEnter || keycode == KeyCode.Return;
        }

        public static float FloatField(string id, float value, float min = float.MinValue, float max = float.MaxValue)
        {
            var focusedFieldId = GUI.GetNameOfFocusedControl();

            if (lastValue != null)
            {
                if (string.IsNullOrEmpty(focusedFieldId))
                {
                    if (id == lastFocusedFieldId)
                    {
                        if (float.TryParse(lastValue.Replace(",", "."), out float val))
                        {
                            value = Mathf.Min(max, Mathf.Max(min, val));
                        }
                        lastValue = null;
                    }
                }
                else if (EnterPressed() && id == lastFocusedFieldId)
                {
                    if (float.TryParse(lastValue.Replace(",", "."), out float val))
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
                lastValue = lastValue ?? value.ToString("F3");
                GUI.SetNextControlName(id);
                lastValue = GUILayout.TextField(lastValue, GUILayout.Width(50));
                lastFocusedFieldId = focusedFieldId;
            }
            else
            {
                GUI.SetNextControlName(id);
                GUILayout.TextField(value.ToString("F3"), GUILayout.Width(50));
            }
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.isScrollWheel)
            {
                if (lastFocusedFieldId == focusedFieldId)
                {
                    lastValue = null;
                    GUI.FocusControl(null);
                }
                var deltaVal = Mathf.Sign(Event.current.delta.y);
                if (Event.current.alt)
                {
                    if (Event.current.control)
                    {
                        deltaVal *= 0.001f;
                    }
                    else
                    {
                        deltaVal *= 0.01f;
                    }
                }
                else if (Event.current.control)
                {
                    if (Event.current.shift)
                    {
                        deltaVal *= 100f;
                    }
                    else
                    {
                        deltaVal *= 0.1f;
                    }
                }
                else if (Event.current.shift)
                {
                    deltaVal *= 10f;
                }
                value = Mathf.Min(max, Mathf.Max(min, value - deltaVal));
            }

            return value;
        }
    }
}