using Fps;
using UnityEngine;

[RequireComponent(typeof(FpsAnimator), typeof(MyProxyMovementDriver))]
public class FpsProxyDriver : MonoBehaviour
{
    [SerializeField] public GameObject ControllerProxy;

    private FpsAnimator animator;
    private MyProxyMovementDriver movement;
    private CommandFrameSystem commandFrame;

    private Vector3 velocity;

    private void Awake()
    {
        animator = GetComponent<FpsAnimator>();
        movement = GetComponent<MyProxyMovementDriver>();
    }

    private void Start()
    {
        ControllerProxy.transform.parent = null;
        movement.Controller = ControllerProxy.GetComponent<CharacterController>();
    }

    private void Update()
    {
        // fpsAnimator.SetAiming(gunState.Data.IsAiming);
        animator.SetGrounded(MyMovementUtils.IsGrounded(movement.Controller));
        animator.SetMovement(movement.GetVelocity());
        animator.SetPitch(movement.GetPitch());

        var newPosition = Vector3.Lerp(transform.position, ControllerProxy.transform.position, 0.5f);
        velocity = (newPosition - transform.position) / Time.deltaTime;
        transform.position = newPosition;
        transform.rotation = Quaternion.Slerp(transform.rotation, ControllerProxy.transform.rotation, 0.5f);
    }
}
