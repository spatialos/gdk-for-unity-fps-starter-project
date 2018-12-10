using System.Collections.Generic;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

public class AdvancedEntityPipeline : IEntityGameObjectCreator
{
    private const string GameobjectNameFormat = "{0}(SpatialOS {1}, Worker: {2})";
    private const string WorkerAttributeFormat = "workerId:{0}";
    private const string PlayerMetadata = "Player";

    private readonly GameObject cachedAuthPlayer;
    private readonly GameObject cachedNonAuthPlayer;

    private readonly IEntityGameObjectCreator fallback;
    private readonly string workerIdAttribute;
    private readonly Worker worker;

    private readonly Dictionary<EntityId, GameObject> playerEntityObjects = new Dictionary<EntityId, GameObject>();

    public AdvancedEntityPipeline(Worker worker, string authPlayer, string nonAuthPlayer,
        IEntityGameObjectCreator fallback)
    {
        this.worker = worker;
        this.fallback = fallback;
        workerIdAttribute = string.Format(WorkerAttributeFormat, worker.WorkerId);
        cachedAuthPlayer = Resources.Load<GameObject>(authPlayer);
        cachedNonAuthPlayer = Resources.Load<GameObject>(nonAuthPlayer);
    }

    public void OnEntityCreated(SpatialOSEntity entity, EntityGameObjectLinker linker)
    {
        if (!entity.HasComponent<Metadata.Component>())
        {
            return;
        }

        var prefabName = entity.GetComponent<Metadata.Component>().EntityType;
        if (prefabName.Equals(PlayerMetadata))
        {
            var gameObject = CreatePlayerEntity(entity);
            playerEntityObjects.Add(entity.SpatialOSEntityId, gameObject);
            linker.LinkGameObjectToSpatialOSEntity(entity.SpatialOSEntityId, gameObject);

            return;
        }

        fallback.OnEntityCreated(entity, linker);
    }

    public void OnEntityRemoved(EntityId entityId)
    {
        if (playerEntityObjects.TryGetValue(entityId, out var gameObject))
        {
            Object.Destroy(gameObject);
            playerEntityObjects.Remove(entityId);
            return;
        }

        fallback.OnEntityRemoved(entityId);
    }

    private static string GetGameObjectName(GameObject prefab, SpatialOSEntity entity, Worker worker)
    {
        return string.Format(GameobjectNameFormat, prefab.name, entity.SpatialOSEntityId, worker.WorkerType);
    }

    private GameObject CreatePlayerEntity(SpatialOSEntity entity)
    {
        var isAuthoritative = IsPlayerAuthoritative(entity);

        var serverPosition = entity.GetComponent<ServerMovement.Component>();
        var position = serverPosition.Latest.Position.ToVector3() + worker.Origin;

        var prefab = isAuthoritative ? cachedAuthPlayer : cachedNonAuthPlayer;
        var gameObject = Object.Instantiate(prefab, position, Quaternion.identity);

        gameObject.name = GetGameObjectName(prefab, entity, worker);

        return gameObject;
    }

    private bool IsPlayerAuthoritative(SpatialOSEntity entity)
    {
        if (!entity.HasComponent<EntityAcl.Component>())
        {
            Debug.LogErrorFormat("Failed to determine authority of player {0} as there was no EntityAcl at " +
                "the time of creation.", entity.SpatialOSEntityId.Id);
            return false;
        }

        if (!entity.GetComponent<EntityAcl.Component>().ComponentWriteAcl.TryGetValue(ClientMovement.ComponentId,
            out var clientMovementWrite))
        {
            Debug.LogErrorFormat("Failed to determine authority of player {0} as the EntityAcl did not contain the" +
                "ClientMovement component.", entity.SpatialOSEntityId.Id);
            return false;
        }

        foreach (var attributeSet in clientMovementWrite.AttributeSet)
        {
            if (attributeSet.Attribute.Contains(workerIdAttribute))
            {
                return true;
            }
        }

        return false;
    }
}
