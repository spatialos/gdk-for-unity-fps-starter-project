using System.Collections;
using UnityEngine;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.PlayerLifecycle;

namespace Fps
{
    public class GameLogicWorkerConnector : WorkerConnectorBase
    {
        public bool DisableRenderers = true;

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

        protected override IEnumerator LoadWorld()
        {
            yield return base.LoadWorld();
            
            var levelTiles = LevelInstance.GetComponentsInChildren<TileEnabler>(true);
            foreach (var tileEnabler in levelTiles)
            {
                tileEnabler.Initialize(false);
            }

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
