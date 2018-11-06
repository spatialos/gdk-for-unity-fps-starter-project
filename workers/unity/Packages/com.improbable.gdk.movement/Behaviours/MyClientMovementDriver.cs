using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;

public class MyClientMovementDriver : MonoBehaviour
{
    [Require] private ClientMovement.Requirable.Writer movement;
    [Require] private ServerMovement.Requirable.Reader serverMovement;

    public CharacterController Controller;
    public bool UseKeyboardInput = true;

    private SpatialOSComponent spatial;
    private CommandFrameSystem commandFrame;

    private float pitchSpeed = 1;
    private float yawSpeed = 1;

    public Transform ViewTransform;

    private int lastFrame = -1;

    private bool forwardThisFrame;
    private bool backThisFrame;
    private bool leftThisFrame;
    private bool rightThisFrame;
    private bool jumpThisFrame;
    private bool sprintThisFrame;

    private StringBuilder outBuilder = new StringBuilder();

    private Color[] rewindColors = new Color[]
    {
        Color.red, Color.white, Color.yellow, Color.green
    };

    private int rewindColorIndex = 0;

    private Dictionary<int, Vector3> movementState = new Dictionary<int, Vector3>();
    private Dictionary<int, ClientRequest> inputState = new Dictionary<int, ClientRequest>();

    private readonly MyMovementUtils.IMovementProcessor[] movementProcessors =
    {
        new StandardMovement(),
        new JumpMovement(),
        new MyMovementUtils.Gravity(),
        new MyMovementUtils.TerminalVelocity(),
    };

