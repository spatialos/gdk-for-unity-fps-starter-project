using Improbable.Fps.Custommovement;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

public class FpsServerDriver : MonoBehaviour, MyMovementUtils.IMovementStateRestorer, MyMovementUtils.ICustomMovementProcessor
{
    [Require] private ServerMovement.Requirable.Writer writer;

    private CharacterController controller;
    private MyServerMovementDriver driver;
    private SpatialOSComponent spatial;

    private MyMovementUtils.IMovementProcessor[] movementProcessors;
    private readonly MyMovementUtils.TeleportProcessor teleportProcessor = new MyMovementUtils.TeleportProcessor();
    private readonly MyMovementUtils.RemoveWorkerOrigin removeWorkerOrigin = new MyMovementUtils.RemoveWorkerOrigin();


    private void OnEnable()
    {
        controller = GetComponent<CharacterController>();
        driver = GetComponent<MyServerMovementDriver>();
        spatial = GetComponent<SpatialOSComponent>();

        driver.SetCustomProcessor(this);
        driver.SetStateRestorer(this);

        teleportProcessor.Origin = spatial.Worker.Origin;
        removeWorkerOrigin.Origin = spatial.Worker.Origin;

        movementProcessors = new MyMovementUtils.IMovementProcessor[]
        {
            teleportProcessor,
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

    public MovementState Process(CustomInput input, MovementState previousState, float deltaTime)
    {
        var newState = new MovementState();
        var wrappedInput = new ClientRequest()
        {
            Input = input
        };
        for (var i = 0; i < movementProcessors.Length; i++)
        {
            if (!movementProcessors[i].Process(wrappedInput, previousState, ref newState, deltaTime))
            {
                break;
            }
        }

        return newState;
    }

    public void Teleport(Vector3 spawnPosition)
    {
        teleportProcessor.Teleport(spawnPosition);
    }
}
