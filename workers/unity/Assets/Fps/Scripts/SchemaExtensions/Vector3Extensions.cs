using UnityEngine;

namespace Fps.SchemaExtensions
{
    public static class Vector3Extensions
    {
        public static int ToInt1k(this float value)
        {
            return Mathf.RoundToInt(value * 100000);
        }

        public static float ToFloat1k(this int value)
        {
            return ((float) value) / 100000;
        }

        public static Vector3Int ToVector3Int(this Vector3 value)
        {
            return new Vector3Int
            {
                X = value.x.ToInt1k(),
                Y = value.y.ToInt1k(),
                Z = value.z.ToInt1k()
            };
        }

        public static Vector3 ToVector3(this Vector3Int value)
        {
            return new Vector3(value.X.ToFloat1k(), value.Y.ToFloat1k(), value.Z.ToFloat1k());
        }
    }
}
