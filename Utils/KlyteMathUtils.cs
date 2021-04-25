using ColossalFramework.Math;
using UnityEngine;

namespace Klyte.Commons.Utils
{
    public class KlyteMathUtils
    {
        #region Math Utils

        public static float CalcBezierLenght(Bezier3 bz, float precision = 0.5f) => CalcBezierLenght(bz.a, bz.b, bz.c, bz.d, precision);
        public static float CalcBezierLenght(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float precision)
        {

            Vector3 aa = (-a + 3 * (b - c) + d);
            Vector3 bb = 3 * (a + c) - 6 * b;
            Vector3 cc = 3 * (b - a);

            int len = (int)(1.0f / precision);
            float[] arcLengths = new float[len + 1];
            arcLengths[0] = 0;

            Vector3 ov = a;
            Vector3 v;
            float clen = 0.0f;
            for (int i = 1; i <= len; i++)
            {
                float t = (i * precision);
                v = ((aa * t + (bb)) * t + cc) * t + a;
                clen += (ov - v).magnitude;
                arcLengths[i] = clen;
                ov = v;
            }
            return clen;

        }

        public static Vector2 RadianToVector2(float radian) => new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

        public static Vector2 DegreeToVector2(float degree) => RadianToVector2(degree * Mathf.Deg2Rad);

        private const ulong DeBruijnSequence = 0x37E84A99DAE458F;

        private static readonly int[] MultiplyDeBruijnBitPosition =
        {
        0, 1, 17, 2, 18, 50, 3, 57,
        47, 19, 22, 51, 29, 4, 33, 58,
        15, 48, 20, 27, 25, 23, 52, 41,
        54, 30, 38, 5, 43, 34, 59, 8,
        63, 16, 49, 56, 46, 21, 28, 32,
        14, 26, 24, 40, 53, 37, 42, 7,
        62, 55, 45, 31, 13, 39, 36, 6,
        61, 44, 12, 35, 60, 11, 10, 9,
    };

        /// <summary>
        /// Search the mask data from least significant bit (LSB) to the most significant bit (MSB) for a set bit (1)
        /// using De Bruijn sequence approach. Warning: Will return zero for b = 0.
        /// </summary>
        /// <param name="b">Target number.</param>
        /// <returns>Zero-based position of LSB (from right to left).</returns>
        public static int BitScanForward(ulong b) => b == 0 ? -1 : MultiplyDeBruijnBitPosition[((ulong)((long)b & -(long)b) * DeBruijnSequence) >> 58];
        #endregion
    }
}
