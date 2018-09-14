using UnityEngine;
using Unity.Entities;

namespace Improbable.Gdk.Interaction
{
    public struct AttemptInteractRaycast : IComponentData
    {
        public Vector3 Position;
        public Vector3 RaycastDirection;
        public float Radius;
    }
}
