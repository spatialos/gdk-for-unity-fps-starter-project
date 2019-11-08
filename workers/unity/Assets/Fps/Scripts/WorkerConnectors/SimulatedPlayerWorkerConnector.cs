using System.Text;
using System.Threading.Tasks;
using Fps.Config;
using Fps.WorkerConnectors;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.PlayerLifecycle;

namespace Fps.WorkerConnectors
{
    public class SimulatedPlayerWorkerConnector : WorkerConnector
    {
        private const string AuthPlayer = "Prefabs/SimulatedPlayer/SimulatedPlayer";
        private const string NonAuthPlayer = "Prefabs/SimulatedPlayer/SimulatedPlayerProxy";

        public async Task ConnectSimulatedPlayer(string simulatedPlayerDevAuthToken,
            string simulatedPlayerTargetDeployment)
        {
            var connectionParams = CreateConnectionParameters(WorkerUtils.UnityClient);
            connectionParams.Network.UseExternalIp = true;

            var builder = new SpatialOSConnectionHandlerBuilder()
                .SetConnectionParameters(connectionParams)
                .SetConnectionFlow(new ChosenDeploymentLocatorFlow(simulatedPlayerTargetDeployment)
                {
                    DevAuthToken = simulatedPlayerDevAuthToken
                });

            await Connect(builder, new ForwardingDispatcher());
        }

        public async Task ConnectSimulatedPlayer()
        {
            var connectionParams = CreateConnectionParameters(WorkerUtils.UnityClient);

            // Force the Worker ID back to the generated one otherwise it will take the coordinator's worker ID.
            var workerId = CreateNewWorkerId(WorkerUtils.UnityClient);
            var flow = new ReceptionistFlow(workerId, new CommandLineConnectionFlowInitializer());
            flow.WorkerId = workerId;

            var builder = new SpatialOSConnectionHandlerBuilder()
                .SetConnectionParameters(connectionParams)
                .SetConnectionFlow(flow);

            await Connect(builder, new ForwardingDispatcher());
        }

        public void SpawnPlayer(int number)
        {
            var serializedArgs = Encoding.ASCII.GetBytes($"Simulated Player {number}");
            var sendSystem = Worker.World.GetExistingSystem<SendCreatePlayerRequestSystem>();
            sendSystem.RequestPlayerCreation(serializedArgs);
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            PlayerLifecycleHelper.AddClientSystems(Worker.World, false);

            GameObjectCreationHelper.EnableStandardGameObjectCreation(Worker.World,
                new AdvancedEntityPipeline(Worker, AuthPlayer, NonAuthPlayer));
        }
    }
}
