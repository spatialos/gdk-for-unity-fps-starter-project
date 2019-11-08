using Fps.SchemaExtensions;
using Improbable;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Fps.Movement
{
    public class ServerMovementDriver : CharacterControllerMotor
    {
#pragma warning disable 649
        [Require] private ServerMovementWriter server;
        [Require] private ClientMovementReader client;
        [Require] private PositionWriter spatialPosition;
#pragma warning restore 649

        [SerializeField] private float spatialPositionUpdateHz = 1.0f;
        [SerializeField, HideInInspector] private float spatialPositionUpdateDelta;

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
            var linkedEntityComponent = GetComponent<LinkedEntityComponent>();
            origin = linkedEntityComponent.Worker.Origin;

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
                Position = positionNoOffset.ToVector3Int(),
                IncludesJump = request.IncludesJump,
                Timestamp = request.Timestamp,
                TimeDelta = request.TimeDelta
            };

            var update = new ServerMovement.Update { Latest = response };
            server.SendUpdate(update);

            if (Time.time - lastSpatialPositionTime > spatialPositionUpdateDelta)
            {
                var positionUpdate = new Position.Update { Coords = Coordinates.FromUnityVector(positionNoOffset) };
                spatialPosition.SendUpdate(positionUpdate);
                lastSpatialPositionTime = Time.time;
            }
        }
    }
}
