using System.Collections;
using Fps.SchemaExtensions;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Fps.Respawning
{
    public class RespawnHandler : MonoBehaviour
    {
        [Require] private HealthComponentCommandReceiver respawnRequests;
        [Require] private HealthComponentWriter health;
        [Require] private ServerMovementWriter serverMovement;
        [Require] private PositionWriter spatialPosition;

        private LinkedEntityComponent spatial;

        private void OnEnable()
        {
            respawnRequests.OnRequestRespawnRequestReceived += OnRequestRespawn;
            spatial = GetComponent<LinkedEntityComponent>();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnRequestRespawn(HealthComponent.RequestRespawn.ReceivedRequest request)
        {
            // Reset the player's health.
            var healthUpdate = new HealthComponent.Update
            {
                Health = health.Data.MaxHealth
            };
            health.SendUpdate(healthUpdate);

            // Move to a spawn point (position and rotation)
            var (spawnPosition, spawnYaw, spawnPitch) = Respawning.SpawnPoints.GetRandomSpawnPoint();
            var newLatest = new ServerResponse
            {
                Position = spawnPosition.ToVector3Int(),
                IncludesJump = false,
                Timestamp = serverMovement.Data.Latest.Timestamp,
                TimeDelta = 0
            };

            var serverMovementUpdate = new ServerMovement.Update
            {
                Latest = newLatest
            };
            serverMovement.SendUpdate(serverMovementUpdate);

            transform.position = spawnPosition + spatial.Worker.Origin;

            var forceRotationRequest = new RotationUpdate
            {
                Yaw = spawnYaw.ToInt1k(),
                Pitch = spawnPitch.ToInt1k(),
                TimeDelta = 0
            };
            serverMovement.SendForcedRotationEvent(forceRotationRequest);

            // Trigger the respawn event.
            health.SendRespawnEvent(new Empty());

            // Update spatial position in the next frame.
            StartCoroutine(SetSpatialPosition(spawnPosition));
        }

        private IEnumerator SetSpatialPosition(Vector3 position)
        {
            yield return null;
            var spatialPositionUpdate = new Position.Update
            {
                Coords = Coordinates.FromUnityVector(position)
            };
            spatialPosition.SendUpdate(spatialPositionUpdate);
        }
    }
}
