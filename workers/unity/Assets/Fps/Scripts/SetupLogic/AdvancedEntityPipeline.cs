using System;
using System.Collections.Generic;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Gdk.Subscriptions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fps
{
    public class AdvancedEntityPipeline : IEntityGameObjectCreator
    {
        private const string PlayerEntityType = "Player";

        private readonly GameObject cachedAuthPlayer;
        private readonly GameObject cachedNonAuthPlayer;

        private readonly GameObjectCreatorFromMetadata fallback;

        private readonly string workerId;
        private readonly string workerType;
        private readonly Vector3 workerOrigin;

        private readonly Dictionary<EntityId, GameObject> gameObjectsCreated = new Dictionary<EntityId, GameObject>();

        public event Action OnRemovedAuthoritativePlayer;

        private readonly Type[] componentsToAdd =
        {
            typeof(Transform),
            typeof(Rigidbody)
        };

        public AdvancedEntityPipeline(WorkerInWorld worker, string authPlayer, string nonAuthPlayer)
        {
            workerId = worker.WorkerId;
            workerType = worker.WorkerType;
            workerOrigin = worker.Origin;

            fallback = new GameObjectCreatorFromMetadata(workerType, workerOrigin, worker.LogDispatcher);
            cachedAuthPlayer = Resources.Load<GameObject>(authPlayer);
            cachedNonAuthPlayer = Resources.Load<GameObject>(nonAuthPlayer);
        }

        public void OnEntityCreated(SpatialOSEntity entity, EntityGameObjectLinker linker)
        {
            if (!entity.TryGetComponent<Metadata.Component>(out var metadata))
            {
                return;
            }

            if (metadata.EntityType == PlayerEntityType)
            {
                CreatePlayerGameObject(entity, linker);
                return;
            }

            fallback.OnEntityCreated(entity, linker);
        }

        private void CreatePlayerGameObject(SpatialOSEntity entity, EntityGameObjectLinker linker)
        {
            if (!entity.TryGetComponent<OwningWorker.Component>(out var owningWorker))
            {
                throw new InvalidOperationException("Player entity does not have the OwningWorker component");
            }

            var serverPosition = entity.GetComponent<ServerMovement.Component>();
            var position = serverPosition.Latest.Position.ToVector3() + workerOrigin;

            var prefab = owningWorker.WorkerId == workerId ? cachedAuthPlayer : cachedNonAuthPlayer;
            var gameObject = Object.Instantiate(prefab, position, Quaternion.identity);

            gameObjectsCreated.Add(entity.SpatialOSEntityId, gameObject);
            gameObject.name = $"{prefab.name}(SpatialOS {entity.SpatialOSEntityId}, Worker: {workerType})";
            linker.LinkGameObjectToSpatialOSEntity(entity.SpatialOSEntityId, gameObject, componentsToAdd);
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
