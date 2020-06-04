using System;
using System.Collections.Generic;
using Fps.Movement;
using Fps.SchemaExtensions;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Representation;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.Subscriptions;
using Unity.Entities;
using UnityEngine;

namespace Fps.Config
{
    public class AdvancedEntityPipeline : IEntityGameObjectCreator
    {
        private const string PlayerEntityType = "Player";

        private readonly string workerType;
        private readonly Vector3 workerOrigin;

        private readonly Dictionary<EntityId, GameObject> gameObjectsCreated = new Dictionary<EntityId, GameObject>();

        public event Action OnRemovedAuthoritativePlayer;

        private readonly Type[] componentsToAdd =
        {
            typeof(Transform), typeof(Rigidbody)
        };

        public AdvancedEntityPipeline(WorkerInWorld worker)
        {
            workerType = worker.WorkerType;
            workerOrigin = worker.Origin;
        }

        public void PopulateEntityTypeExpectations(EntityTypeExpectations entityTypeExpectations)
        {
            entityTypeExpectations.RegisterDefault(new[]
            {
                typeof(Position.Component)
            });

            entityTypeExpectations.RegisterEntityType(PlayerEntityType, new[]
            {
                typeof(ServerMovement.Component)
            });
        }

        public void OnEntityCreated(SpatialOSEntityInfo entityInfo, GameObject prefab, EntityManager entityManager,
            EntityGameObjectLinker linker)
        {
            Vector3 spawnPosition;
            switch (entityInfo.EntityType)
            {
                case PlayerEntityType:
                    spawnPosition = entityManager.GetComponentData<ServerMovement.Component>(entityInfo.Entity)
                        .Latest.Position.ToVector3();
                    break;
                default:
                    spawnPosition = entityManager.GetComponentData<Position.Component>(entityInfo.Entity)
                        .Coords.ToUnityVector();
                    break;
            }

            var gameObject = UnityEngine.Object.Instantiate(prefab, spawnPosition + workerOrigin, Quaternion.identity);

            gameObjectsCreated.Add(entityInfo.SpatialOSEntityId, gameObject);
            gameObject.name = $"{prefab.name}(SpatialOS: {entityInfo.SpatialOSEntityId}, Worker: {workerType})";
            linker.LinkGameObjectToSpatialOSEntity(entityInfo.SpatialOSEntityId, gameObject, componentsToAdd);
        }

        public void OnEntityRemoved(EntityId entityId)
        {
            if (!gameObjectsCreated.TryGetValue(entityId, out var gameObject))
            {
                return;
            }

            // Trigger a callback when authoritative player gets removed
            if (gameObject.GetComponent<FpsDriver>() != null)
            {
                OnRemovedAuthoritativePlayer?.Invoke();
            }

            gameObjectsCreated.Remove(entityId);
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
