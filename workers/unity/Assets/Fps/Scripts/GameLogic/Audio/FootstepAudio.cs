using Improbable.Gdk.Movement;
using UnityEngine;

namespace Fps.GameLogic.Audio
{
    public class FootstepAudio : MonoBehaviour
    {
        [SerializeField] private Transform LeftFoot;
        [SerializeField] private Transform RightFoot;

        private AudioRandomiser leftAudio;
        private AudioRandomiser rightAudio;
        private GroundChecker groundChecker;
        private FpsDriver fpsDriver;

        private int previousLeftRaycast;
        private int previousRightRaycast;

        private bool previousFootGroundedLeft;
        private bool previousFootGroundedRight;
        private bool previousIsGroundedState;
        private bool isAuthoritative;

        private bool previousInAirState;

        private readonly float hitRayLength = 0.1f;

        private RaycastHit[] results = new RaycastHit[1];

        private void Awake()
        {
            leftAudio = LeftFoot.GetComponent<AudioRandomiser>();
            rightAudio = RightFoot.GetComponent<AudioRandomiser>();
            groundChecker = GetComponentInParent<GroundChecker>();
            fpsDriver = GetComponentInParent<FpsDriver>();
        }

        private void Start()
        {
            previousFootGroundedLeft = IsFootGrounded(LeftFoot);
            previousFootGroundedRight = IsFootGrounded(RightFoot);
            isAuthoritative = fpsDriver != null;
        }

        private void Update()
        {
            var currentFootGroundedLeft = IsFootGrounded(LeftFoot);
            var currentFootGroundedRight = IsFootGrounded(RightFoot);
            var currentIsGroundedState = groundChecker.Grounded;

            if (currentIsGroundedState != null && currentIsGroundedState && !previousIsGroundedState)
            {
                // Player landed this frame
                leftAudio.Play();
                rightAudio.Play();
            }

            if (!previousFootGroundedLeft && currentFootGroundedLeft)
            {
                // Left foot grounded this frame.

                if (!isAuthoritative)
                {
                    leftAudio.Play();
                }

                if (isAuthoritative && NavKeysDown())
                {
                    // Player is authoritative, only play footstep noise when WASD are pressed
                    leftAudio.Play();
                }
            }

            if (!previousFootGroundedRight && currentFootGroundedRight)
            {
                // Right foot grounded this frame.

                if (!isAuthoritative)
                {
                    rightAudio.Play();
                }

                if (isAuthoritative && NavKeysDown())
                {
                    // Player is authoritative, only play footstep noise when WASD are pressed
                    rightAudio.Play();
                }
            }

            previousFootGroundedLeft = currentFootGroundedLeft;
            previousFootGroundedRight = currentFootGroundedRight;
            previousIsGroundedState = currentIsGroundedState;
        }

        private bool IsFootGrounded(Transform foot)
        {
            return Physics.RaycastNonAlloc(foot.position, Vector3.down, results, hitRayLength) == 1;
        }

        private bool NavKeysDown()
        {
            return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        }
    }
}

