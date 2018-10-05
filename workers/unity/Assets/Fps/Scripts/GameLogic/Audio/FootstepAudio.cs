using UnityEngine;

namespace Fps.GameLogic.Audio
{
    public class FootstepAudio : MonoBehaviour
    {
        [SerializeField] private Transform LeftFoot;
        [SerializeField] private Transform RightFoot;

        private AudioRandomiser leftAudio;
        private AudioRandomiser rightAudio;

        private int previousLeftRaycast;
        private int previousRightRaycast;

        private bool previousFootGroundedLeft;
        private bool previousFootGroundedRight;

        private readonly float hitRayLength = 0.1f; 

        private RaycastHit[] results = new RaycastHit[1];

        private void Awake()
        {
            leftAudio = LeftFoot.GetComponent<AudioRandomiser>();
            rightAudio = RightFoot.GetComponent<AudioRandomiser>();
        }

        private void Start()
        {
            previousFootGroundedLeft = IsFootGrounded(LeftFoot);
            previousFootGroundedRight = IsFootGrounded(RightFoot);
        }

        private void Update()
        {
            var currentFootGroundedLeft = IsFootGrounded(LeftFoot);
            var currentFootGroundedRight = IsFootGrounded(RightFoot);

            if (!previousFootGroundedLeft && currentFootGroundedLeft)
            {
                // Left foot grounded this frame.
                leftAudio.Play();
            }

            if (!previousFootGroundedRight && currentFootGroundedRight)
            {
                // Right foot grounded this frame.
                rightAudio.Play();
            }

            previousFootGroundedLeft = currentFootGroundedLeft;
            previousFootGroundedRight = currentFootGroundedRight; 
        }

        private bool IsFootGrounded(Transform foot)
        {
            return Physics.RaycastNonAlloc(foot.position, Vector3.down, results, hitRayLength) == 1;
        }
    }
}

