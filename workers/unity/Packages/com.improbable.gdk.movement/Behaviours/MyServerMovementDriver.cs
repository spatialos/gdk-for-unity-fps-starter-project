using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    private int firstFrame = -1;

    private int nextExpectedInput = -1;
    private int nextServerFrame = -1;
    private const int FrameBuffer = 10;

    private float clientDilation = 1f;

    private MyMovementUtils.PidController pidController =
        new MyMovementUtils.PidController(0.1f, 0.01f, 0.0f, 1.0f, 100.0f);

    private ClientRequest lastInput;

    private StringBuilder logOut = new StringBuilder();

    private Queue<ClientRequest> inputReceived = new Queue<ClientRequest>();

    private Dictionary<int, ClientRequest> clientInputs = new Dictionary<int, ClientRequest>();
    private Dictionary<int, Vector3> movementState = new Dictionary<int, Vector3>();

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
    }

    private void OnEnable()
    {
        spatial = GetComponent<SpatialOSComponent>();
        commandFrame = spatial.World.GetExistingManager<CommandFrameSystem>();

        clientInput.OnClientInput += OnClientInputReceived;

        origin = spatial.Worker.Origin;
    }

    private float lastInputReceiveTime = -1;
    private List<float> inputReceiveRate = new List<float>(20);

    private void OnClientInputReceived(ClientRequest request)
    {
        // Debug.LogFormat("[Server] Received client frame {0} on frame {1}",
        //     request.Timestamp, lastFrame);
        inputReceived.Enqueue(request);
        ProcessInput();
    }

    private void UpdateInputReceivedRate()
    {
        if (inputReceiveRate.Count >= 20)
        {
            inputReceiveRate.RemoveAt(0);
        }

        if (lastInputReceiveTime > 0)
        {
            var delta = Time.time - lastInputReceiveTime;
            inputReceiveRate.Add(delta);
            lastInputReceiveTime = Time.time;
        }
        else
        {
            lastInputReceiveTime = Time.time;
        }
    }

    private float GetAverageInputReceivedRate()
    {
        if (inputReceiveRate.Count > 0)
        {
            return inputReceiveRate.Average() / CommandFrameSystem.FrameLength;
        }

        return -1;
    }

    private void ProcessInput()
    {
        if (lastFrame < 0)
        {
            return;
        }

        while (inputReceived.Count > 0)
        {
            var request = inputReceived.Dequeue();
            // If this is the first client input, use it for the next cf + Buffer.
            if (!hasInput)
            {
                nextExpectedInput = request.Timestamp;
                nextServerFrame = lastFrame + FrameBuffer;

                Debug.LogFormat("[Server] First Client frame {0}, applied on server frame {1}. Current frame {2}",
                    nextExpectedInput, nextServerFrame, lastFrame);

                hasInput = true;
            }

            if (request.IncludesJump)
            {
                Debug.LogFormat("Got Jump. Client Frame {0}, Received Frame {1}, Apply Frame {2}", request.Timestamp,
                    lastFrame, nextServerFrame);
            }

            if (request.Timestamp != nextExpectedInput)
            {
                Debug.LogWarningFormat(
                    "[Server] Expected input frame {0}, but received {1}", nextExpectedInput, request.Timestamp);
            }

            clientInputs.Add(nextServerFrame, request);

            // if (clientInputs.Count > 5)
            // {
            //     Debug.LogWarningFormat(
            //         "[Server] Client {0}, inputs {1}", spatial.SpatialEntityId, clientInputs.Count);
            // }

            nextServerFrame++;
            nextExpectedInput++;
        }
    }

    private List<float> inputConsumptionRate = new List<float>(20);

    private void Update()
    {
        TunePid();
        UpdateInputReceivedRate();

        if (commandFrame.NewFrame)
        {
            lastFrame = commandFrame.CurrentFrame;

            if (!hasInput || lastFrame < firstFrame)
            {
                return;
            }

            if (clientInputs.ContainsKey(lastFrame))
            {
                lastInput = clientInputs[lastFrame];
                clientInputs.Remove(lastFrame);
            }
            else
            {
                Debug.LogFormat("[Server] No client input for frame {0}. next input available: {1}",
                    lastFrame, GetNextFrame());
                if (clientInputs.Count == 0)
                {
                    nextServerFrame += 1;
                }
            }

            UpdateInputConsumptionRate();

            if (lastInput.IncludesJump)
            {
                Debug.LogFormat("Applying jump. Client Frame {0}, Local Frame {1}", lastInput.Timestamp, lastFrame);
            }

            logOut.AppendLine(
                string.Format("[{0}] Before: {1}", lastFrame, controller.transform.position));
            MyMovementUtils.ApplyInput(controller, lastInput, lastFrame, GetVelocity(lastFrame),
                movementProcessors);
            logOut.AppendLine(
                string.Format("[{0}] After: {1}", lastFrame, controller.transform.position));
            SaveMovementState();
            SendMovement();

            // Remove movement state from 10 frames ago
            movementState.Remove(lastFrame - 10);
            MyMovementUtils.CleanProcessors(movementProcessors, lastFrame - 10);

            UpdateDilation();
        }
    }

    private void TunePid()
    {
        var changed = false;
        var kp = pidController.Kp;
        var ki = pidController.Ki;
        var kd = pidController.Kd;

        // Kp up.
        if (Input.GetKeyDown(KeyCode.G))
        {
            kp += 0.01f;
            changed = true;
        }

        // Kp down.
        if (Input.GetKeyDown(KeyCode.B))
        {
            kp -= 0.01f;
            changed = true;
        }

        // Ki up.
        if (Input.GetKeyDown(KeyCode.H))
        {
            ki += 0.01f;
            changed = true;
        }

        // Ki down.
        if (Input.GetKeyDown(KeyCode.N))
        {
            ki -= 0.01f;
            changed = true;
        }

        // Kd up.
        if (Input.GetKeyDown(KeyCode.J))
        {
            kd += 0.01f;
            changed = true;
        }

        // Kd down.
        if (Input.GetKeyDown(KeyCode.M))
        {
            kd -= 0.01f;
            changed = true;
        }

        if (changed)
        {
            pidController = new MyMovementUtils.PidController(kp, ki, kd);
        }
    }

    private float lastConsumptionTime = -1;

    private void UpdateInputConsumptionRate()
    {
        if (inputConsumptionRate.Count >= 20)
        {
            inputConsumptionRate.RemoveAt(0);
        }

        if (lastConsumptionTime > 0)
        {
            // we have a last time to compare to.
            var delta = Time.time - lastConsumptionTime;
            inputConsumptionRate.Add(delta);
            lastConsumptionTime = Time.time;
        }
        else
        {
            lastConsumptionTime = Time.time;
        }
    }

    private float GetAverageInputConsumptionRate()
    {
        if (inputConsumptionRate.Count > 0)
        {
            return inputConsumptionRate.Average() / CommandFrameSystem.FrameLength;
        }

        return -1;
    }

    private int GetNextFrame()
    {
        if (clientInputs.Count > 0)
        {
            return clientInputs.Keys.Min();
        }

        return -1;
    }

    private void SaveMovementState()
    {
        // Debug.LogFormat("[Server] {0} = {1}", ToClienfCf(lastFrame), controller.transform.position - origin);
        movementState.Add(lastFrame, controller.transform.position);
    }

    private float lastPidUpdateTime = -1;

    private void UpdateDilation()
    {
        // Don't do anything until the buffer's full for the first time.
        if (lastFrame < firstFrame)
        {
            return;
        }

        var bufferSize = clientInputs.Count;
        var error = bufferSize - FrameBuffer;

        var dt = CommandFrameSystem.FrameLength;
        if (lastPidUpdateTime > 0)
        {
            dt = Time.time - lastPidUpdateTime;
        }

        lastPidUpdateTime = Time.time;

        var adjustment = pidController.Update(error, dt);

        clientDilation = adjustment;
    }

    private int positionRate = 30;
    private int positionTick = 0;

    private void SendMovement()
    {
        var position = controller.transform.position - origin;

        var response = new ServerResponse
        {
            Position = position.ToIntAbsolute(),
            Timestamp = lastInput.Timestamp,
            Yaw = lastInput.CameraYaw,
            Pitch = lastInput.CameraPitch,
            NextDilation = (int) (Mathf.Clamp(clientDilation, 0.5f, 1.5f) * 100000f)
        };
        server.SendServerMovement(response);
        var update = new ServerMovement.Update { Latest = response };
        server.Send(update);

        positionTick -= 1;
        if (positionTick <= 0)
        {
            positionTick = positionRate;
            spatialPosition.Send(new Position.Update()
            {
                Coords = new Option<Coordinates>(new Coordinates(position.x, position.y, position.z))
            });
        }
    }

    private Vector3 GetVelocity(int frame)
    {
        // return the difference of the previous 2 movement states.
        if (movementState.TryGetValue(frame - 2, out var before) &&
            movementState.TryGetValue(frame - 1, out var after))
        {
            return (after - before) / MyMovementUtils.FrameLength;
        }

        return Vector3.zero;
    }

    private void OnGUI()
    {
        var cons = GetAverageInputConsumptionRate();
        var ins = GetAverageInputReceivedRate();
        var delta = clientInputs.Count - FrameBuffer;

        GUI.Label(new Rect(10, 100, 700, 20),
            string.Format("Input Buffer: {0:00}, in: {1:00.00}, out: {2:00.00}, d: {3:00.00}, cd: {4:00.00}",
                clientInputs.Count, ins, cons, delta, clientDilation));

        GUI.Label(new Rect(10, 200, 700, 20),
            string.Format("Kp: {0:00.00} Ki: {1:00.00} Kd: {2:00.00}, lastError: {3:00.00}, integral: {4:00.00}, value: {5:00.00}",
                pidController.Kp, pidController.Ki, pidController.Kd,
                pidController.lastError, pidController.integral, pidController.value));
    }
}
