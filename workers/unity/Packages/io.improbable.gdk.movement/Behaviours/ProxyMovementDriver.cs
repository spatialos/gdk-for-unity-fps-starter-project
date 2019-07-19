using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.TransformSynchronization;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Improbable.Gdk.Movement
{
    public class ProxyMovementDriver : GroundCheckingDriver
    {
        [Require] private ServerMovementReader serverMovementReader;
        [Require] private ClientRotationReader clientRotationReader;

        [SerializeField] private RotationConstraints rotationConstraints = new RotationConstraints
        {
            XAxisRotation = true,
            YAxisRotation = true,
            ZAxisRotation = true
        };

        private LinkedEntityComponent LinkedEntityComponent;
        private Vector3 origin;

        //Rotation Variables
        private float timeLeftToRotate;
        private float lastFullTime;
        private Quaternion source;
        private Quaternion target;
        private bool hasRotationLeft;

        private void OnEnable()
        {
            LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
            origin = LinkedEntityComponent.Worker.Origin;

            serverMovementReader.OnMovementUpdate += ServerMovementUpdate;
            clientRotationReader.OnRotationUpdate += ClientRotationUpdate;

            ClientRotationUpdate(clientRotationReader.Data.Rotation);
            ServerMovementUpdate(serverMovementReader.Data.Movement);
        }

        private void ClientRotationUpdate(RotationInfo rotationInfo)
        {
            hasRotationLeft = true;
            lastFullTime = timeLeftToRotate = rotationInfo.TimeDelta;
            target = TransformUtils.ToUnityQuaternion(rotationInfo.Rotation);
            source = transform.rotation;
        }

        private void ServerMovementUpdate(MovementInfo movementInfo)
        {
            if (serverMovementReader.Authority != Authority.NotAuthoritative)
            {
                return;
            }

            Interpolate(TransformUtils.ToUnityVector3(movementInfo.Position) + origin, movementInfo.TimeDelta);
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
    }
}
