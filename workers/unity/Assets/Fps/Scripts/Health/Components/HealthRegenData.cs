using Unity.Entities;

namespace Fps.Health
{
    public struct HealthRegenData : IComponentData
    {
        // Timers used for health regeneration
        public float DamagedRecentlyTimer;
        public float NextSpatialSyncTimer;
        public float NextRegenTimer;
    }
}
