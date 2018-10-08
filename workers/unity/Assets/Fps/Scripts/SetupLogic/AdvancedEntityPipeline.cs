using Improbable;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using Improbable.Worker;
using UnityEngine;

public class AdvancedEntityPipeline : IEntityGameObjectCreator
{
    private GameObject cachedAuthPlayer;
    private GameObject cachedNonAuthPlayer;

    private IEntityGameObjectCreator fallback;
    private string workerIdAttribute;
    private string workerType;

    private readonly string gameobjectNameFormat = "{0}(SpatialOS {1}, Worker: {2})";
    private readonly string workerAttributeFormat = "workerId:{0}";
    private readonly string playerMetadata = "Player";

    public AdvancedEntityPipeline(string workerType, string workerId, string authPlayer, string nonAuthPlayer,
        IEntityGameObjectCreator fallback)
    {
        this.workerType = workerType;
        this.fallback = fallback;
        workerIdAttribute = string.Format(workerAttributeFormat, workerId);
        cachedAuthPlayer = Resources.Load<GameObject>(authPlayer);
        cachedNonAuthPlayer = Resources.Load<GameObject>(nonAuthPlayer);
    }

    public GameObject OnEntityCreated(SpatialOSEntity entity)
    {
        if (!entity.HasComponent<Metadata.Component>())
        {
            return null;
        }

        var prefabName = entity.GetComponent<Metadata.Component>().EntityType;
        if (prefabName.Equals(playerMetadata))
        {
            var clientMovement = entity.GetComponent<ClientMovement.Component>();
            if (entity.GetComponent<EntityAcl.Component>().ComponentWriteAcl
                .TryGetValue(clientMovement.ComponentId, out var clientMovementWrite))
            {
                var authority = false;
                foreach (var attributeSet in clientMovementWrite.AttributeSet)
                {
                    if (attributeSet.Attribute.Contains(workerIdAttribute))
                    {
                        authority = true;
                    }
                }

                var serverPosition = entity.GetComponent<ServerMovement.Component>();
                var position = serverPosition.Latest.Position.ToVector3();

                var prefab = authority ? cachedAuthPlayer : cachedNonAuthPlayer;
                var gameObject = Object.Instantiate(prefab, position, Quaternion.identity);
                gameObject.name =
                    string.Format(gameobjectNameFormat, prefab.name, entity.SpatialOSEntityId, workerType);
                return gameObject;
            }
        }

        return fallback.OnEntityCreated(entity);
    }

    public void OnEntityRemoved(EntityId entityId, GameObject linkedGameObject)
    {
        fallback.OnEntityRemoved(entityId, linkedGameObject);
    }
}
