using Klyte.Commons.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klyte.Commons.LiteUI
{
    internal sealed class ColorPicker : GUIWindow, IGameObject
    {
        private readonly Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();
        private readonly Color LineColor = Color.white;


        private readonly int colorPickerSize = 142;
        private readonly int hueBarWidth = 26;

        private Texture2D colorPickerTexture;
        private Texture2D hueBarTexture;
        private ColorUtil.HSV currentHSV;
        private float originalAlpha;

        private Rect colorPickerRect;
        private Rect hueBarRect;

        private Texture2D lineTexTexture;

        public ColorPicker()
            : base("ColorPicker", new Rect(16.0f, 16.0f, 188.0f, 156.0f), false, false)
        {
            colorPickerRect = new Rect(8.0f, 8.0f, colorPickerSize, colorPickerSize);
            hueBarRect = new Rect(colorPickerRect.x + colorPickerSize + 4.0f, colorPickerRect.y, hueBarWidth, colorPickerRect.height);
        }

        public string CurrentValueId { get; private set; }

        public Color SelectedColor
        {
            get
            {
                var result = ColorUtil.HSV.HSV2RGB(currentHSV);
                result.a = originalAlpha;
                return result;
            }
        }

        private Texture2D Texture => colorPickerTexture ?? (colorPickerTexture = new Texture2D(colorPickerSize, colorPickerSize));

        private Texture2D HueBar => hueBarTexture ?? (hueBarTexture = DrawHueBar(hueBarWidth, colorPickerSize));

        private Texture2D LineTex => lineTexTexture ?? (lineTexTexture = DrawLineTex());

        public Texture2D GetColorTexture(string id, Color color)
        {
            if (!TextureCache.TryGetValue(id, out var texture))
            {
                texture = new Texture2D(1, 1);
                TextureCache.Add(id, texture);
            }

            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        public void Show(string id, Color initialColor)
        {
            CurrentValueId = id;
            originalAlpha = initialColor.a;
            currentHSV = ColorUtil.HSV.RGB2HSV(initialColor);
            currentHSV.H = 360.0f - currentHSV.H;
            UpdateColorTexture();

            Vector2 mouse = Input.mousePosition;
            mouse.y = Screen.height - mouse.y;

            mouse.x -= WindowRect.width;
            var windowRect = WindowRect;
            windowRect.position = mouse;
            MoveResize(windowRect);
            Visible = true;
        }

        public void Update()
        {
            Vector2 mouse = Input.mousePosition;
            mouse.y = Screen.height - mouse.y;

            if (Input.GetMouseButton(0))
            {
                if (!WindowRect.Contains(mouse))
                {
                    Visible = false;
                    return;
                }
            }
            else
            {
                return;
            }

            mouse -= WindowRect.position;

            if (hueBarRect.Contains(mouse))
            {
                currentHSV.H = (1.0f - Mathf.Clamp01((mouse.y - hueBarRect.y) / hueBarRect.height)) * 360.0f;
                UpdateColorTexture();
            }

            if (colorPickerRect.Contains(mouse))
            {
                currentHSV.S = Mathf.Clamp01((mouse.x - colorPickerRect.x) / colorPickerRect.width);
                currentHSV.V = Mathf.Clamp01((mouse.y - colorPickerRect.y) / colorPickerRect.height);
            }
        }

        protected override void HandleException(Exception ex)
        {
            LogUtils.DoErrorLog("Exception in ColorPicker - " + ex.Message);
            Visible = false;
        }

        protected override void DrawWindow()
        {
            GUI.DrawTexture(colorPickerRect, Texture);
            GUI.DrawTexture(hueBarRect, HueBar);

            var hueBarLineY = hueBarRect.y + (1.0f - (float)currentHSV.H / 360.0f) * hueBarRect.height;
            GUI.DrawTexture(new Rect(hueBarRect.x - 2.0f, hueBarLineY, hueBarRect.width + 4.0f, 2.0f), LineTex);

            var colorPickerLineY = colorPickerRect.x + (float)currentHSV.V * colorPickerRect.width;
            GUI.DrawTexture(new Rect(colorPickerRect.x - 1.0f, colorPickerLineY, colorPickerRect.width + 2.0f, 1.0f), LineTex);

            var colorPickerLineX = colorPickerRect.y + (float)currentHSV.S * colorPickerRect.height;
            GUI.DrawTexture(new Rect(colorPickerLineX, colorPickerRect.y - 1.0f, 1.0f, colorPickerRect.height + 2.0f), LineTex);
        }

        private static Texture2D DrawHueBar(int width, int height)
        {
            var texture = new Texture2D(width, height);

            for (var y = 0; y < height; y++)
            {
                var color = GetColorAtT(y / (float)height * 360.0f);

                for (var x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        private Texture2D DrawLineTex()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, LineColor);
            tex.Apply();
            return tex;
        }

        private static Color GetColorAtT(double hue)
            => ColorUtil.HSV.HSV2RGB(new ColorUtil.HSV { H = hue, S = 1.0f, V = 1.0f });

        private static Color GetColorAtXY(double hue, float xT, float yT)
            => ColorUtil.HSV.HSV2RGB(new ColorUtil.HSV { H = hue, S = xT, V = yT });

        private void UpdateColorTexture()
        {
            for (var x = 0; x < Texture.width; x++)
            {
                for (var y = 0; y < Texture.height; y++)
                {
                    Texture.SetPixel(x, y, GetColorAtXY(currentHSV.H, x / (float)Texture.width, 1.0f - (y / (float)Texture.height)));
                }
            }

            Texture.Apply();
        }

        public Color PresentColor(string id, Color value, bool enabled, bool useAlpha = false)
        {

            if (enabled)
            {
                var r = (int)Mathf.Clamp(value.r * 255.0f, byte.MinValue, byte.MaxValue);
                var g = (int)Mathf.Clamp(value.g * 255.0f, byte.MinValue, byte.MaxValue);
                var b = (int)Mathf.Clamp(value.b * 255.0f, byte.MinValue, byte.MaxValue);
                var a = useAlpha ? (int)Mathf.Clamp(value.a * 255.0f, byte.MinValue, byte.MaxValue) : 255;

                r = GUIIntField.IntField(id + ".r", r, fieldWidth: 30);
                g = GUIIntField.IntField(id + ".g", g, fieldWidth: 30);
                b = GUIIntField.IntField(id + ".b", b, fieldWidth: 30);
                if (useAlpha)
                {
                    a = GUIIntField.IntField(id + ".a", a, fieldWidth: 30);
                }

                value.r = Mathf.Clamp01(r / 255.0f);
                value.g = Mathf.Clamp01(g / 255.0f);
                value.b = Mathf.Clamp01(b / 255.0f);
                value.a = Mathf.Clamp01(a / 255.0f);

                if (GUILayout.Button(string.Empty, GUILayout.MinWidth(20), GUILayout.MaxWidth(80)))
                {
                    Show(id, value);
                }
                else
                {
                    if (Visible && CurrentValueId == id)
                    {
                        value = SelectedColor;
                    }
                }
            }
            else
            {
                GUILayout.Box(string.Empty, GUILayout.MinWidth(useAlpha ? 90 : 60), GUILayout.MaxWidth(100));
            }


            var lastRect = GUILayoutUtility.GetLastRect();
            var colorRect = lastRect;
            colorRect.x += 4.0f;
            colorRect.y += 4.0f;
            colorRect.width -= 8.0f;
            colorRect.height -= 8.0f;
            lastRect.x += 4.0f;
            lastRect.width -= 8.0f;
            GUI.DrawTexture(colorRect, GetColorTexture(id, value), ScaleMode.StretchToFill);
            GUI.Label(lastRect, "#" + (useAlpha ? value.ToRGBA() : value.ToRGB()), new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = new GUIStyleState()
                {
                    textColor = value.ContrastColor()
                }
            });

            return value;
        }
    }
}