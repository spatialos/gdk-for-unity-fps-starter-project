using System.Collections.Generic;
using System.Linq;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MyServerMovementDriver : MonoBehaviour
{
    [Require] private ClientMovement.Requirable.Reader clientInput;
    [Require] private ServerMovement.Requirable.Writer server;
    [Require] private Position.Requirable.Writer spatialPosition;

    private CharacterController controller;
    private SpatialOSComponent spatial;
    private CommandFrameSystem commandFrame;

    private Vector3 origin;

    private int lastFrame = -1;
    private bool hasInput = false;

    private int nextExpectedInput = -1;
    private int nextServerFrame = -1;
    private int frameBuffer = 5;

    private float clientDilation = 1f;

    private float rtt = (5 - 1) * 2 * CommandFrameSystem.FrameLength;
    private const float RttAlpha = 0.95f;

    private const int PositionRate = 30;
    private int positionTick = 0;

    private ClientRequest lastInput;

    private readonly Queue<ClientRequest> clientInputs = new Queue<ClientRequest>();
    private readonly Dictionary<int, MovementState> movementState = new Dictionary<int, MovementState>();

    private readonly MyMovementUtils.IMovementProcessor[] movementProcessors =
    {
        new StandardMovement(),
        new JumpMovement(),
        new MyMovementUtils.Gravity(),
        new MyMovementUtils.TerminalVelocity(),
    };

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        renderLine = nextRenderLine;
        nextRenderLine += 1;
    }

    private void OnEnable()
    {
        spatial = GetComponent<SpatialOSComponent>();
        commandFrame = spatial.World.GetExistingManager<CommandFrameSystem>();

        clientInput.OnClientInput += OnClientInputReceived;

        origin = spatial.Worker.Origin;
    }

    private void OnClientInputReceived(ClientRequest request)
    {
        // Debug.LogFormat("[Server] Received client frame {0} on frame {1}",
        //     request.Timestamp, lastFrame);
        UpdateRtt(request);

        if (lastFrame < 0)
        {
            return;
        }

        // If this is the first client input, use it for the next cf + Buffer.
        if (!hasInput)
        {
            nextExpectedInput = request.Timestamp;
            nextServerFrame = lastFrame + frameBuffer;

            Debug.LogFormat("[Server] First Client frame {0}, applied on server frame {1}. Current frame {2}",
                nextExpectedInput, nextServerFrame, lastFrame);

            hasInput = true;
        }

        if (request.Timestamp != nextExpectedInput)
        {
            Debug.LogWarningFormat(
                "[Server] Expected input frame {0}, but received {1}", nextExpectedInput, request.Timestamp);
        }

        request.Movement.X = nextServerFrame;

        clientInputs.Enqueue(request);

        nextServerFrame++;
        nextExpectedInput++;
    }

    private void UpdateRtt(ClientRequest request)
    {
        if (request.AppliedDilation > 0)
        {
            var sample = Time.time - (request.AppliedDilation / 100000f);
            rtt = RttAlpha * rtt + (1 - RttAlpha) * sample;
            frameBuffer = Mathf.CeilToInt(rtt / (2 * CommandFrameSystem.FrameLength) + 1);
        }
    }

    private void Update()
    {
        if (commandFrame.NewFrame)
        {
            if (!hasInput)
            {
                lastFrame = commandFrame.CurrentFrame;
                return;
            }

            while (lastFrame <= commandFrame.CurrentFrame)
            {
                lastFrame += 1;

                if (clientInputs.Count > 0)
                {
                    var nextInputFrame = clientInputs.Peek().Movement.X;
                    if (nextInputFrame == lastFrame)
                    {
                        lastInput = clientInputs.Dequeue();
                    }
                    else if (nextInputFrame < lastFrame)
                    {
                        Debug.LogWarningFormat("Next Input for frame: {0}, but next frame is: {1}",
                            nextInputFrame, lastFrame);
                    }
                }
                else
                {
                    nextServerFrame += 1;
                }

                MyMovementUtils.ApplyInput(
                    controller, lastInput, lastFrame, GetVelocity(lastFrame), movementProcessors);

                var state = SaveMovementState();
                SendMovement(state);

                // Remove movement state from 10 frames ago
                movementState.Remove(lastFrame - 10);
                MyMovementUtils.CleanProcessors(movementProcessors, lastFrame - 10);
            }

            UpdateDilation();
        }
    }


    private MovementState SaveMovementState()
    {
        // Debug.LogFormat("[Server] {0} = {1}", ToClienfCf(lastFrame), controller.transform.position - origin);
        var state = new MovementState((controller.transform.position - origin).ToIntAbsolute());
        movementState.Add(lastFrame, state);
        return state;
    }

    private readonly Queue<float> bufferSizeQueue = new Queue<float>(100);

    private void UpdateDilation()
    {
        // Don't do anything until the buffer's full for the first time.

        if (bufferSizeQueue.Count < 100)
        {
            bufferSizeQueue.Enqueue(clientInputs.Count);
            return;
        }

        bufferSizeQueue.Dequeue();
        bufferSizeQueue.Enqueue(clientInputs.Count);

        var error = bufferSizeQueue.Average() - frameBuffer;

        clientDilation = lastFrame % 100 == 0 ? Mathf.FloorToInt(error) : 0;
    }

    private void SendMovement(MovementState state)
    {
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
        var update = new ServerMovement.Update { Latest = response };
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
    }

    private Vector3 GetVelocity(int frame)
    {
        // return the difference of the previous 2 movement states.
        if (movementState.TryGetValue(frame - 2, out var before) && movementState.TryGetValue(frame - 1, out var after))
        {
            return (after.Position.ToVector3() - before.Position.ToVector3()) / MyMovementUtils.FrameLength;
        }

        return Vector3.zero;
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
            string.Format("Input Buffer: {0:00}, d: {1:00.00}, cd: {2:00.00}, RTT: {3:00.0}, B: {4}",
                clientInputs.Count, delta, clientDilation, rtt * 1000f, frameBuffer));

        GUI.Label(new Rect(10, 300, 700, 20),
            string.Format("Frame: {0}, Length: {1:00.0}, Remainder: {2:00.0}",
                commandFrame.CurrentFrame, CommandFrameSystem.FrameLength * 1000f, commandFrame.GetRemainder() * 1000f));
    }
}
