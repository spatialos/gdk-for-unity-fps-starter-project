using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fps.Config;
using Fps.Metrics;
using Fps.WorldTiles;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Improbable.Worker.CInterop;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fps.WorkerConnectors
{
    public class SimulatedPlayerCoordinatorWorkerConnector : WorkerConnectorBase
    {
        private const string flagDevAuthTokenId = "fps_simulated_players_dev_auth_token_id";
        private const string flagTargetDeployment = "fps_simulated_players_target_deployment";
        private const string flagClientCount = "fps_simulated_players_per_coordinator";
        private const string flagCreationInterval = "fps_simulated_players_creation_interval";

        private delegate void UpdateCoordinatorParams(ref int count, ref int interval);

        private const string DeploymentNameFlag = "deploymentName";

        public GameObject SimulatedPlayerWorkerConnector;

        public int MaxSimulatedPlayerCount = 1;
        public int SimulatedPlayerCreationInterval = 5;

        private readonly Dictionary<EntityId, List<GameObject>> proxies = new Dictionary<EntityId, List<GameObject>>();
        private readonly List<EntityId> localSimulatedPlayers = new List<EntityId>();

        private readonly List<GameObject> simulatedPlayerConnectors = new List<GameObject>();

        private bool connectPlayersWithDevAuth;

        private CancellationTokenSource tokenSource;
        private int TimeBetweenCycleSecs = 2;

        protected override async void Start()
        {
            var args = CommandLineArgs.FromCommandLine();

            var deploymentName = string.Empty;

            if (args.TryGetCommandLineValue(DeploymentNameFlag, ref deploymentName))
            {
                connectPlayersWithDevAuth = deploymentName != "local";
            }
            else
            {
                // We are probably in the Editor.
                connectPlayersWithDevAuth = false;
            }

            Application.targetFrameRate = 60;
            await AttemptConnect();
        }

        protected override IConnectionHandlerBuilder GetConnectionHandlerBuilder()
        {
            IConnectionFlow connectionFlow;
            ConnectionParameters connectionParameters;

            var workerId = CreateNewWorkerId(WorkerUtils.SimulatedPlayerCoordinator);

            if (Application.isEditor)
            {
                connectionFlow = new ReceptionistFlow(workerId);
                connectionParameters = CreateConnectionParameters(WorkerUtils.SimulatedPlayerCoordinator);
            }
            else
            {
                connectionFlow = new ReceptionistFlow(workerId, new CommandLineConnectionFlowInitializer());
                connectionParameters = CreateConnectionParameters(WorkerUtils.SimulatedPlayerCoordinator,
                    new CommandLineConnectionParameterInitializer());
            }

            return new SpatialOSConnectionHandlerBuilder()
                .SetConnectionFlow(connectionFlow)
                .SetConnectionParameters(connectionParameters);
        }

        protected override async void HandleWorkerConnectionEstablished()
        {
            base.HandleWorkerConnectionEstablished();

            Worker.World.GetOrCreateSystem<MetricSendSystem>();

            if (SimulatedPlayerWorkerConnector == null)
            {
                return;
            }

            tokenSource = new CancellationTokenSource();

            try
            {
                // If we connect via dev auth. We are connecting via the Alpha Locator & we want to use worker flags
                // to change the parameters..
                if (connectPlayersWithDevAuth)
                {
                    await WaitForWorkerFlags(flagDevAuthTokenId, flagTargetDeployment, flagClientCount,
                        flagCreationInterval);

                    var simulatedPlayerDevAuthTokenId = Worker.GetWorkerFlag(flagDevAuthTokenId);
                    var simulatedPlayerTargetDeployment = Worker.GetWorkerFlag(flagTargetDeployment);

                    int.TryParse(Worker.GetWorkerFlag(flagCreationInterval), out var originalInterval);
                    int.TryParse(Worker.GetWorkerFlag(flagClientCount), out var originalCount);

                    using (var workerFlagTracker =
                        new WorkerFlagTracker(Worker.World.GetExistingSystem<WorkerFlagCallbackSystem>()))
                    {
                        await Monitor(originalCount, originalInterval,
                            connector =>
                                connector.ConnectSimulatedPlayer(simulatedPlayerDevAuthTokenId,
                                    simulatedPlayerTargetDeployment),
                            (ref int count, ref int interval) =>
                                MonitorWorkerFlags(workerFlagTracker, ref count, ref interval));
                    }
                }
                // If not using dev auth flow, we are in the editor or standalone worker on a local deployment.
                // Connect via receptionist & use prefabs to change the parameters.
                else
                {
                    await Monitor(MaxSimulatedPlayerCount, SimulatedPlayerCreationInterval,
                        connector => connector.ConnectSimulatedPlayer(),
                        (ref int count, ref int interval) =>
                        {
                            count = MaxSimulatedPlayerCount;
                            interval = SimulatedPlayerCreationInterval;
                        });
                }
            }
            catch (TaskCanceledException)
            {
                // This is fine. Means we have triggered a cancel via Dispose().
            }
        }

        public override void Dispose()
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = null;

            base.Dispose();
        }

        // Wait for a number of worker flags to be present for this worker. Will spin forever otherwise.
        private async Task WaitForWorkerFlags(params string[] flagKeys)
        {
            var token = tokenSource.Token;

            while (flagKeys.Any(key => string.IsNullOrEmpty(Worker.GetWorkerFlag(key))))
            {
                if (token.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                Worker.LogDispatcher.HandleLog(LogType.Log, new LogEvent("Waiting for required worker flags.."));
                await Task.Delay(TimeSpan.FromSeconds(TimeBetweenCycleSecs), token);
            }
        }

        // Update worker flags if they have changed.
        private void MonitorWorkerFlags(WorkerFlagTracker tracker, ref int count, ref int interval)
        {
            if (tracker.TryGetFlagChange(flagCreationInterval, out var intervalStr) &&
                int.TryParse(intervalStr, out var newInterval))
            {
                interval = newInterval;
            }

            if (tracker.TryGetFlagChange(flagClientCount, out var newCountStr) &&
                int.TryParse(newCountStr, out var newCount))
            {
                count = newCount;
            }

            tracker.Reset();
        }

        private async Task Monitor(int count, int interval, Func<SimulatedPlayerWorkerConnector, Task> connectMethod,
            UpdateCoordinatorParams updateParams)
        {
            var token = tokenSource.Token;

            while (Worker != null && Worker.IsConnected)
            {
                if (token.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                updateParams(ref count, ref interval);

                while (simulatedPlayerConnectors.Count < count)
                {
                    await CreateSimPlayer(interval, connectMethod, token);
                }

                while (simulatedPlayerConnectors.Count > count)
                {
                    RemoveSimPlayer();
                }

                await Task.Delay(TimeSpan.FromSeconds(TimeBetweenCycleSecs), token);
            }
        }

        private async Task CreateSimPlayer(int delayInterval, Func<SimulatedPlayerWorkerConnector, Task> connectMethod,
            CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromSeconds(Random.Range(delayInterval, 1.25f * delayInterval)), token);
            var simPlayer = Instantiate(SimulatedPlayerWorkerConnector, transform.position, transform.rotation);
            var connector = simPlayer.GetComponent<SimulatedPlayerWorkerConnector>();

            await connectMethod(connector);
            connector.SpawnPlayer(simulatedPlayerConnectors.Count);

            simulatedPlayerConnectors.Add(simPlayer);
        }

        private void RemoveSimPlayer()
        {
            var simulatedPlayer = simulatedPlayerConnectors[0];
            simulatedPlayerConnectors.Remove(simulatedPlayer);
            Destroy(simulatedPlayer);
        }

        public void RegisterProxyPrefabForEntity(EntityId entityId, GameObject proxy)
        {
            if (!proxies.ContainsKey(entityId))
            {
                proxies[entityId] = new List<GameObject>();
            }

            var entityProxies = proxies[entityId];

            entityProxies.Add(proxy);

            // Disable GameObject if there is a proxy already enabled for this entity.
            if (entityProxies.Count > 1)
            {
                proxy.SetActive(false);
            }
            else if (localSimulatedPlayers.Contains(entityId))
            {
                proxy.SetActive(false);
            }
        }

        // Can only be called from the active proxy
        public void UnregisterProxyPrefabForEntity(EntityId entityId, GameObject proxy)
        {
            if (!proxies.ContainsKey(entityId) || proxies[entityId].Count == 0)
            {
                return;
            }

            var entityProxies = proxies[entityId];
            entityProxies.Remove(proxy);
            entityProxies.RemoveAll(p => p == null);

            if (!localSimulatedPlayers.Contains(entityId) && entityProxies.Count > 0)
            {
                entityProxies[0].SetActive(true);
            }
        }

        public void RegisterLocalSimulatedPlayer(EntityId entityId, GameObject prefab)
        {
            localSimulatedPlayers.Add(entityId);
            if (proxies.ContainsKey(entityId))
            {
                var entityProxies = proxies[entityId];
                entityProxies.RemoveAll(p => p == null);

                foreach (var proxy in entityProxies)
                {
                    proxy.SetActive(false);
                }
            }
        }

        public void UnregisterLocalSimulatedPlayer(EntityId entityId, GameObject prefab)
        {
            localSimulatedPlayers.Remove(entityId);

            if (!proxies.ContainsKey(entityId))
            {
                return;
            }

            var entityProxies = proxies[entityId];
            entityProxies.RemoveAll(p => p == null);

            if (entityProxies.Count > 0)
            {
                entityProxies[0].SetActive(true);
            }
        }

        public Bounds GetWorldBounds()
        {
            var worldSize = GetWorldSize();
            return new Bounds(Worker.Origin, MapBuilder.GetWorldDimensions(worldSize));
        }

        private class WorkerFlagTracker : IDisposable
        {
            private readonly Dictionary<string, string> flagChanges = new Dictionary<string, string>();

            private readonly WorkerFlagCallbackSystem callbackSystem;
            private readonly ulong callbackKey;

            public WorkerFlagTracker(WorkerFlagCallbackSystem callbackSystem)
            {
                this.callbackSystem = callbackSystem;
                callbackKey = callbackSystem.RegisterWorkerFlagChangeCallback(OnWorkerFlagChange);
            }

            public bool TryGetFlagChange(string key, out string value)
            {
                return flagChanges.TryGetValue(key, out value);
            }

            public void Reset()
            {
                flagChanges.Clear();
            }

            private void OnWorkerFlagChange((string, string) pair)
            {
                flagChanges[pair.Item1] = pair.Item2;
            }

            public void Dispose()
            {
                callbackSystem.UnregisterWorkerFlagChangeCallback(callbackKey);
            }
        }
    }
}
