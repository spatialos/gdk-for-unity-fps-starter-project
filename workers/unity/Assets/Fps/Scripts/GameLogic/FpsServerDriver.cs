using Improbable.Fps.Custommovement;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using Improbable.Worker.CInterop;
using UnityEngine;

public class FpsServerDriver : MonoBehaviour, MyMovementUtils.IMovementStateRestorer
{
    [Require] private ServerMovement.Requirable.Writer writer;

    private CharacterController controller;
    private MyServerMovementDriver driver;
    private FpsMovement fpsMovement;
    private SpatialOSComponent spatial;

    private MyMovementUtils.IMovementProcessorOLD[] movementProcessorsOld;
    private readonly MyMovementUtils.TeleportProcessorOld teleportProcessorOld = new MyMovementUtils.TeleportProcessorOld();
    private readonly MyMovementUtils.RemoveWorkerOrigin removeWorkerOrigin = new MyMovementUtils.RemoveWorkerOrigin();


    private void OnEnable()
    {
        controller = GetComponent<CharacterController>();
        driver = GetComponent<MyServerMovementDriver>();
        spatial = GetComponent<SpatialOSComponent>();
        fpsMovement = new FpsMovement(gameObject, controller);
        driver.SetCustomProcessor(fpsMovement);
        driver.SetStateRestorer(this);

        teleportProcessorOld.Origin = spatial.Worker.Origin;
        removeWorkerOrigin.Origin = spatial.Worker.Origin;

        movementProcessorsOld = new MyMovementUtils.IMovementProcessorOLD[]
        {
            teleportProcessorOld,
            new StandardMovement(),
            new MyMovementUtils.SprintCooldown(),
            new JumpMovement(),
            new MyMovementUtils.Gravity(),
            new MyMovementUtils.TerminalVelocity(),
            new MyMovementUtils.CharacterControllerMovement(controller),
            removeWorkerOrigin,
            new IsGroundedMovement(),
            new MyMovementUtils.AdjustVelocity(),
        };
    }

    public void Restore(MovementState state)
    {
        controller.transform.position = state.Position.ToVector3() + spatial.Worker.Origin;
    }

    public void Teleport(Vector3 spawnPosition)
    {
        teleportProcessorOld.Teleport(spawnPosition);
    }
}
