using System;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public static class ColorExtensions
    {
        public static string ToRGBA(this Color32 color) => color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
        public static string ToRGB(this Color32 color) => color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        public static string ToRGBA(this Color color) => ToRGBA((Color32)color);
        public static string ToRGB(this Color color) => ToRGB((Color32)color);
        public static Color SetBrightness(this Color color, float brightness)
        {
            Color.RGBToHSV(color, out float hue, out float saturation, out _);
            return Color.HSVToRGB(hue, saturation, brightness);
        }
        public static Color ClampSaturation(this Color color, float maxSaturation)
        {
            Color.RGBToHSV(color, out float hue, out float saturation, out float brightness);
            return Color.HSVToRGB(hue, Mathf.Min(saturation, maxSaturation), brightness);
        }
        public static Color MultiplyChannelsButAlpha(this Color color, Color other) => new Color(color.r * other.r, color.g * other.g, color.b * other.b, color.a);
        public static Color32 FromRGBA(string rgba)
        {
            long value = Convert.ToInt64(rgba, 16);
            return FromRGBA(value);
        }

        public static Color32 FromRGBA(long value) => new Color32((byte)((value & 0xFF000000) >> 24), (byte)((value & 0xFF0000) >> 16), (byte)((value & 0xFF00) >> 8), (byte)((value & 0xFF)));

        public static Color32 FromRGB(string rgb)
        {
            int value = Convert.ToInt32(rgb, 16);
            return FromRGB(value);
        }

        public static Color32 FromRGB(int value) => new Color32((byte)((value & 0xFF0000) >> 16), (byte)((value & 0xFF00) >> 8), (byte)((value & 0xFF)), 0xFF);
    }
}
