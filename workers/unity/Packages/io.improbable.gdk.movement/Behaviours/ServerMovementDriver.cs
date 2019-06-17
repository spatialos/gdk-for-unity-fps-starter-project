using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

namespace Improbable.Gdk.Movement
{
    public class ServerMovementDriver : CharacterControllerMotor
    {
        [Require] private ServerMovementWriter server;
        [Require] private ClientMovementReader client;
        [Require] private PositionWriter spatialPosition;

        [SerializeField] private float spatialPositionUpdateHz = 1.0f;
        [SerializeField, HideInInspector] private float spatialPositionUpdateDelta;

        private LinkedEntityComponent LinkedEntityComponent;

        private Vector3 lastPosition;
        private Vector3 origin;
        private float lastSpatialPositionTime;

        // Cache the update delta values.
        private void OnValidate()
        {
            if (spatialPositionUpdateHz > 0.0f)
            {
                spatialPositionUpdateDelta = 1.0f / spatialPositionUpdateHz;
            }
            else
            {
                spatialPositionUpdateDelta = 1.0f;
                Debug.LogError("The Spatial Position Update Hz must be greater than 0.");
            }
        }

        private void OnEnable()
        {
            LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
            origin = LinkedEntityComponent.Worker.Origin;

            client.OnLatestUpdate += OnClientUpdate;
        }

        private void OnClientUpdate(ClientRequest request)
        {
            // Move the player by the given delta.
            Move(request.Movement.ToVector3());

            var positionNoOffset = transform.position - origin;

            // Send the update using the new position.
            var response = new ServerResponse
            {
                Position = positionNoOffset.ToIntAbsolute(),
                IncludesJump = request.IncludesJump,
                Timestamp = request.Timestamp,
                TimeDelta = request.TimeDelta
            };
            var update = new ServerMovement.Update { Latest = response };
            server.SendUpdate(update);

            if (Time.time - lastSpatialPositionTime > spatialPositionUpdateDelta)
            {
                var positionUpdate = new Position.Update { Coords = positionNoOffset.ToSpatialCoordinates() };
                spatialPosition.SendUpdate(positionUpdate);
                lastSpatialPositionTime = Time.time;
            }
        }
    }
}
