using Unity.Entities;

namespace Fps
{
    public struct PickupRespawnDelay : IComponentData
    {
        public float RechargeTime;
    }
}
