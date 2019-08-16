using Improbable.Gdk.Standardtypes;
using UnityEngine;

namespace Improbable.Gdk.StandardTypes
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

        public static IntAbsolute ToIntAbsolute(this Vector3 value)
        {
            return new IntAbsolute
            {
                X = value.x.ToInt1k(),
                Y = value.y.ToInt1k(),
                Z = value.z.ToInt1k()
            };
        }

        public static Vector3 ToVector3(this IntAbsolute value)
        {
            return new Vector3(value.X.ToFloat1k(), value.Y.ToFloat1k(), value.Z.ToFloat1k());
        }

        public static IntDelta ToIntDelta(this Vector3 value)
        {
            return new IntDelta
            {
                X = value.x.ToInt1k(),
                Y = value.y.ToInt1k(),
                Z = value.z.ToInt1k()
            };
        }

        public static Vector3 ToVector3(this IntDelta value)
        {
            return new Vector3(value.X.ToFloat1k(), value.Y.ToFloat1k(), value.Z.ToFloat1k());
        }
    }
}
