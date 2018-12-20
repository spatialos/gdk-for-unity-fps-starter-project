using System.Collections;
using Improbable;
using Improbable.Common;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Health;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

namespace Fps
{
    public class RespawnHandler : MonoBehaviour
    {
        [Require] private HealthComponent.Requirable.CommandRequestHandler respawnRequests;
        [Require] private HealthComponent.Requirable.Writer health;
        [Require] private Position.Requirable.Writer spatialPosition;

        private FpsServerDriver movementDriver;

        private void OnEnable()
        {
            respawnRequests.OnRequestRespawnRequest += OnRequestRespawn;

            movementDriver = GetComponent<FpsServerDriver>();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnRequestRespawn(HealthComponent.RequestRespawn.RequestResponder request)
        {
            // Reset the player's health.
            var healthUpdate = new HealthComponent.Update
            {
                Health = health.Data.MaxHealth
            };
            health.Send(healthUpdate);

            // Move to a spawn point (position and rotation)
            var (spawnPosition, spawnYaw, spawnPitch) = SpawnPoints.GetRandomSpawnPoint();

            Debug.LogFormat("Request respawn to {0}", spawnPosition);
            movementDriver.Teleport(spawnPosition);

            // Trigger the respawn event.
            health.SendRespawn(new Empty());

            // Update spatial position in the next frame.
            StartCoroutine(SetSpatialPosition(spawnPosition));
        }

        private IEnumerator SetSpatialPosition(Vector3 position)
        {
            yield return null;
            var spatialPositionUpdate = new Position.Update
            {
                Coords = position.ToSpatialCoordinates()
            };
            spatialPosition.Send(spatialPositionUpdate);
        }
    }
}