    private void OnEnable()
    {
        spatial = GetComponent<SpatialOSComponent>();
        commandFrame = spatial.World.GetExistingManager<CommandFrameSystem>();
        commandFrame.CurrentFrame = 0;
        serverMovement.OnServerMovement += OnServerMovement;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDestroy()
    {
        // var fileOut = new StreamWriter("C:/GdkLogs/MyClientMovement.txt");
        // fileOut.Write(outBuilder.ToString());
        // fileOut.Close();
    }

    private void Update()
    {
        if (UseKeyboardInput)
        {
            forwardThisFrame |= Input.GetKey(KeyCode.W);
            backThisFrame |= Input.GetKey(KeyCode.S);
            leftThisFrame |= Input.GetKey(KeyCode.A);
            rightThisFrame |= Input.GetKey(KeyCode.D);
            jumpThisFrame |= Input.GetKey(KeyCode.Space);
            sprintThisFrame |= Input.GetKey(KeyCode.LeftShift);
        }

        if (commandFrame.CurrentFrame != lastFrame)
        {
            lastFrame = commandFrame.CurrentFrame;
            // Debug.LogFormat("Send forward={0} for cf {1}", forwardThisFrame, lastFrame);

            var rot = ViewTransform.rotation.eulerAngles;
            rot.x -= Input.GetAxis("Mouse Y") * pitchSpeed;
            ViewTransform.rotation = Quaternion.Euler(rot);

            var controllerRot = Controller.transform.rotation.eulerAngles;
            controllerRot.y += Input.GetAxis("Mouse X") * yawSpeed;
            Controller.transform.rotation = Quaternion.Euler(controllerRot);

            var input = SendInput();
            outBuilder.AppendLine(string.Format("[{0}] Before: {1}", lastFrame, Controller.transform.position));
            MyMovementUtils.ApplyInput(Controller, input, lastFrame, GetVelocity(lastFrame), movementProcessors);
            outBuilder.AppendLine(string.Format("[{0}] After: {1}", lastFrame, Controller.transform.position));
            SaveMovementState(lastFrame);
            SaveInputState(input);

            forwardThisFrame = false;
            backThisFrame = false;
            leftThisFrame = false;
            rightThisFrame = false;
            jumpThisFrame = false;
            sprintThisFrame = false;
        }

        // transform.position = Controller.transform.position;
    }

    private void SetInput(bool forward, bool back, bool left, bool right, bool jump, bool sprint)
    {
        forwardThisFrame = forward;
        backThisFrame = back;
        leftThisFrame = left;
        rightThisFrame = right;
        jumpThisFrame = jump;
        sprintThisFrame = sprint;
    }

    private void OnServerMovement(ServerResponse response)
    {
        // Debug.LogFormat("[Client] Received {0} (local frame {1})", response.Timestamp, lastFrame);

        if (movementState.ContainsKey(response.Timestamp))
        {
            // Check if server agrees, which it always should.
            var predictionPosition = movementState[response.Timestamp];
            var actualPosition = response.Position.ToVector3();
            var distance = Vector3.Distance(predictionPosition, actualPosition);
            if (distance > 0.1f)
            {
                Debug.LogFormat("Mispredicted cf {0}", response.Timestamp);
                Debug.LogFormat("Predicted: {0}", predictionPosition);
                Debug.LogFormat("Actual: {0}", actualPosition);
                Debug.LogFormat("Diff: {0}", distance);
                Debug.LogFormat("Replaying input from {0} to {1}", response.Timestamp + 1, lastFrame);


                outBuilder.AppendLine(string.Format("[{0}] Before Reset frame {1}: {2}",
                    lastFrame, response.Timestamp, Controller.transform.position
                ));

                Controller.transform.position = actualPosition;
                SaveMovementState(response.Timestamp);

                outBuilder.AppendLine(string.Format("[{0}] After Reset frame {1}: {2}",
                    lastFrame, response.Timestamp, Controller.transform.position
                ));
                // Replay inputs until lastFrame, storing movementstates.
                for (var i = response.Timestamp + 1; i <= lastFrame; i++)
                {
                    Debug.LogFormat("[Replaying Frame {0} ({1})]", i, rewindColors[rewindColorIndex]);
                    Debug.LogFormat("Input {0}", InputToString(inputState[i]));
                    Debug.LogFormat("Previous Position {0}", movementState[i]);
                    Debug.LogFormat("Previous Velocity {0}", GetVelocity(i));

                    outBuilder.AppendLine(string.Format("[{0}] Before replay {1}: {2}",
                        lastFrame, i, Controller.transform.position
                    ));
                    MyMovementUtils.ApplyInput(Controller, inputState[i], i, GetVelocity(i), movementProcessors);

                    outBuilder.AppendLine(string.Format("[{0}] After replay {1}: {2}",
                        lastFrame, i, Controller.transform.position
                    ));
                    SaveMovementState(i);


                    Debug.LogFormat("Adjusted Position: {0}", movementState[i]);
                    Debug.DrawLine(
                        Controller.transform.position,
                        Controller.transform.position + Vector3.up * 500,
                        rewindColors[rewindColorIndex], 100f);
                }

                outBuilder.AppendLine(string.Format("[{0}] After reapply: {1}",
                    lastFrame, Controller.transform.position));

                rewindColorIndex = (rewindColorIndex + 1) % rewindColors.Length;
            }
            else
            {
                // Debug.LogFormat("[Client] {0} confirmed", response.Timestamp);
            }

            inputState.Remove(response.Timestamp);

            // Remove the previous last confirmed state, keep this one around for potential velocity calculation.
            movementState.Remove(response.Timestamp - 2);
            MyMovementUtils.CleanProcessors(movementProcessors, response.Timestamp - 2);
        }
        else
        {
            // Debug.LogWarningFormat("Don't have movement state for cf {0}", response.Timestamp);
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
            IncludesJump = jumpThisFrame,
            IncludesSprint = sprintThisFrame,
            CameraYaw = (int) (Controller.transform.rotation.eulerAngles.y * 100000f),
            Timestamp = lastFrame
        };

        movement.SendClientInput(clientRequest);
        // Debug.LogFormat("[Client] Sent {0}", clientRequest.Timestamp);
        return clientRequest;
    }

    private void SaveMovementState(int frame)
    {
        // Debug.LogFormat("[Client] {0} = {1}", frame, controller.transform.position);
        movementState.Remove(frame);
        movementState.Add(frame, Controller.transform.position);
    }

    private void SaveInputState(ClientRequest request)
    {
        inputState.Add(lastFrame, request);
    }

    private Vector3 GetVelocity(int frame)
    {
        // return the difference of the previous 2 movement states.
        if (movementState.TryGetValue(frame - 2, out var before) &&
            movementState.TryGetValue(frame - 1, out var after))
        {
            return (after - before) / MyMovementUtils.FrameLength;
        }

        Debug.LogWarningFormat("Looking for velocity for frame {0}", frame);
        return Vector3.zero;
    }

    private void OnGUI()
    {
        // // Print Current movement states on right side of screen.
        // var frames = new List<int>(movementState.Keys);
        // frames.Sort();
        //
        // var i = 10;
        // const int lineHeight = 30;
        //
        // foreach (var frame in frames)
        // {
        //     GUI.Label(new Rect(700, i, 300, lineHeight), string.Format("[{0}] {1}", frame, movementState[frame]));
        //     i += lineHeight + 2;
        // }
        GUI.Label(new Rect(700, 10, 200, 30), string.Format("C.Grounded: {0}", MyMovementUtils.IsGrounded(Controller)));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(Controller.transform.position, 0.1f);

        float c = 0;
        for (var i = lastFrame; movementState.ContainsKey(i); i--)
        {
            Gizmos.color = Color.Lerp(Color.red, Color.white, c / movementState.Count);
            Gizmos.DrawWireSphere(movementState[i], 0.5f);
            c += 1;
        }
    }

    private string InputToString(ClientRequest request)
    {
        return string.Format("[F:{0} B:{1} R:{2} L:{3}, J:{4}, S:{5}, Yaw:{6}]",
            request.ForwardPressed, request.BackPressed,
            request.RightPressed, request.LeftPressed,
            request.IncludesJump, request.IncludesSprint,
            request.CameraYaw);
    }
}
