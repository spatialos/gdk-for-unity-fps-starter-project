using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;

public class MyClientMovementDriver : MonoBehaviour
{
    [Require] private ClientMovement.Requirable.Writer movement;
    [Require] private ServerMovement.Requirable.Reader serverMovement;

    public CharacterController Controller;
    private SpatialOSComponent spatial;
    private CommandFrameSystem commandFrame;

    private int lastServerTimestamp = -1;
    private float lastServerTimestampReceived = -1;

    private float nextDilation = 1f;

    private int lastFrame = -1;
    private MovementState lastMovementState;
    private int confirmedFrame = -1;

    private bool forwardThisFrame;
    private bool backThisFrame;
    private bool leftThisFrame;
    private bool rightThisFrame;
    private bool jumpThisFrame;
    private bool sprintThisFrame;
    private bool aimThisFrame;
    private float yawThisFrame;
    private float pitchThisFrame;

    private const float NetPauseOnBufferSizeMultiplier = 10f;
    private const float NetResumeOnBufferSizeMultiplier = 1.5f;
    private int frameBuffer = 1000;

    private SortedDictionary<int, MovementState> movementState = new SortedDictionary<int, MovementState>();
    private SortedDictionary<int, ClientRequest> inputState = new SortedDictionary<int, ClientRequest>();

    private Queue<ServerResponse> serverResponses = new Queue<ServerResponse>();

    private MyMovementUtils.IMovementProcessor[] movementProcessors = { };

    private MyMovementUtils.IMovementStateRestorer stateRestorer;

    private int debugRow;

    private void Awake()
    {
        debugRow = nextRow;
        nextRow++;
    }

