using Unity.Entities;

namespace Improbable.Gdk.Interaction
{
    public struct TriggeredInteract : IComponentData
    {
        public long TargetEntityId;
    }
}
