using UnityEngine;
using Improbable.Gdk.GameObjectCreation;
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
            Application.targetFrameRate = 60;
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
            GameObjectCreationHelper.EnableStandardGameObjectCreation(world);

            // Shooting
            world.GetOrCreateManager<ServerShootingSystem>();

            // Metrics
            world.GetOrCreateManager<MetricSendSystem>();

            // Health
            world.GetOrCreateManager<ServerHealthModifierSystem>();
            world.GetOrCreateManager<HealthRegenSystem>();

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
