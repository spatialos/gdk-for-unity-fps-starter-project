using Improbable.Gdk.Core;
using Improbable.Gdk.Health;
using Unity.Entities;

namespace Improbable.Gdk.Guns
{
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    public class ServerShootingSystem : ComponentSystem
    {
        private WorkerSystem workerSystem;
        private CommandSystem commandSystem;
        private ComponentUpdateSystem componentUpdateSystem;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            workerSystem = World.GetExistingManager<WorkerSystem>();
            commandSystem = World.GetExistingManager<CommandSystem>();
            componentUpdateSystem = World.GetExistingManager<ComponentUpdateSystem>();
        }

        protected override void OnUpdate()
        {
            var events = componentUpdateSystem.GetEventsReceived<ShootingComponent.Shots.Event>();
            var gunDataForEntity = GetComponentDataFromEntity<GunComponent.Component>();

            for (var i = 0; i < events.Count; ++i)
            {
                ref readonly var shotEvent = ref events[i];
                var shotInfo = shotEvent.Event.Payload;
                if (!ValidateShot(shotInfo))
                {
                    continue;
                }

                var shooterSpatialID = new EntityId(shotInfo.EntityId);
                if (!workerSystem.TryGetEntity(shooterSpatialID, out var shooterEntity))
                {
                    continue;
                }

                var gunComponent = gunDataForEntity[shooterEntity];
                var damage = GunDictionary.Get(gunComponent.GunId).ShotDamage;

                var modifyHealthRequest = new HealthComponent.ModifyHealth.Request(
                    new EntityId(shotInfo.EntityId),
                    new HealthModifier()
                    {
                        Amount = -damage,
                        Origin = shotInfo.HitOrigin,
                        AppliedLocation = shotInfo.HitLocation
                    }
                );

                commandSystem.SendCommand(modifyHealthRequest);
            }
        }

        private bool ValidateShot(ShotInfo shot)
        {
            return shot.HitSomething && shot.EntityId > 0;
        }
    }
}
