using System.Collections.Generic;
using System.Linq;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MyServerMovementDriver : MonoBehaviour, MyMovementUtils.IMovementStateRestorer
{
    [Require] private ClientMovement.Requirable.Reader clientInput;
    [Require] private ServerMovement.Requirable.Writer server;
    [Require] private Position.Requirable.Writer spatialPosition;

    private CharacterController controller;
    private SpatialOSComponent spatial;
    private CommandFrameSystem commandFrame;

    private Vector3 origin;

    private int lastFrame = -1;
    private int firstFrame = -1;
    private int frameBuffer = 5;
    private int lastSentFrame = -1;

    private float clientDilation = 0f;
    private int bufferCount = 0;
    private float bufferAvg = 0;
    private const float BufferAlpha = 0.99f;
    private int emptySamples = 0;

    private float rtt = (5 - 1) * 2 * CommandFrameSystem.FrameLength;
    private const float RttAlpha = 0.95f;

    private const int PositionRate = 30;
    private int positionTick = 0;

    private ClientRequest lastInput = new ClientRequest() { Timestamp = -1 };

    private readonly List<ClientRequest> clientInputs = new List<ClientRequest>();
    private readonly Dictionary<int, MovementState> movementState = new Dictionary<int, MovementState>();

    private readonly MyMovementUtils.RemoveWorkerOrigin removeWorkerOrigin = new MyMovementUtils.RemoveWorkerOrigin();
    private readonly MyMovementUtils.TeleportProcessor teleportProcessor = new MyMovementUtils.TeleportProcessor();

    private MyMovementUtils.IMovementProcessor[] movementProcessors;

    private MyMovementUtils.IMovementStateRestorer stateRestorer;

    public int workerIndex = -1;

    private static int nextWorkerIndex = 0;

    private void Awake()
    {
        workerIndex = nextWorkerIndex;
        nextWorkerIndex++;

        controller = GetComponent<CharacterController>();
        renderLine = nextRenderLine;
        nextRenderLine += 1;

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

        stateRestorer = this;
    }

    private void OnEnable()
    {
        spatial = GetComponent<SpatialOSComponent>();
        commandFrame = spatial.World.GetExistingManager<CommandFrameSystem>();

        teleportProcessor.Origin = spatial.Worker.Origin;
        removeWorkerOrigin.Origin = spatial.Worker.Origin;

        clientInput.BufferUpdated += ClientInputOnBufferUpdated;

        lastFrame = commandFrame.CurrentFrame;

        clientInputs.Clear();
        movementState.Clear();

        bufferAvg = server.Data.BufferSizeAvg;
        rtt = server.Data.Rtt;

        firstFrame = -1;

        origin = spatial.Worker.Origin;
    }

    private void ClientInputOnBufferUpdated(List<ClientRequest> buffer)
    {
        if (lastFrame < 0)
        {
            return;
        }

        if (firstFrame < 0)
        {
            // Debug.Log($"[Server-{workerIndex}] First Frame Setup.");
            firstFrame = lastFrame + frameBuffer;
            movementState[firstFrame - 1] = server.Data.Latest.MovementState;

            //Debug.Log($"[{lastFrame}] First Input received, applied on {firstFrame}, offset: {clientFrameOffset}");
        }

        // Debug.Log($"[Server {lastFrame}] Clearing {clientInputs.Count} inputs.");
        clientInputs.Clear();
        foreach (var request in buffer)
        {
            if (request.Timestamp > lastSentFrame)
            {
                clientInputs.Add(request);
            }
        }
        // Debug.Log($"[Server {lastFrame}] Added {clientInputs.Count} inputs.");

        if (firstFrame < lastFrame && clientInputs.Count > 0)
        {
            UpdateRtt(clientInputs.Last());
        }
    }

    private void UpdateRtt(ClientRequest request)
    {
        if (request.AppliedDilation > 0)
        {
            var sample = Time.time - (request.AppliedDilation / 100000f);
            rtt = RttAlpha * rtt + (1 - RttAlpha) * sample;
            frameBuffer = MyMovementUtils.CalculateInputBufferSize(rtt);
            // Debug.Log($"[Server {lastFrame}] Update Frame Buffer to {frameBuffer}");
        }
    }

    private void Update()
    {
        if (commandFrame.NewFrame)
        {
            if (firstFrame < 0)
            {
                lastFrame = commandFrame.CurrentFrame;
                return;
            }

            while (lastFrame <= commandFrame.CurrentFrame)
            {
                lastFrame += 1;

                if (lastFrame < firstFrame)
                {
                    //Debug.LogFormat($"[{lastFrame}] Skipping frame until first frame: {firstFrame}");
                    continue;
                }

                if (clientInputs.Count > 0)
                {
                    lastInput = clientInputs[0];
                    clientInputs.RemoveAt(0);
                    // Debug.Log($"[Server {lastFrame}] Dequeue Client input {lastFrame} ({clientInputs.Count})");
                }
                else
                {
                    lastInput.Timestamp = lastInput.Timestamp + 1;
                }

                movementState.TryGetValue(lastFrame - 1, out var previousState);
                // shouldn't need to call restore here.
                stateRestorer.Restore(previousState);
                var state = MyMovementUtils.ApplyInput(lastInput, previousState, movementProcessors);
                movementState[lastFrame] = state;
                SendMovement(state);

                // Remove movement state from 10 frames ago
                movementState.Remove(lastFrame - 10);
            }

            UpdateBufferAdjustment();
        }
    }

    private void SendMovement(MovementState state)
    {
        // Debug.Log($"[Server:{lastFrame}] Send Response: {clientFrame}");

        var response = new ServerResponse
        {
            MovementState = state,
            Timestamp = lastInput.Timestamp,
            Yaw = lastInput.CameraYaw,
            Pitch = lastInput.CameraPitch,
            Aiming = lastInput.AimPressed,
            NextDilation = (int) (clientDilation * 100000f),
            AppliedDilation = (int) (Time.time * 100000f)
        };
        server.SendServerMovement(response);
        var update = new ServerMovement.Update
        {
            Latest = response,
            BufferSizeAvg = bufferAvg,
            Rtt = rtt
        };
        server.Send(update);

        positionTick -= 1;
        if (positionTick <= 0)
        {
            var pos = state.Position.ToVector3();
            positionTick = PositionRate;
            spatialPosition.Send(new Position.Update()
            {
                Coords = new Option<Coordinates>(new Coordinates(pos.x, pos.y, pos.z))
            });
        }

        if (lastInput.Timestamp <= lastSentFrame)
        {
            Debug.LogWarning($"[Server {lastFrame}] Sending Duplicate Frame. Last Frame: {lastSentFrame}, sending {lastInput.Timestamp}");
        }

        lastSentFrame = lastInput.Timestamp;
    }

    private void UpdateBufferAdjustment()
    {
        bufferCount = clientInputs.Count;
        bufferAvg = BufferAlpha * bufferAvg + (1 - BufferAlpha) * bufferCount;
        if (bufferCount == 0)
        {
            emptySamples++;
        }

        if (lastFrame % CommandFrameSystem.AdjustmentFrames == 0)
        {
            var error = bufferAvg - frameBuffer;
            if (error < -0.3f)
            {
                clientDilation = -1;
            }
            else if (error > 0.3f)
            {
                clientDilation = 1;
            }

            emptySamples = 0;
        }
        else
        {
            clientDilation = 0;
        }
    }

    public void Teleport(Vector3 spawnPosition)
    {
        //Debug.LogFormat("Mark Teleport Processor with position: {0}", spawnPosition);
        teleportProcessor.Teleport(spawnPosition);
    }

    private static int nextRenderLine = 0;
    private int renderLine = 0;

    private void OnGUI()
    {
        if (!MyMovementUtils.ShowDebug)
        {
            return;
        }

        var delta = clientInputs.Count - frameBuffer;

        GUI.Label(new Rect(10, 100 + renderLine * 20, 700, 20),
            string.Format("Input Buffer Sample: {0:00}, Avg: {1:00.00}, cd: {2:00.00}, RTT: {3:00.0}, B: {4}, Empty: {5}",
                bufferCount, bufferAvg, clientDilation, rtt * 1000f, frameBuffer, emptySamples));

        GUI.Label(new Rect(10, 300, 700, 20),
            string.Format("Frame: {0}, Length: {1:00.0}, Remainder: {2:00.0}",
                commandFrame.CurrentFrame, CommandFrameSystem.FrameLength * 1000f, commandFrame.GetRemainder() * 1000f));
    }

    public void Restore(MovementState state)
    {
        controller.transform.position = state.Position.ToVector3() + spatial.Worker.Origin;
    }
}
