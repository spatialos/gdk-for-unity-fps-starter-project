using System;
using System.Collections;
using Fps;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using Random = UnityEngine.Random;

public class SimulatedPlayerCoordinatorWorkerConnector : WorkerConnectorBase
{
    private const string FlagClientCount = "fps_simulated_players_per_coordinator";
    private const string FlagCreationInterval = "fps_simulated_players_creation_interval";
    private const string FlagDevAuthTokenId = "fps_simulated_players_dev_auth_token_id";
    private const string FlagTargetDeployment = "fps_simulated_players_target_deployment";

    public GameObject SimulatedPlayerWorkerConnector;
    public int DefaultSimulatedPlayerCount = 1;
    public int DefaultSimulatedPlayerCreationInterval = 5;
    public string SimulatedPlayerDevAuthTokenId;
    public string SimulatedPlayerTargetDeployment;

    private readonly Dictionary<EntityId, List<GameObject>> proxies = new Dictionary<EntityId, List<GameObject>>();
    private readonly List<EntityId> localSimulatedPlayers = new List<EntityId>();

    private readonly List<GameObject> simulatedPlayerConnectors = new List<GameObject>();

    protected override async void Start()
    {
        Application.targetFrameRate = 60;
        await AttemptConnect();
    }

    protected override string GetWorkerType()
    {
        return WorkerUtils.SimulatedPlayerCoordinator;
    }

    protected override async void HandleWorkerConnectionEstablished()
    {
        base.HandleWorkerConnectionEstablished();

        Worker.World.GetOrCreateManager<MetricSendSystem>();

        CheckWorkerFlags();

        if (SimulatedPlayerWorkerConnector != null)
        {
            while (Application.isPlaying && simulatedPlayerConnectors.Count < DefaultSimulatedPlayerCount)
            {
                var simulatedPlayer = Instantiate(SimulatedPlayerWorkerConnector, transform.position, transform.rotation);
                var simulatedPlayerConnector = simulatedPlayer.GetComponent<SimulatedPlayerWorkerConnector>();
                await simulatedPlayerConnector
                    .ConnectSimulatedPlayer(Worker.LogDispatcher, SimulatedPlayerDevAuthTokenId, SimulatedPlayerTargetDeployment);
                simulatedPlayerConnector.SpawnPlayer(simulatedPlayerConnectors.Count);

                simulatedPlayerConnectors.Add(simulatedPlayer);
                await Task.Delay(TimeSpan.FromSeconds(
                    Random.Range(DefaultSimulatedPlayerCreationInterval, 1.25f * DefaultSimulatedPlayerCreationInterval)));
            }

            if (Application.isPlaying)
            {
                StartCoroutine(MonitorSimulatedPlayers());
            }
        }
    }

    public override void Dispose()
    {
        StopAllCoroutines();
        base.Dispose();
    }

    private IEnumerator MonitorSimulatedPlayers()
    {
        while (Worker.Connection?.GetConnectionStatusCode() == ConnectionStatusCode.Success)
        {
            yield return new WaitForSeconds(5);

            if (CheckWorkerFlags())
            {
                while (simulatedPlayerConnectors.Count < DefaultSimulatedPlayerCount)
                {
                    yield return new WaitForSeconds(
                        Random.Range(DefaultSimulatedPlayerCreationInterval, 1.25f * DefaultSimulatedPlayerCreationInterval));
                    var simulatedPlayer = Instantiate(SimulatedPlayerWorkerConnector, transform.position, transform.rotation);
                    var simulatedPlayerConnector = simulatedPlayer.GetComponent<SimulatedPlayerWorkerConnector>();

                    var task = simulatedPlayerConnector.ConnectSimulatedPlayer(Worker.LogDispatcher, SimulatedPlayerDevAuthTokenId,
                        SimulatedPlayerTargetDeployment);
                    yield return new WaitUntil(() => task.IsCompleted);
                    simulatedPlayerConnector.SpawnPlayer(simulatedPlayerConnectors.Count);

                    simulatedPlayerConnectors.Add(simulatedPlayer);
                }

                while (simulatedPlayerConnectors.Count > DefaultSimulatedPlayerCount)
                {
                    var simulatedPlayer = simulatedPlayerConnectors[0];
                    simulatedPlayerConnectors.Remove(simulatedPlayer);
                    Destroy(simulatedPlayer);
                }
            }
        }
    }

    private bool CheckWorkerFlags()
    {
        if (int.TryParse(Worker.Connection.GetWorkerFlag(FlagCreationInterval), out var newInterval))
        {
            DefaultSimulatedPlayerCreationInterval = newInterval;
        }

        SimulatedPlayerDevAuthTokenId = Worker.Connection.GetWorkerFlag(FlagDevAuthTokenId);
        SimulatedPlayerTargetDeployment = Worker.Connection.GetWorkerFlag(FlagTargetDeployment);

        if (int.TryParse(Worker.Connection.GetWorkerFlag(FlagClientCount), out var newCount))
        {
            if (DefaultSimulatedPlayerCount != newCount)
            {
                DefaultSimulatedPlayerCount = newCount;
                return true;
            }
        }

        return false;
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
        var worldSize = MapBuilder.GetWorldSizeFlag(Worker.Connection);
        if (!MapBuilder.TryGetWorldLayerCount(MapBuilderSettings, worldSize, out var worldLayerCount))
        {
            worldLayerCount = MapBuilderSettings.SmallWorldLayerCount;
        }

        return new Bounds(Worker.Origin, MapBuilder.GetWorldDimensions(MapBuilderSettings, worldLayerCount));
    }
}
