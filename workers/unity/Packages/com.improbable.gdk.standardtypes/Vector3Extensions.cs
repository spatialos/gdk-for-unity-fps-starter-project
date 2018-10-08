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

        public static Vector3 ToUnityVector3(this Vector3f vector3f)
        {
            return new Vector3(vector3f.X, vector3f.Y, vector3f.Z);
        }

        public static Vector3f ToSpatialVector3f(this Vector3 unityVector3)
        {
            return new Vector3f
            {
                X = unityVector3.x,
                Y = unityVector3.y,
                Z = unityVector3.z
            };
        }

        public static Vector3d ToSpatialVector3d(this Vector3 unityVector3)
        {
            return new Vector3d
            {
                X = unityVector3.x,
                Y = unityVector3.y,
                Z = unityVector3.z
            };
        }

        public static Coordinates ToSpatialCoordinates(this Vector3 unityVector3)
        {
            return new Coordinates
            {
                X = unityVector3.x,
                Y = unityVector3.y,
                Z = unityVector3.z
            };
        }
    }
}
