using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.StandardTypes;
using Improbable.Worker.Core;
using UnityEngine;

namespace Improbable.Gdk.Movement
{
    public class ProxyMovementDriver : CharacterControllerMotor
    {
        [Require] private ServerMovement.Requirable.Reader server;
        [Require] private ClientRotation.Requirable.Reader client;

        [SerializeField] private RotationConstraints rotationConstraints = new RotationConstraints
        {
            XAxisRotation = true,
            YAxisRotation = true,
            ZAxisRotation = true
        };

        private SpatialOSComponent spatialOSComponent;
        private Vector3 origin;

        //Rotation Variables
        private float timeLeftToRotate;
        private float lastFullTime;
        private Quaternion source;
        private Quaternion target;
        private bool hasRotationLeft;

        private void OnEnable()
        {
            spatialOSComponent = GetComponent<SpatialOSComponent>();
            origin = spatialOSComponent.Worker.Origin;

            server.LatestUpdated += OnServerUpdate;
            client.LatestUpdated += OnClientUpdate;

            OnClientUpdate(client.Data.Latest);
            OnServerUpdate(server.Data.Latest);
        }

        private void OnClientUpdate(RotationUpdate rotation)
        {
            var x = rotationConstraints.XAxisRotation ? rotation.Pitch.ToFloat1k() : 0;
            var y = rotationConstraints.YAxisRotation ? rotation.Yaw.ToFloat1k() : 0;
            var z = rotationConstraints.ZAxisRotation ? rotation.Roll.ToFloat1k() : 0;

            UpdateRotation(Quaternion.Euler(x, y, z), rotation.TimeDelta);
        }

        private void OnServerUpdate(ServerResponse movement)
        {
            if (server.Authority != Authority.NotAuthoritative)
            {
                return;
            }

            Interpolate(movement.Position.ToVector3() + origin, movement.TimeDelta);
        }

        public void UpdateRotation(Quaternion targetQuaternion, float timeDelta)
        {
            hasRotationLeft = true;
            lastFullTime = timeLeftToRotate = timeDelta;
            target = targetQuaternion;
            source = transform.rotation;
        }

        protected override void Update()
        {
            base.Update();
            if (!hasRotationLeft)
            {
                return;
            }

            if (Time.deltaTime < timeLeftToRotate)
            {
                transform.rotation =
                    Quaternion.Lerp(source, target, 1 - timeLeftToRotate / lastFullTime);
                timeLeftToRotate -= Time.deltaTime;
            }
            else
            {
                transform.rotation = target;
                hasRotationLeft = false;
            }
        }

        public bool GetGrounded()
        {
            CheckGrounded();
            return IsGrounded;
        }
    }
}
