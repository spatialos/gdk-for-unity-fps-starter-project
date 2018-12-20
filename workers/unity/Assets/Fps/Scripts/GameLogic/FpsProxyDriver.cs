using Fps;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

[RequireComponent(typeof(FpsAnimator), typeof(MyProxyMovementDriver))]
public class FpsProxyDriver : MonoBehaviour
{
    [Require] private ServerMovement.Requirable.Reader reader;

    [SerializeField] public GameObject ControllerProxy;

    private FpsAnimator animator;
    private MyProxyMovementDriver movement;
    private FpsMovement fpsMovement;
    private CharacterController controller;

    private Vector3 origin;

    private void OnEnable()
    {
        origin = GetComponent<SpatialOSComponent>().Worker.Origin;
        animator = GetComponent<FpsAnimator>();
        movement = GetComponent<MyProxyMovementDriver>();
    }

    private void Start()
    {
        ControllerProxy.transform.parent = null;
        ControllerProxy.name = $"{name} Controller Proxy";

        //TODO Params don't matter since Process is never called on Proxy, should refactor this.
        fpsMovement = new FpsMovement(ControllerProxy.GetComponent<CharacterController>(), Vector3.zero);
        controller = ControllerProxy.GetComponent<CharacterController>();
    }

    private void OnDestroy()
    {
        if (ControllerProxy != null)
        {
            Destroy(ControllerProxy);
        }
    }

    private void Update()
    {
        if (MyMovementUtils.GetProxyState(out var t, out var from, out var to, movement, fpsMovement))
        {
            var fromPosition = from.StandardMovement.Position.ToVector3() + origin;
            var toPosition = to.StandardMovement.Position.ToVector3() + origin;
            var rot = controller.transform.rotation.eulerAngles;
            var fromRot = Quaternion.Euler(from.Pitch / 100000f, from.Yaw / 100000f, rot.z);
            var toRot = Quaternion.Euler(to.Pitch / 100000f, to.Yaw / 100000f, rot.z);

            var newRot = Quaternion.Slerp(fromRot, toRot, t);
            var newRotEuler = newRot.eulerAngles;

            controller.transform.position = Vector3.Lerp(fromPosition, toPosition, t);
            controller.transform.rotation = Quaternion.Euler(rot.x, newRotEuler.y, rot.z);

            animator.SetAiming(from.IsAiming);
            animator.SetGrounded(from.IsGrounded);
            var vel = Vector3.Lerp(from.StandardMovement.Velocity.ToVector3(), to.StandardMovement.Velocity.ToVector3(), t);
            vel.y = 0;
            animator.SetVelocity(vel);
            animator.SetPitch(newRot.eulerAngles.x);

            transform.position = ControllerProxy.transform.position;
            transform.rotation = ControllerProxy.transform.rotation;
        }
    }
}
