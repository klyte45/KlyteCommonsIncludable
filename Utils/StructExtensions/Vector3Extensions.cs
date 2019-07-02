using UnityEngine;

namespace Klyte.Commons.Utils
{
    public static class Vector3Extensions
    {
        public static float GetAngleXZ(this Vector3 dir)
        {
            return Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        }
        public static float SqrDistance(this Vector3 a, Vector3 b)
        {
            Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            return (vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }
    }
}
