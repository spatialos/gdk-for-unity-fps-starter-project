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
        [Require] private ServerMovement.Requirable.Writer serverMovement;
        [Require] private Position.Requirable.Writer spatialPosition;

        private SpatialOSComponent spatial;

        private void OnEnable()
        {
            respawnRequests.OnRequestRespawnRequest += OnRequestRespawn;
            spatial = GetComponent<SpatialOSComponent>();
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
            var newLatest = new ServerResponse
            {
                Position = spawnPosition.ToIntAbsolute(),
                // JumpPressed = false,
                Timestamp = serverMovement.Data.Latest.Timestamp,
                // TimeDelta = 0
            };

            var serverMovementUpdate = new ServerMovement.Update
            {
                Latest = newLatest
            };
            serverMovement.Send(serverMovementUpdate);

            transform.position = spawnPosition + spatial.Worker.Origin;

            var forceRotationRequest = new RotationUpdate
            {
                Yaw = spawnYaw.ToInt1k(),
                Pitch = spawnPitch.ToInt1k(),
                TimeDelta = 0
            };
            serverMovement.SendForcedRotation(forceRotationRequest);

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
