using System.Collections;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Fps.Guns;
using Fps.Health;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

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

        protected override IConnectionHandlerBuilder GetConnectionHandlerBuilder()
        {
            IConnectionFlow connectionFlow;
            ConnectionParameters connectionParameters;

            var workerId = CreateNewWorkerId(WorkerUtils.UnityGameLogic);

            if (Application.isEditor)
            {
                connectionFlow = new ReceptionistFlow(workerId);
                connectionParameters = CreateConnectionParameters(WorkerUtils.UnityGameLogic);
            }
            else
            {
                connectionFlow = new ReceptionistFlow(workerId, new CommandLineConnectionFlowInitializer());
                connectionParameters = CreateConnectionParameters(WorkerUtils.UnityGameLogic,
                    new CommandLineConnectionParameterInitializer());
            }

            return new SpatialOSConnectionHandlerBuilder()
                .SetConnectionFlow(connectionFlow)
                .SetConnectionParameters(connectionParameters);
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
