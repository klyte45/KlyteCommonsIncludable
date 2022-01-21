using System;
using UnityEngine;

namespace Klyte.Commons.LiteUI
{
    internal static class ColorUtil
    {
        public struct HSV
        {
            public double H;
            public double S;
            public double V;

            public static HSV RGB2HSV(Color color) => RGB2HSV((int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));

            public static HSV RGB2HSV(double r, double b, double g)
            {
                double delta;
                double min;
                double h = 0;
                double s;
                double v;

                min = Math.Min(Math.Min(r, g), b);
                v = Math.Max(Math.Max(r, g), b);
                delta = v - min;

                s = v == 0.0 ? 0 : delta / v;

                if (s == 0)
                {
                    h = 0.0f;
                }
                else
                {
                    if (r == v)
                    {
                        h = (g - b) / delta;
                    }
                    else if (g == v)
                    {
                        h = 2 + (b - r) / delta;
                    }
                    else if (b == v)
                    {
                        h = 4 + (r - g) / delta;
                    }

                    h *= 60;
                    if (h < 0.0)
                    {
                        h += 360;
                    }
                }

                return new HSV
                {
                    H = h,
                    S = s,
                    V = v / 255,
                };
            }

            public static Color HSV2RGB(HSV color) => HSV2RGB(color.H, color.S, color.V);

            public static Color HSV2RGB(double h, double s, double v)
            {
                double r;
                double g;
                double b;

                if (s == 0)
                {
                    r = v;
                    g = v;
                    b = v;
                }
                else
                {
                    int i;
                    double f;
                    double p;
                    double q;
                    double t;

                    if (h == 360)
                    {
                        h = 0;
                    }
                    else
                    {
                        h /= 60;
                    }

                    i = (int)h;
                    f = h - i;

                    p = v * (1.0 - s);
                    q = v * (1.0 - s * f);
                    t = v * (1.0 - s * (1.0f - f));

                    switch (i)
                    {
                        case 0:
                            r = v;
                            g = t;
                            b = p;
                            break;

                        case 1:
                            r = q;
                            g = v;
                            b = p;
                            break;

                        case 2:
                            r = p;
                            g = v;
                            b = t;
                            break;

                        case 3:
                            r = p;
                            g = q;
                            b = v;
                            break;

                        case 4:
                            r = t;
                            g = p;
                            b = v;
                            break;

                        default:
                            r = v;
                            g = p;
                            b = q;
                            break;
                    }
                }

                return new Color((float)r, (float)g, (float)b, 1);
            }

            public override string ToString() => $"H: {H.ToString("0.00")}, S: {S.ToString("0.00")}, V:{V.ToString("0.00")}";
        }
    }
}