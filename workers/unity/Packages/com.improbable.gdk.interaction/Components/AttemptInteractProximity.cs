using UnityEngine;
using Unity.Entities;

namespace Improbable.Gdk.Interaction
{
    public struct AttemptInteractProximity : IComponentData
    {
        public Vector3 Position;
        public float Radius;
    }
}
