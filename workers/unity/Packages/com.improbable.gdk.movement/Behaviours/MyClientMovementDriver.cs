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

    private const float NetPauseOnBufferSizeMultiplier = 10f;
    private const float NetResumeOnBufferSizeMultiplier = 1.5f;
    private int frameBuffer = 1000;

    private SortedDictionary<int, MovementState> movementState = new SortedDictionary<int, MovementState>();
    private SortedDictionary<int, ClientRequest> inputState = new SortedDictionary<int, ClientRequest>();

    private Queue<ServerResponse> serverResponses = new Queue<ServerResponse>();

    private IMovementProcessor customProcessor;

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
        serverMovement.LatestUpdated += ServerMovementOnLatestUpdated;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ServerMovementOnLatestUpdated(ServerResponse response)
    {
        serverResponses.Enqueue(response);

        UpdateFrameBuffer();
    }

    public void SetCustomProcessor(IMovementProcessor processor)
    {
        customProcessor = processor;
    }

    private void UpdateFrameBuffer()
    {
        var rtt = serverMovement.Data.Rtt;

        frameBuffer = rtt > 0 ? MyMovementUtils.CalculateInputBufferSize(rtt) : 100;
    }

    private void Update()
    {
        if (customProcessor == null)
        {
            return;
        }

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
            byte[] rawInput = null;
            if (lastFrame < commandFrame.CurrentFrame)
            {
                rawInput = customProcessor.ConsumeInput();
            }

            while (lastFrame < commandFrame.CurrentFrame)
            {
                lastFrame += 1;

                var input = SendInput(rawInput);
                movementState.TryGetValue(lastFrame - 1, out var previousState);
                customProcessor.RestoreToState(previousState.RawState);

                lastMovementState = MyMovementUtils.ApplyCustomInput(input, previousState, customProcessor);

                movementState[lastFrame] = lastMovementState;
                inputState.Add(lastFrame, input);
            }

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

            customProcessor.RestoreToState(movementState[lastFrame - 1].RawState);
            MyMovementUtils.ApplyPartialCustomInput(inputState[lastFrame], movementState[lastFrame - 1],
                customProcessor, remainder);
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

            customProcessor.RestoreToState(movementState[lastFrame - 2].RawState);
            MyMovementUtils.ApplyPartialCustomInput(inputState[lastFrame - 1], movementState[lastFrame - 2],
                customProcessor, CommandFrameSystem.FrameLength + remainder);
        }
    }

    private void ProcessServerResponses()
    {
        while (serverResponses.Count > 0)
        {
            var response = serverResponses.Dequeue();
            if (response.Timestamp < confirmedFrame)
            {
                continue;
            }

            if (movementState.ContainsKey(response.Timestamp))
            {
                // Check if server agrees, which it always should.
                if (customProcessor.ShouldReplay(
                    movementState[response.Timestamp].RawState, response.MovementState.RawState))
                {
                    Debug.LogFormat("Mispredicted cf {0}", response.Timestamp);
                    Debug.LogFormat("Replaying input from {0} to {1}", response.Timestamp + 1, lastFrame);

                    var previousState = response.MovementState;
                    movementState[response.Timestamp] = previousState;
                    customProcessor.RestoreToState(previousState.RawState);

                    // Replay inputs until lastFrame, storing movementstates.
                    var i = response.Timestamp + 1;
                    while (i <= lastFrame)
                    {
                        movementState[i] =
                            MyMovementUtils.ApplyCustomInput(inputState[i], movementState[i - 1], customProcessor);

                        Debug.DrawLine(
                            Controller.transform.position,
                            Controller.transform.position + Vector3.up * 500);
                        i++;
                    }
                }

                confirmedFrame = response.Timestamp;
                inputState.Remove(response.Timestamp);


                // Remove the previous last confirmed state, keep this one around for potential velocity calculation
                movementState.Remove(response.Timestamp - 2);
            }
            else
            {
                Debug.LogWarning($"[Client {lastFrame}] Don't have movement state for cf {response.Timestamp}");
            }

            nextDilation = response.NextFrameAdjustment / 100000f;
            lastServerTimestamp = response.ServerTime;
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

    private ClientRequest SendInput(byte[] rawInput)
    {
        var clientRequest = new ClientRequest
        {
            InputRaw = rawInput,
            Timestamp = lastFrame,
            LastReceivedServerTime = lastServerTimestamp + (Time.time - lastServerTimestampReceived) * 100000f
        };

        var unackedInput = new List<ClientRequest>(inputState.Values) { clientRequest };

        movement.Send(new ClientMovement.Update
        {
            Buffer = unackedInput
        });

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
}
