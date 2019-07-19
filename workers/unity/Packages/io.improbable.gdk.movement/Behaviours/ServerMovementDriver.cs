using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.StandardTypes;
using Improbable.Gdk.TransformSynchronization;
using UnityEngine;

namespace Improbable.Gdk.Movement
{
    public class ServerMovementDriver : CharacterControllerMotor
    {
        [Require] private ServerMovementWriter serverMovementWriter;
        [Require] private ClientMovementReader clientMovementReader;
        [Require] private PositionWriter spatialPositionWriter;

        [SerializeField] private float spatialPositionUpdateHz = 1.0f;
        [SerializeField, HideInInspector] private float spatialPositionUpdateDelta;

        private LinkedEntityComponent LinkedEntityComponent;

        private Vector3 lastPosition;
        private Vector3 workerOrigin;
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
            workerOrigin = LinkedEntityComponent.Worker.Origin;

            clientMovementReader.OnMovementUpdate += OnClientUpdate;
        }

        private void OnClientUpdate(MovementInfo request)
        {
            // Move the player by the given delta.
            Move(TransformUtils.ToUnityVector3(request.Position));

            var positionNoOffset = transform.position - workerOrigin;

            serverMovementWriter.SendUpdate(new ServerMovement.Update
            {
                Movement = new MovementInfo
                {
                    Position = TransformUtils.ToFixedPointVector3(positionNoOffset),
                    IncludesJump = request.IncludesJump,
                    TimeDelta = request.TimeDelta
                }
            });

            if (!(Time.time - lastSpatialPositionTime > spatialPositionUpdateDelta))
            {
                return;
            }

            spatialPositionWriter.SendUpdate(new Position.Update
            {
                Coords = positionNoOffset.ToSpatialCoordinates()
            });

            lastSpatialPositionTime = Time.time;
        }
    }
}
