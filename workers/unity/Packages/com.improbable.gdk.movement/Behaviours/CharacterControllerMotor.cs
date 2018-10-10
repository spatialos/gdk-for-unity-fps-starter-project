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
        private IMotorExtension[] motorExtensions;

        // Ground checking
        public bool IsGrounded { get; private set; }

        [Tooltip("Uses an overlap sphere of this radius from the character's feet to check for collision.")]
        [SerializeField]
        private float groundedRadius = 0.05f;

        [SerializeField] private LayerMask groundLayerMask = ~0;
        private readonly Collider[] groundedOverlapSphereArray = new Collider[1];

        protected virtual void Awake()
        {
            characterController = GetComponent<CharacterController>();
            motorExtensions = GetComponents<IMotorExtension>();
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
        public virtual void MoveWithExtensions(Vector3 toMove)
        {
            if (characterController.enabled)
            {
                foreach (var extension in motorExtensions)
                {
                    extension.BeforeMove(toMove);
                }

                characterController.Move(toMove);
            }
        }

        protected void CheckGrounded()
        {
            IsGrounded = Physics.OverlapSphereNonAlloc(transform.position, groundedRadius, groundedOverlapSphereArray,
                groundLayerMask) > 0;
        }

        protected void CheckOverrideInAir()
        {
            foreach (var extension in motorExtensions)
            {
                if (extension.IsOverrideAir())
                {
                    IsGrounded = true;
                    return;
                }
            }
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
                    transform.position += distanceToMove;
                    distanceLeftToMove -= distanceToMove;
                    timeLeftToMove -= Time.deltaTime;
                }
                else
                {
                    transform.position += distanceLeftToMove;
                    hasMovementLeft = false;
                }
            }
        }
    }
}
