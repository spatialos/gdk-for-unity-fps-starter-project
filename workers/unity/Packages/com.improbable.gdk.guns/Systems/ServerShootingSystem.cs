using Improbable.Gdk.Core;
using Improbable.Gdk.Health;
using Unity.Collections;
using Unity.Entities;

namespace Improbable.Gdk.Guns
{
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    public class ServerShootingSystem : ComponentSystem
    {
        private struct PlayersShooting
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<SpatialEntityId> EntityId;
            [ReadOnly] public ComponentDataArray<GunComponent.Component> Gun;
            [ReadOnly] public EntityArray Entities;
        }

        [Inject] private PlayersShooting playersShooting;
        [Inject] private CommandSystem commandSystem;
        [Inject] private ComponentUpdateSystem updateSystem;

        protected override void OnUpdate()
        {
            for (var i = 0; i < playersShooting.Length; i++)
            {
                var gunId = playersShooting.Gun[i].GunId;
                var gunSettings = GunDictionary.Get(gunId);
                var damage = gunSettings.ShotDamage;

                var events =
                    updateSystem.GetEventsReceived<ShootingComponent.Shots.Event>(playersShooting.EntityId[i].EntityId);
                for (int j = 0; j < events.Count; ++j)
                {
                    ref readonly var shotInfo = ref events[j].Event.Payload;
                    if (!ValidateShot(shotInfo))
                    {
                        continue;
                    }

                    // Send command to entity being shot.
                    var modifyHealthRequest = new HealthComponent.ModifyHealth.Request(
                        new EntityId(shotInfo.EntityId),
                        new HealthModifier()
                        {
                            Amount = -damage,
                            Origin = shotInfo.HitOrigin,
                            AppliedLocation = shotInfo.HitLocation
                        });
                    commandSystem.SendCommand(modifyHealthRequest, playersShooting.Entities[i]);
                }
            }
        }

        private bool ValidateShot(ShotInfo shot)
        {
            return shot.HitSomething && shot.EntityId > 0;
        }
    }
}
