using UnityEngine;

namespace Klyte.Commons.Utils
{
    public static class Vector2Extensions
    {
        public static float GetAngleToPoint(this Vector2 from, Vector2 to)
        {
            float ca = to.x - from.x;
            float co = -to.y + from.y;
            //LogUtils.DoLog($"ca = {ca},co = {co};");

            return co < 0
                ? 360 - (((Mathf.Atan2(ca, co) * Mathf.Rad2Deg) + 360) % 360)
                : 360 - (((Mathf.Atan2(ca, co) * Mathf.Rad2Deg) + 180 + 360) % 360);
        }
    }
}
