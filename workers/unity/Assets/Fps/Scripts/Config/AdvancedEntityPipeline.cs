using System;
using System.Collections.Generic;
using System.IO;
using Fps.Movement;
using Fps.SchemaExtensions;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Gdk.Subscriptions;
using Unity.Entities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fps.Config
{
    public class AdvancedEntityPipeline : IEntityGameObjectCreator
    {
        private const string PlayerEntityType = "Player";

        private readonly GameObject cachedAuthPlayer;
        private readonly GameObject cachedNonAuthPlayer;

        private readonly string workerId;
        private readonly string workerType;
        private readonly Vector3 workerOrigin;

        private readonly Dictionary<string, GameObject> cachedPrefabs = new Dictionary<string, GameObject>();
        private readonly Dictionary<EntityId, GameObject> gameObjectsCreated = new Dictionary<EntityId, GameObject>();

        public event Action OnRemovedAuthoritativePlayer;

        private readonly Type[] componentsToAdd =
        {
            typeof(Transform), typeof(Rigidbody)
        };

        public ComponentType[] MinimumComponentTypes { get; } = { };

        public AdvancedEntityPipeline(WorkerInWorld worker, string authPlayer, string nonAuthPlayer)
        {
            workerId = worker.WorkerId;
            workerType = worker.WorkerType;
            workerOrigin = worker.Origin;

            cachedAuthPlayer = Resources.Load<GameObject>(authPlayer);
            cachedNonAuthPlayer = Resources.Load<GameObject>(nonAuthPlayer);
        }

        public void Register(Dictionary<string, EntityTypeRegistration> entityTypeRegistrations)
        {
            entityTypeRegistrations.Add(PlayerEntityType, new EntityTypeRegistration(CreatePlayerGameObject,
                typeof(OwningWorker.Component),
                typeof(ServerMovement.Component)));
        }

        public void OnEntityCreated(SpatialOSEntity entity, EntityGameObjectLinker linker)
        {
            if (!entity.TryGetComponent<Metadata.Component>(out var metadata) ||
                !entity.TryGetComponent<Position.Component>(out var spatialOSPosition))
            {
                return;
            }

            var prefabName = metadata.EntityType;
            var position = spatialOSPosition.Coords.ToUnityVector() + workerOrigin;

            if (!cachedPrefabs.TryGetValue(prefabName, out var prefab))
            {
                var workerSpecificPath = Path.Combine("Prefabs", workerType, prefabName);
                var commonPath = Path.Combine("Prefabs", "Common", prefabName);

                prefab = Resources.Load<GameObject>(workerSpecificPath) ?? Resources.Load<GameObject>(commonPath);
                cachedPrefabs[prefabName] = prefab;
            }

            if (prefab == null)
            {
                return;
            }

            var gameObject = Object.Instantiate(prefab, position, Quaternion.identity);
            gameObject.name = $"{prefab.name}(SpatialOS: {entity.SpatialOSEntityId}, Worker: {workerType})";

            gameObjectsCreated.Add(entity.SpatialOSEntityId, gameObject);
            linker.LinkGameObjectToSpatialOSEntity(entity.SpatialOSEntityId, gameObject, componentsToAdd);
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
