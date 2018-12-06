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

    private int lastServerTimestamp = 0;
    private float lastServerTimestampReceived = 0;

    private float nextDilation = 1f;

    private int lastFrame = -1;
    private MovementState lastMovementState = new MovementState();

    private bool forwardThisFrame;
    private bool backThisFrame;
    private bool leftThisFrame;
    private bool rightThisFrame;
    private bool jumpThisFrame;
    private bool sprintThisFrame;
    private bool aimThisFrame;
    private float yawThisFrame;
    private float pitchThisFrame;

    private Color[] rewindColors = new Color[]
    {
        Color.red, Color.white, Color.yellow, Color.green
    };

    private int rewindColorIndex = 0;

    private Dictionary<int, MovementState> movementState = new Dictionary<int, MovementState>();
    private Dictionary<int, ClientRequest> inputState = new Dictionary<int, ClientRequest>();

    private MyMovementUtils.IMovementProcessor[] movementProcessors = { };

    private void OnEnable()
    {
        spatial = GetComponent<SpatialOSComponent>();
        commandFrame = spatial.World.GetExistingManager<CommandFrameSystem>();
        commandFrame.CurrentFrame = 0;
        serverMovement.OnServerMovement += OnServerMovement;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetMovementProcessors(MyMovementUtils.IMovementProcessor[] processors)
    {
        movementProcessors = processors;
    }

    private void Update()
    {
        if (commandFrame.NewFrame)
        {
            while (lastFrame < commandFrame.CurrentFrame)
            {
                lastFrame += 1;

                var input = SendInput();
                movementState.TryGetValue(lastFrame - 1, out var previousState);
                lastMovementState = MyMovementUtils.ApplyInput(Controller, input, previousState, movementProcessors);
                movementState[lastFrame] = lastMovementState;
                SaveInputState(input);
            }

            forwardThisFrame = false;
            backThisFrame = false;
            leftThisFrame = false;
            rightThisFrame = false;
            jumpThisFrame = false;
            sprintThisFrame = false;
            aimThisFrame = false;
            commandFrame.ServerAdjustment = nextDilation;
        }

        UpdateInRateStates();
    }

    public void AddInput(bool forward, bool back, bool left, bool right, bool jump, bool sprint, bool aim, float yaw, float pitch)
    {
        forwardThisFrame |= forward;
        backThisFrame |= back;
        leftThisFrame |= left;
        rightThisFrame |= right;
        jumpThisFrame |= jump;
        sprintThisFrame |= sprint;
        aimThisFrame |= aim;
        yawThisFrame = yaw;
        pitchThisFrame = pitch;
    }

    private void OnServerMovement(ServerResponse response)
    {
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

                // Replay inputs until lastFrame, storing movementstates.
                for (var i = response.Timestamp + 1; i <= lastFrame; i++)
                {
                    Debug.LogFormat("[Replaying Frame {0} ({1})]", i, rewindColors[rewindColorIndex]);
                    Debug.LogFormat("Input {0}", InputToString(inputState[i]));
                    Debug.LogFormat("Previous Position {0}", movementState[i].Position.ToVector3());
                    Debug.LogFormat("Previous Velocity {0}", movementState[i].Velocity.ToVector3());

                    movementState[i] = MyMovementUtils.ApplyInput(Controller, inputState[i], movementState[i - 1], movementProcessors);

                    Debug.LogFormat("Adjusted Position: {0}", movementState[i].Position.ToVector3());
                    Debug.DrawLine(
                        Controller.transform.position,
                        Controller.transform.position + Vector3.up * 500,
                        rewindColors[rewindColorIndex], 100f);
                }

                rewindColorIndex = (rewindColorIndex + 1) % rewindColors.Length;
            }
            else
            {
                // Debug.LogFormat("[Client] {0} confirmed", response.Timestamp);
            }

            inputState.Remove(response.Timestamp);

            // Remove the previous last confirmed state, keep this one around for potential velocity calculation
            movementState.Remove(response.Timestamp - 2);
        }
        else
        {
            // Debug.LogWarningFormat("Don't have movement state for cf {0}", response.Timestamp);
        }

        // update dilation
        // commandFrame.ServerAdjustment = response.TimeDelta;
        // commandFrame.ServerAdjustment = response.NextDilation / 100000f;
        nextDilation = response.NextDilation / 100000f;
        lastServerTimestamp = response.AppliedDilation;
        lastServerTimestampReceived = Time.time;
    }

    private float lastInputSentTime = -1;
    private List<float> inputSendRate = new List<float>(20);

    private void UpdateInputSendRate()
    {
        if (inputSendRate.Count >= 20)
        {
            inputSendRate.RemoveAt(0);
        }

        if (lastInputSentTime > 0)
        {
            // we have a last time to compare to.
            var delta = Time.time - lastInputSentTime;
            inputSendRate.Add(delta);
            lastInputSentTime = Time.time;
        }
        else
        {
            lastInputSentTime = Time.time;
        }
    }

    private float GetInputSendRate()
    {
        if (inputSendRate.Count > 0)
        {
            return inputSendRate.Average() / CommandFrameSystem.FrameLength;
        }

        return -1;
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

        movement.SendClientInput(clientRequest);

        UpdateInputSendRate();
        // Debug.LogFormat("[Client] Sent {0}", clientRequest.Timestamp);
        return clientRequest;
    }

    private void SaveInputState(ClientRequest request)
    {
        inputState.Add(lastFrame, request);
    }

    public MovementState GetState(int frame)
    {
        movementState.TryGetValue(frame, out var state);
        return state;
    }

    public MovementState GetLatestState()
    {
        return lastMovementState;
    }

    private void OnGUI()
    {
        if (!MyMovementUtils.ShowDebug)
        {
            return;
        }

        GUI.Label(new Rect(10, 10, 700, 20),
            string.Format("Frame: {0:00.00}, Fudge: {1:00.00}, Adjustment: {2:00.00}, rate: {3:00.00}, avg: {4:00.00}, var: {5:00.00}",
                CommandFrameSystem.FrameLength,
                commandFrame.ManualFudge,
                commandFrame.ServerAdjustment,
                GetInputSendRate(),
                averageRate,
                rateVar));
    }

    private Queue<float> outRate = new Queue<float>(50);
    private float averageRate = 1.0f;
    private float rateVar = 0.0f;

    private void UpdateInRateStates()
    {
        if (outRate.Count >= 50)
        {
            outRate.Dequeue();
        }

        outRate.Enqueue(GetInputSendRate());
        averageRate = outRate.Average();
        var absDiffs = 0f;

        foreach (var rate in outRate)
        {
            absDiffs += Mathf.Abs(rate - averageRate);
        }

        rateVar = absDiffs / outRate.Count;
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
