using ColossalFramework.Math;
using UnityEngine;

namespace Klyte.Commons.Utils.StructExtensions
{
    public static class Vector3Extensions
    {
        public static float GetAngleXZ(this Vector3 dir) => Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        public static float SqrDistance(this Vector3 a, Vector3 b)
        {
            Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            return (vector.x * vector.x) + (vector.y * vector.y) + (vector.z * vector.z);
        }

        public static Segment3 ToRayY(this Vector3 vector) => new Segment3(new Vector3(vector.x, -999999f, vector.z), new Vector3(vector.x, 999999f, vector.z));

        public static Vector3 MakeFlat(this Vector3 v) => new Vector3(v.x, 0f, v.z);
        public static Vector3 SetHeight(this Vector3 v, float height) => new Vector3(v.x, height, v.z);
        public static Vector3 AddHeight(this Vector3 v, float deltaHeight) => new Vector3(v.x, v.y + deltaHeight, v.z);
        public static Vector3 MakeFlatNormalized(this Vector3 v) => new Vector3(v.x, 0f, v.z).normalized;
        public static Vector3 Turn90(this Vector3 v, bool isClockWise) => isClockWise ? new Vector3(v.z, v.y, -v.x) : new Vector3(-v.z, v.y, v.x);
        public static Vector3 TurnDeg(this Vector3 vector, float turnAngle, bool isClockWise) => vector.TurnRad(turnAngle * Mathf.Deg2Rad, isClockWise);
        public static Vector3 TurnRad(this Vector3 vector, float turnAngle, bool isClockWise)
        {
            turnAngle = isClockWise ? -turnAngle : turnAngle;
            var newX = vector.x * Mathf.Cos(turnAngle) - vector.z * Mathf.Sin(turnAngle);
            var newZ = vector.x * Mathf.Sin(turnAngle) + vector.z * Mathf.Cos(turnAngle);
            return new Vector3(newX, vector.y, newZ);
        }
    }
}
