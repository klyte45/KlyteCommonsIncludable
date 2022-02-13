using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Klyte.Commons.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace Klyte.Commons.LiteUI
{
    public static class GUIKlyteCommons
    {
        public const string v_null = "<color=#FF00FF>--NULL--</color>";
        public const string v_empty = "<color=#888888>--EMPTY--</color>";
        public static readonly Texture2D darkGreenTexture;
        public static readonly Texture2D greenTexture;
        public static readonly Texture2D darkRedTexture;
        public static readonly Texture2D redTexture;

        static GUIKlyteCommons()
        {
            darkGreenTexture = CreateTextureOfColor(Color.Lerp(Color.green, Color.gray, 0.5f));
            greenTexture = CreateTextureOfColor(Color.green);
            darkRedTexture = CreateTextureOfColor(Color.Lerp(Color.red, Color.gray, 0.5f));
            redTexture = CreateTextureOfColor(Color.red);
        }

        private static Texture2D CreateTextureOfColor(Color src)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, src);
            texture.Apply();
            return texture;
        }
        public static Texture GetByNameFromDefaultAtlas(string name) => UIView.GetAView().defaultAtlas.sprites.Where(x => x.name == name).FirstOrDefault().texture;

        public static bool AddVector3Field(Vector3Xml input, string i18nEntry, string baseFieldName)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(Locale.Get(i18nEntry));
                GUILayout.FlexibleSpace();

                var x = GUIFloatField.FloatField(baseFieldName + "X", input.X);
                var y = GUIFloatField.FloatField(baseFieldName + "Y", input.Y);
                var z = GUIFloatField.FloatField(baseFieldName + "Z", input.Z);
                var changed = x != input.X || y != input.Y || z != input.Z;
                input.X = x;
                input.Y = y;
                input.Z = z;
                return changed;
            };
        }

        public static bool AddVector3Field(ref Vector3 input, string i18nEntry, string baseFieldName)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(Locale.Get(i18nEntry));
                GUILayout.FlexibleSpace();
                var x = GUIFloatField.FloatField(baseFieldName + "X", input.x);
                var y = GUIFloatField.FloatField(baseFieldName + "Y", input.y);
                var z = GUIFloatField.FloatField(baseFieldName + "Z", input.z);
                var changed = x != input.x || y != input.y || z != input.z;
                input.x = x;
                input.y = y;
                input.z = z;
                return changed;
            };
        }

        #region Utility UI structures
        public static void ButtonSelector(float totalWidth, string label, string buttonText, Action action)
        {
            using (new GUILayout.HorizontalScope(GUILayout.Width(totalWidth - 10)))
            {
                GUILayout.Label(label, GUILayout.Width(totalWidth / 3));
                if (buttonText == "")
                {
                    buttonText = v_empty;
                }
                if (GUILayout.Button(buttonText ?? v_null))
                {
                    action();
                }
            };
        }
        public static bool CreateItemVerticalList(Rect sideListArea, ref Vector2 scrollPosition, int currentSelection, string[] sideList, string addButtonText, GUIStyle addButtonStyle, out int newSelection)
        {
            var result = false;
            using (new GUILayout.AreaScope(sideListArea))
            {
                using (var scroll = new GUILayout.ScrollViewScope(scrollPosition))
                {
                    newSelection = currentSelection;
                    var newListSel = GUILayout.SelectionGrid(currentSelection, sideList, 1, new GUIStyle(GUI.skin.button) { wordWrap = true });
                    if (newListSel >= 0 && newListSel < sideList.Length)
                    {
                        newSelection = newListSel;
                    }

                    if (GUILayout.Button(addButtonText, addButtonStyle, GUILayout.ExpandWidth(true)))
                    {
                        result = true;
                        newSelection = sideList.Length;
                    }
                    scrollPosition = scroll.scrollPosition;
                }
            }
            return result;
        }

        public static void SquareTextureButton(Texture2D icon, string tooltip, Action onClick, bool condition = true)
        {
            if (condition && GUILayout.Button(new GUIContent(icon, tooltip), GUILayout.Width(30), GUILayout.Height(30)))
            {
                onClick();
            }
        }
        #endregion

        [Obsolete("Use Scope", true)]
        public static void DoInArea(Rect size, Action<Rect> action)
        {
            GUILayout.BeginArea(size);
            try
            {
                action(size);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                GUILayout.EndArea();
            }
        }
        [Obsolete("Use Scope", true)]
        public static void DoInScroll(ref Vector2 scrollPos, Action action)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            try
            {
                action();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                GUILayout.EndScrollView();
            }
        }
        [Obsolete("Use Scope", true)]
        public static void DoInScroll(ref Vector2 scrollPos, bool alwaysShowHorizontal, bool alwaysShowVertical, Action action, params GUILayoutOption[] options)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, alwaysShowHorizontal, alwaysShowVertical, options);
            try
            {
                action();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                GUILayout.EndScrollView();
            }
        }
        [Obsolete("Use Scope", true)]
        public static void DoInHorizontal(Action action, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
            try
            {
                action();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }
        [Obsolete("Use Scope", true)]
        public static void DoInVertical(Action action, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
            try
            {
                action();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                GUILayout.EndVertical();
            }
        }
    }
}