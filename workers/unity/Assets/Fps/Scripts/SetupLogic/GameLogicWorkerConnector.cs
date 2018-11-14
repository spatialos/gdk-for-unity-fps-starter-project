using UnityEngine;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.PlayerLifecycle;

namespace Fps
{
    public class GameLogicWorkerConnector : WorkerConnectorBase
    {
        public bool DisableRenderers = true;

        protected override async void Start()
        {
            await AttemptConnect();
        }

        protected override string GetWorkerType()
        {
            return WorkerUtils.UnityGameLogic;
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            var world = Worker.World;

            PlayerLifecycleHelper.AddServerSystems(world);
            GameObjectRepresentationHelper.AddSystems(world);
            GameObjectCreationHelper.EnableStandardGameObjectCreation(world);

            // Shooting
            world.GetOrCreateManager<ServerShootingSystem>();

            // Metrics
            world.GetOrCreateManager<MetricSendSystem>();

            // Health
            world.GetOrCreateManager<ServerHealthModifierSystem>();
            world.GetOrCreateManager<HealthRegenSystem>();

            world.GetOrCreateManager<CommandFrameSystem>();

            base.HandleWorkerConnectionEstablished();
        }

        protected override void LoadWorld()
        {
            base.LoadWorld();

            if (DisableRenderers)
            {
                foreach (var renderer in levelInstance.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = false;
                }
            }
        }
    }
}
