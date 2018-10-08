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
        [Require] private ClientRotation.Requirable.CommandRequestSender clientRotationRequest;
        [Require] private HealthComponent.Requirable.Writer health;
        [Require] private ServerMovement.Requirable.Writer serverMovement;
        [Require] private Position.Requirable.Writer spatialPosition;

        private SpatialOSComponent spatial;

        private void OnEnable()
        {
            respawnRequests.OnRequestRespawnRequest += OnRequestRespawn;
            spatial = GetComponent<SpatialOSComponent>();
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
                IncludesJump = false,
                Timestamp = serverMovement.Data.Latest.Timestamp,
                TimeDelta = 0
            };

            var serverMovementUpdate = new ServerMovement.Update
            {
                Latest = newLatest
            };
            serverMovement.Send(serverMovementUpdate);

            // Update the spatial position on respawn.
            var spatialPositionUpdate = new Position.Update
            {
                Coords = spawnPosition.ToSpatialCoordinates()
            };
            spatialPosition.Send(spatialPositionUpdate);

            transform.position = spawnPosition + spatial.Worker.Origin;

            var forceRotationRequest = new RotationUpdate
            {
                Yaw = spawnYaw.ToInt1k(),
                Pitch = spawnPitch.ToInt1k(),
                TimeDelta = 0
            };
            clientRotationRequest.SendForceRotationRequest(spatial.SpatialEntityId, forceRotationRequest);

            // Trigger the respawn event.
            health.SendRespawn(new Empty());
        }
    }
}
