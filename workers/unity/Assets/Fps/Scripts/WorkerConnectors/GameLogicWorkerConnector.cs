using Fps.Config;
using Fps.Guns;
using Fps.Health;
using Fps.Metrics;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Representation;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.LoadBalancing;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Generated;
using Improbable.Worker.CInterop;
using Playground.LoadBalancing;
using Unity.Entities;
using UnityEngine;

namespace Fps.WorkerConnectors
{
    public class GameLogicWorkerConnector : WorkerConnectorBase
    {
        public bool DisableRenderers = true;
        [SerializeField] private EntityRepresentationMapping entityRepresentationMapping;

        protected async void Start()
        {
            Application.targetFrameRate = 60;

            await Connect(GetConnectionHandlerBuilder(), new ForwardingDispatcher());
            await LoadWorld();

            if (DisableRenderers)
            {
                foreach (var childRenderer in LevelInstance.GetComponentsInChildren<Renderer>())
                {
                    childRenderer.enabled = false;
                }
            }
        }

        private IConnectionHandlerBuilder GetConnectionHandlerBuilder()
        {
            IConnectionFlow connectionFlow;
            ConnectionParameters connectionParameters;

            var workerId = CreateNewWorkerId(WorkerUtils.UnityGameLogic);

            if (Application.isEditor)
            {
                connectionFlow = new ReceptionistFlow(workerId);
                connectionParameters = CreateConnectionParameters(WorkerUtils.UnityGameLogic);
                connectionParameters.Network.Kcp.SecurityType = NetworkSecurityType.Insecure;
                connectionParameters.Network.Tcp.SecurityType = NetworkSecurityType.Insecure;
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
            GameObjectCreationHelper.EnableStandardGameObjectCreation(world, entityRepresentationMapping, gameObject);

            Worker.AddLoadBalancingSystems(configure =>
            {
                configure.AddPartitionManagement(WorkerUtils.UnityGameLogic, WorkerUtils.MobileClient, WorkerUtils.UnityClient);
                configure.AddClientLoadBalancing("Player", ComponentSets.PlayerClientSet);

                var loadBalancingMap = new EntityLoadBalancingMap(ComponentSets.DefaultServerSet)
                    .AddOverride("Player", ComponentSets.PlayerServerSet);

                configure.SetSingletonLoadBalancing(WorkerUtils.UnityGameLogic, loadBalancingMap);
            });

            // Shooting
            world.GetOrCreateSystem<ServerShootingSystem>();

            // Metrics
            world.GetOrCreateSystem<MetricSendSystem>();

            // Health
            world.GetOrCreateSystem<ServerHealthModifierSystem>();
            world.GetOrCreateSystem<HealthRegenSystem>();
        }
    }
}