    private void OnEnable()
    {
        spatial = GetComponent<SpatialOSComponent>();
        commandFrame = spatial.World.GetExistingManager<CommandFrameSystem>();
        commandFrame.CurrentFrame = 1;
        movementState[0] = serverMovement.Data.Latest.MovementState;
        UpdateFrameBuffer();
        lastFrame = 0;
        serverMovement.OnServerMovement += OnServerMovement;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetMovementProcessors(MyMovementUtils.IMovementProcessor[] processors)
    {
        movementProcessors = processors;
    }

    public void SetStateRestorer(MyMovementUtils.IMovementStateRestorer restorer)
    {
        stateRestorer = restorer;
    }

    private void UpdateFrameBuffer()
    {
        var rtt = serverMovement.Data.Rtt;

        frameBuffer = rtt > 0 ? MyMovementUtils.CalculateInputBufferSize(rtt) : 100;
    }

    private void Update()
    {
        if (commandFrame.IsPaused())
        {
            if (inputState.Count < frameBuffer * NetResumeOnBufferSizeMultiplier)
            {
                Debug.Log($"[Client {lastFrame}] Resuming network with buffer size {inputState.Count}");
                commandFrame.Resume();
                return;
            }
            else if (inputState.Count < movement.Data.Buffer.Count)
            {
                Debug.Log($"[Client {lastFrame}] Paused, buffer now: {inputState.Count}");
                movement.Send(new ClientMovement.Update()
                {
                    Buffer = new List<ClientRequest>(inputState.Values)
                });
            }
        }

        if (commandFrame.NewFrame)
        {
            while (lastFrame < commandFrame.CurrentFrame)
            {
                lastFrame += 1;

                var input = SendInput();
                movementState.TryGetValue(lastFrame - 1, out var previousState);
                stateRestorer?.Restore(previousState);
                lastMovementState = MyMovementUtils.ApplyInput(input, previousState, movementProcessors);
                movementState[lastFrame] = lastMovementState;
                inputState.Add(lastFrame, input);
            }

            forwardThisFrame = false;
            backThisFrame = false;
            leftThisFrame = false;
            rightThisFrame = false;
            jumpThisFrame = false;
            sprintThisFrame = false;
            aimThisFrame = false;
            commandFrame.ServerAdjustment = nextDilation;

            if (inputState.Count > frameBuffer * NetPauseOnBufferSizeMultiplier)
            {
                Debug.LogWarning($"[Client {lastFrame}] input buffer {inputState.Count} too large ({frameBuffer})");
                commandFrame.Pause();
            }
        }

        ProcessServerResponses();

        // Calculate Partial update.
        var remainder = commandFrame.GetRemainder();
        if (remainder > 0)
        {
            if (!inputState.ContainsKey(lastFrame))
            {
                Debug.LogWarning($"[Client {lastFrame}] No input state found for {lastFrame}");
                return;
            }

            if (!movementState.ContainsKey(lastFrame - 1))
            {
                Debug.LogWarning($"[Client {lastFrame}] No movement state found for {lastFrame - 1}");
                return;
            }

            stateRestorer?.Restore(movementState[lastFrame - 1]);
            MyMovementUtils.ApplyPartialInput(inputState[lastFrame], movementState[lastFrame - 1],
                movementProcessors, remainder);
        }
        else if (remainder < 0)
        {
            if (!inputState.ContainsKey(lastFrame - 1))
            {
                Debug.LogWarning($"[Client {lastFrame}] No input state found for {(lastFrame - 1)}");
                return;
            }

            if (!movementState.ContainsKey(lastFrame - 2))
            {
                Debug.LogWarning($"[Client {lastFrame}] No movement state found for {(lastFrame - 2)}");
                return;
            }

            stateRestorer?.Restore(movementState[lastFrame - 2]);
            MyMovementUtils.ApplyPartialInput(inputState[lastFrame - 1], movementState[lastFrame - 2],
                movementProcessors, CommandFrameSystem.FrameLength + remainder);
        }
    }

    public void AddInput(bool forward = false, bool back = false, bool left = false, bool right = false, bool jump = false, bool sprint = false, bool aim = false, float yaw = -1, float pitch = -1)
    {
        forwardThisFrame |= forward;
        backThisFrame |= back;
        leftThisFrame |= left;
        rightThisFrame |= right;
        jumpThisFrame |= jump;
        sprintThisFrame |= sprint;
        aimThisFrame |= aim;
        if (yaw >= 0)
        {
            yawThisFrame = yaw;
        }

        if (pitchThisFrame >= 0)
        {
            pitchThisFrame = pitch;
        }
    }

    private void OnServerMovement(ServerResponse response)
    {
        // Debug.Log($"[Client {lastFrame}] Received state: {response.Timestamp} CF: {confirmedFrame}");
        serverResponses.Enqueue(response);

        UpdateFrameBuffer();
    }

    private void ProcessServerResponses()
    {
        while (serverResponses.Count > 0)
        {
            var response = serverResponses.Dequeue();

            //Debug.LogFormat($"[{lastFrame}] Server response: {response.Timestamp}");

            if (response.Timestamp < confirmedFrame)
            {
                //Debug.LogWarning($"[Client {lastFrame}] Server resent already confirmed frame: {response.Timestamp}");
                continue;
            }

            if (movementState.ContainsKey(response.Timestamp))
            {
                // Check if server agrees, which it always should.
                var predictionPosition = movementState[response.Timestamp].Position.ToVector3();
                var actualPosition = response.MovementState.Position.ToVector3();
                var distance = Vector3.Distance(predictionPosition, actualPosition);
                if (distance > 0.1f)
                {
                    Debug.LogFormat("Mispredicted cf {0}", response.Timestamp);
                    Debug.LogFormat("Predicted: {0}", predictionPosition);
                    Debug.LogFormat("Actual: {0}", actualPosition);
                    Debug.LogFormat("Diff: {0}", distance);
                    Debug.LogFormat("Replaying input from {0} to {1}", response.Timestamp + 1, lastFrame);

                    // SaveMovementState(response.Timestamp);
                    var previousState = response.MovementState;
                    movementState[response.Timestamp] = previousState;
                    stateRestorer?.Restore(previousState);

                    // Replay inputs until lastFrame, storing movementstates.
                    var i = response.Timestamp + 1;
                    while (i <= lastFrame)
                    {
                        Debug.LogFormat("Input {0}", InputToString(inputState[i]));
                        Debug.LogFormat("Previous Position {0}", movementState[i].Position.ToVector3());
                        Debug.LogFormat("Previous Velocity {0}", movementState[i].Velocity.ToVector3());

                        movementState[i] = MyMovementUtils.ApplyInput(inputState[i], movementState[i - 1], movementProcessors);

                        //Debug.LogFormat("Adjusted Position: {0}", movementState[i].Position.ToVector3());
                        Debug.DrawLine(
                            Controller.transform.position,
                            Controller.transform.position + Vector3.up * 500);
                        i++;
                    }
                }
                else
                {
                    //Debug.LogFormat("[Client] {0} confirmed", response.Timestamp);
                }

                // Debug.Log($"[Client {lastFrame}] Remove confirmed input for {response.Timestamp}");
                confirmedFrame = response.Timestamp;
                inputState.Remove(response.Timestamp);


                // Remove the previous last confirmed state, keep this one around for potential velocity calculation
                movementState.Remove(response.Timestamp - 2);
            }
            else
            {
                Debug.LogWarning($"[Client {lastFrame}] Don't have movement state for cf {response.Timestamp}");
            }

            // update dilation
            // commandFrame.ServerAdjustment = response.TimeDelta;
            // commandFrame.ServerAdjustment = response.NextDilation / 100000f;
            nextDilation = response.NextDilation / 100000f;
            lastServerTimestamp = response.AppliedDilation;
            lastServerTimestampReceived = Time.time;
        }

        var keys = inputState.Keys.ToArray();
        foreach (var key in keys)
        {
            if (key < confirmedFrame)
            {
                inputState.Remove(key);
            }
        }
    }

    private ClientRequest SendInput()
    {
        var clientRequest = new ClientRequest
        {
            ForwardPressed = forwardThisFrame,
            BackPressed = backThisFrame,
            LeftPressed = leftThisFrame,
            RightPressed = rightThisFrame,
            JumpPressed = jumpThisFrame,
            SprintPressed = sprintThisFrame,
            AimPressed = aimThisFrame,
            CameraYaw = (int) (yawThisFrame * 100000f),
            CameraPitch = (int) (pitchThisFrame * 100000f),
            Timestamp = lastFrame,
            AppliedDilation = lastServerTimestamp + (Time.time - lastServerTimestampReceived) * 100000f
        };

        var unackedInput = new List<ClientRequest>(inputState.Values) { clientRequest };

        // Debug.Log($"[Client:{lastFrame}] Send Input");
        // movement.SendClientInput(clientRequest);
        movement.Send(new ClientMovement.Update
        {
            Buffer = unackedInput
        });

        // //Debug.LogFormat("[Client] Sent {0}", clientRequest.Timestamp);
        return clientRequest;
    }

    public MovementState GetLatestState()
    {
        return lastMovementState;
    }

    private static int nextRow = 0;

    private void OnGUI()
    {
        if (!MyMovementUtils.ShowDebug)
        {
            return;
        }

        var minKey = inputState.Count > 0 ? inputState.Keys.Min() : -1;
        var maxKey = inputState.Count > 0 ? inputState.Keys.Max() : -1;

        GUI.Label(new Rect(10, 10 + debugRow * 20, 700, 20),
            $"Fr: {(CommandFrameSystem.FrameLength * 1000f)}," +
            $"Svr Adj: {commandFrame.ServerAdjustment}," +
            $"Adj: {(commandFrame.currentFrameAdjustment * 1000f)}," +
            $"FL: {commandFrame.adjustmentFramesLeft}," +
            $"CF: {confirmedFrame}," +
            $"LB: {inputState.Count} ({serverMovement.Data.BufferSizeAvg})," +
            $"SB: {movement.Data.Buffer.Count}," +
            $"B: {minKey} - {maxKey}," +
            $"{(commandFrame.IsPaused() ? "NET PAUSED" : "")}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(Controller.transform.position, 0.1f);

        float c = 0;
        for (var i = lastFrame; movementState.ContainsKey(i); i--)
        {
            Gizmos.color = Color.Lerp(Color.red, Color.white, c / movementState.Count);
            Gizmos.DrawWireSphere(movementState[i].Position.ToVector3(), 0.5f);
            c += 1;
        }
    }

    private string InputToString(ClientRequest request)
    {
        return string.Format("[F:{0} B:{1} R:{2} L:{3}, J:{4}, S:{5}, Yaw:{6}, Pitch:{7}]",
            request.ForwardPressed, request.BackPressed,
            request.RightPressed, request.LeftPressed,
            request.JumpPressed, request.SprintPressed,
            request.CameraYaw, request.CameraPitch);
    }
}
