using System;
using System.Collections.Generic;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using Improbable.Gdk.Subscriptions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fps
{
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

        private readonly Dictionary<EntityId, GameObject> gameObjectsCreated = new Dictionary<EntityId, GameObject>();

        public event Action OnRemovedAuthoritativePlayer;

        private readonly Type[] componentsToAdd =
        {
            typeof(Transform),
            typeof(Rigidbody)
        };

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
                    var position = serverPosition.Latest.Position.ToVector3() + worker.Origin;

                    var prefab = authority ? cachedAuthPlayer : cachedNonAuthPlayer;
                    var gameObject = Object.Instantiate(prefab, position, Quaternion.identity);

                    gameObjectsCreated.Add(entity.SpatialOSEntityId, gameObject);
                    gameObject.name = GetGameObjectName(prefab, entity, worker);
                    linker.LinkGameObjectToSpatialOSEntity(entity.SpatialOSEntityId, gameObject, componentsToAdd);
                    return;
                }
            }

            fallback.OnEntityCreated(entity, linker);
        }

        private static string GetGameObjectName(GameObject prefab, SpatialOSEntity entity, Worker worker)
        {
            return string.Format(GameobjectNameFormat, prefab.name, entity.SpatialOSEntityId, worker.WorkerType);
        }

        public void OnEntityRemoved(EntityId entityId)
        {
            if (!gameObjectsCreated.TryGetValue(entityId, out var go))
            {
                fallback.OnEntityRemoved(entityId);
                return;
            }

            // Trigger a callback when authoritative player gets removed
            if (go.GetComponent<FpsDriver>() != null)
            {
                OnRemovedAuthoritativePlayer?.Invoke();
            }

            gameObjectsCreated.Remove(entityId);
            Object.Destroy(go);
        }
    }
}
