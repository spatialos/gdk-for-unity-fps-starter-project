﻿using System;
using Fps;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Improbable.Worker;

public class FakeClientCoordinatorWorkerConnector : WorkerConnectorBase
{
    public GameObject FakeClientWorkerConnector;
    public int FakeClientCount = 1;
    public int FakeClientCreationInterval = 5;

    private readonly Dictionary<EntityId, List<GameObject>> proxies = new Dictionary<EntityId, List<GameObject>>();
    private readonly List<EntityId> localFakeClients = new List<EntityId>();

    protected override async void Start()
    {
        Application.targetFrameRate = 60;
        await AttemptConnect();
    }

    protected override string GetWorkerType()
    {
        return WorkerUtils.FakeClientCoorindator;
    }

    protected override async void HandleWorkerConnectionEstablished()
    {
        base.HandleWorkerConnectionEstablished();

        if (FakeClientWorkerConnector != null)
        {
            for (var i = 0; i < FakeClientCount; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(FakeClientCreationInterval));
                Instantiate(FakeClientWorkerConnector, transform.position, transform.rotation);
            }
        }
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
        else if (localFakeClients.Contains(entityId))
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

        if (!localFakeClients.Contains(entityId) && entityProxies.Count > 0)
        {
            entityProxies[0].SetActive(true);
        }
    }

    public void RegisterLocalFakePlayer(EntityId entityId, GameObject prefab)
    {
        localFakeClients.Add(entityId);
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

    public void UnregisterLocalFakePlayer(EntityId entityId, GameObject prefab)
    {
        localFakeClients.Remove(entityId);

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
