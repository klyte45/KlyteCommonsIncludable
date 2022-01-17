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
        public static void AddVector3Field(Vector3Xml input, string i18nEntry, string baseFieldName)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(Locale.Get(i18nEntry));
                GUILayout.FlexibleSpace();
                input.X = GUIFloatField.FloatField(baseFieldName + "X", input.X);
                input.Y = GUIFloatField.FloatField(baseFieldName + "Y", input.Y);
                input.Z = GUIFloatField.FloatField(baseFieldName + "Z", input.Z);
            };
        }

        public static void AddVector3Field(ref Vector3 input, string i18nEntry, string baseFieldName)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(Locale.Get(i18nEntry));
                GUILayout.FlexibleSpace();
                input.x = GUIFloatField.FloatField(baseFieldName + "X", input.x);
                input.y = GUIFloatField.FloatField(baseFieldName + "Y", input.y);
                input.z = GUIFloatField.FloatField(baseFieldName + "Z", input.z);
            };
        }

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