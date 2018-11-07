using Fps;
using UnityEngine;

[RequireComponent(typeof(FpsAnimator), typeof(MyProxyMovementDriver))]
public class FpsProxyDriver : MonoBehaviour
{
    private FpsAnimator animator;
    private MyProxyMovementDriver movement;
    private CommandFrameSystem commandFrame;

    private void Awake()
    {
        animator = GetComponent<FpsAnimator>();
        movement = GetComponent<MyProxyMovementDriver>();
    }

    private void Update()
    {
        // fpsAnimator.SetAiming(gunState.Data.IsAiming);
        animator.SetGrounded(MyMovementUtils.IsGrounded(movement.Controller));
        animator.SetMovement(movement.GetVelocity());
        animator.SetPitch(movement.GetPitch());
    }
}
