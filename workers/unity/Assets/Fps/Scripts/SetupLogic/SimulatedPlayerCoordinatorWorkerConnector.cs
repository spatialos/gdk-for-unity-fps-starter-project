using System;
using System.Collections;
using Fps;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Improbable.Worker;
using Random = UnityEngine.Random;

public class SimulatedPlayerCoordinatorWorkerConnector : WorkerConnectorBase
{
    private const string FlagClientCount = "fps_simulated_players_per_coordinator";
    private const string FlagCreationInterval = "fps_simulated_players_creation_interval";

    public GameObject SimulatedPlayerWorkerConnector;
    public int SimulatedPlayerCount = 1;
    public int SimulatedPlayerCreationInterval = 5;

    private readonly Dictionary<EntityId, List<GameObject>> proxies = new Dictionary<EntityId, List<GameObject>>();
    private readonly List<EntityId> localSimulatedPlayers = new List<EntityId>();

    private readonly List<GameObject> SimulatedPlayerConnectors = new List<GameObject>();

    protected override async void Start()
    {
        Application.targetFrameRate = 60;
        await AttemptConnect();
    }

    protected override string GetWorkerType()
    {
        return WorkerUtils.SimulatedPlayerCoorindator;
    }

    protected override async void HandleWorkerConnectionEstablished()
    {
        base.HandleWorkerConnectionEstablished();

        Worker.World.GetOrCreateManager<MetricSendSystem>();

        CheckWorkerFlags();

        if (SimulatedPlayerWorkerConnector != null)
        {
            while (SimulatedPlayerConnectors.Count < SimulatedPlayerCount)
            {
                await Task.Delay(TimeSpan.FromSeconds(
                    Random.Range(SimulatedPlayerCreationInterval, 1.25f * SimulatedPlayerCreationInterval)));
                var SimulatedPlayer = Instantiate(SimulatedPlayerWorkerConnector, transform.position, transform.rotation);
                SimulatedPlayerConnectors.Add(SimulatedPlayer);
            }

            StartCoroutine(MonitorSimulatedPlayers());
        }
    }

    private IEnumerator MonitorSimulatedPlayers()
    {
        while (Worker.Connection?.IsConnected == true)
        {
            yield return new WaitForSeconds(5);

            if (CheckWorkerFlags())
            {
                while (SimulatedPlayerConnectors.Count < SimulatedPlayerCount)
                {
                    yield return new WaitForSeconds(
                        Random.Range(SimulatedPlayerCreationInterval, 1.25f * SimulatedPlayerCreationInterval));
                    var SimulatedPlayer = Instantiate(SimulatedPlayerWorkerConnector, transform.position, transform.rotation);
                    SimulatedPlayerConnectors.Add(SimulatedPlayer);
                }

                while (SimulatedPlayerConnectors.Count > SimulatedPlayerCount)
                {
                    var SimulatedPlayer = SimulatedPlayerConnectors[0];
                    SimulatedPlayerConnectors.Remove(SimulatedPlayer);
                    Destroy(SimulatedPlayer);
                }
            }
        }
    }

    private bool CheckWorkerFlags()
    {
        if (int.TryParse(Worker.Connection.GetWorkerFlag(FlagCreationInterval), out var newInterval))
        {
            SimulatedPlayerCreationInterval = newInterval;
        }

        if (int.TryParse(Worker.Connection.GetWorkerFlag(FlagClientCount), out var newCount))
        {
            if (SimulatedPlayerCount != newCount)
            {
                SimulatedPlayerCount = newCount;
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
}
