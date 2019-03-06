using Improbable.Gdk.Core;
using Improbable.Gdk.Health;
using Unity.Collections;
using Unity.Entities;

namespace Improbable.Gdk.Guns
{
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    public class ServerShootingSystem : ComponentSystem
    {
        private struct PlayersShooting
        {
            public readonly int Length;
            public ComponentDataArray<HealthComponent.CommandSenders.ModifyHealth> ModifyHealthCommandSenders;
            [ReadOnly] public ComponentDataArray<SpatialEntityId> EntityId;
            [ReadOnly] public ComponentDataArray<ShootingComponent.ReceivedEvents.Shots> Shots;
            [ReadOnly] public ComponentDataArray<GunComponent.Component> Gun;
        }

        [Inject] private PlayersShooting playersShooting;

        protected override void OnUpdate()
        {
            for (var i = 0; i < playersShooting.Length; i++)
            {
                var commandSender = playersShooting.ModifyHealthCommandSenders[i];
                var commandSent = false;
                var gunId = playersShooting.Gun[i].GunId;
                var gunSettings = GunDictionary.Get(gunId);
                var damage = gunSettings.ShotDamage;

                foreach (var shot in playersShooting.Shots[i].Events)
                {
                    var shotInfo = shot;
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
                    commandSender.RequestsToSend.Add(modifyHealthRequest);
                    commandSent = true;
                }

                if (commandSent)
                {
                    playersShooting.ModifyHealthCommandSenders[i] = commandSender;
                }
            }
        }

        private bool ValidateShot(ShotInfo shot)
        {
            return shot.HitSomething && shot.EntityId > 0;
        }
    }
}
