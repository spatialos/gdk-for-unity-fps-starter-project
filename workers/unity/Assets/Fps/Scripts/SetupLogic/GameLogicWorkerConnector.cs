using System.Collections;
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
            base.Start();
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
            world.GetOrCreateSystem<ServerShootingSystem>();

            // Session
            world.GetOrCreateSystem<PlayerStateServerSystem>();

            // Metrics
            world.GetOrCreateSystem<MetricSendSystem>();

            // Health
            world.GetOrCreateSystem<ServerHealthModifierSystem>();
            world.GetOrCreateSystem<HealthRegenSystem>();

            base.HandleWorkerConnectionEstablished();
        }

        protected override IEnumerator LoadWorld()
        {
            yield return base.LoadWorld();

            if (DisableRenderers)
            {
                foreach (var childRenderer in LevelInstance.GetComponentsInChildren<Renderer>())
                {
                    childRenderer.enabled = false;
                }
            }
        }
    }
}
