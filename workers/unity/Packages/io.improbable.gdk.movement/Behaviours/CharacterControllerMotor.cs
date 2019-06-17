using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Improbable.Gdk.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerMotor : MonoBehaviour
    {
        //CSP
        private readonly Queue<MovementRequest> requests = new Queue<MovementRequest>();
        private Vector3 cumulativeMovement;
        private float cumulativeTimeDelta;
        private bool anyMovement;

        //Interpolation
        private bool hasMovementLeft;
        private float timeLeftToMove;
        private Vector3 distanceLeftToMove;
        private int messageStamp;

        [Tooltip(
            "When interpolating motion, if the (squared) distance travelled is greater than this, teleport instead of interpolating.")]
        [SerializeField]
        private float teleportDistanceSqrd = 25f;

        private CharacterController characterController;
        protected IMotorExtension[] MotorExtensions;

        protected virtual void Awake()
        {
            characterController = GetComponent<CharacterController>();
            MotorExtensions = GetComponents<IMotorExtension>();
        }

        public bool HasEnoughMovement(float threshold, out Vector3 movement, out float timeDelta, out bool anyMovement,
            out int messageStamp)
        {
            movement = cumulativeMovement;
            timeDelta = cumulativeTimeDelta;
            anyMovement = this.anyMovement;
            messageStamp = this.messageStamp;
            return cumulativeTimeDelta > threshold;
        }

        public void Reset()
        {
            requests.Enqueue(new MovementRequest(messageStamp, cumulativeMovement));
            cumulativeMovement = Vector3.zero;
            cumulativeTimeDelta = 0;
            anyMovement = false;
            messageStamp++;
        }

        public void MoveFrame(Vector3 toMove)
        {
            cumulativeTimeDelta += Time.deltaTime;
            var before = transform.position;
            MoveWithExtensions(toMove);
            var delta = transform.position - before;
            anyMovement |= delta.sqrMagnitude / Time.deltaTime > 0;
            cumulativeMovement += delta;
        }

        public void NoMovement()
        {
            cumulativeTimeDelta += Time.deltaTime;
        }

        public void Reconcile(Vector3 position, int timestamp)
        {
            transform.position = position;
            foreach (var each in requests.ToList())
            {
                if (each.Timestamp <= timestamp)
                {
                    requests.Dequeue();
                }
                else
                {
                    Move(each.Movement);
                }
            }

            Move(cumulativeMovement);
        }

        public virtual void Move(Vector3 toMove)
        {
            if (characterController.enabled)
            {
                characterController.Move(toMove);
            }
        }

        // Separate from regular move, as we only want the extensions applied to the movement once.
        public void MoveWithExtensions(Vector3 toMove)
        {
            if (!characterController.enabled)
            {
                return;
            }

            foreach (var extension in MotorExtensions)
            {
                extension.BeforeMove(toMove);
            }

            Move(toMove);
        }

        public void Interpolate(Vector3 target, float timeDelta)
        {
            distanceLeftToMove = target - transform.position;
            var sqrMagnitude = distanceLeftToMove.sqrMagnitude;
            hasMovementLeft = sqrMagnitude < teleportDistanceSqrd || timeDelta != 0;
            if (hasMovementLeft)
            {
                timeLeftToMove = timeDelta;
            }
            else
            {
                transform.position = target;
            }
        }

        protected virtual void Update()
        {
            if (hasMovementLeft)
            {
                if (Time.deltaTime < timeLeftToMove)
                {
                    var percentageToMove = Time.deltaTime / timeLeftToMove;
                    var distanceToMove = distanceLeftToMove * percentageToMove;
                    Move(distanceToMove);
                    distanceLeftToMove -= distanceToMove;
                    timeLeftToMove -= Time.deltaTime;
                }
                else
                {
                    Move(distanceLeftToMove);
                    hasMovementLeft = false;
                }
            }
        }
    }
}
