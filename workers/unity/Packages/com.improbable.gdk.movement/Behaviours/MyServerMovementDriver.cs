using System.Collections.Generic;
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
    private int clientFrameOffset = -1;
    private int firstFrame = -1;
    private const int FrameBuffer = 2;

    private StringBuilder logOut = new StringBuilder();

    private Dictionary<int, ClientRequest> clientInputs = new Dictionary<int, ClientRequest>();
    private Dictionary<int, Vector3> movementState = new Dictionary<int, Vector3>();

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnDestroy()
    {
        // var sw = new StreamWriter("C:/GdkLogs/MyServerMovement.txt");
        // sw.Write(logOut.ToString());
        // sw.Close();
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
        // If this is the first client input, use it for the next cf + Buffer.
        if (clientFrameOffset < 0)
        {
            clientFrameOffset = lastFrame - request.Timestamp + FrameBuffer;
            firstFrame = lastFrame + FrameBuffer;

            // Debug.LogFormat("[Server] Setting offset to {0}, currentFrame {1}", clientFrameOffset, lastFrame);
        }

        // Debug.LogFormat("[Server] Received {0}. Local frame {1}", request.Timestamp, lastFrame);
        // Debug.LogFormat("[Server] Adding forward={0} to cf {1}", request.ForwardPressed, request.Timestamp);
        clientInputs.Add(request.Timestamp + clientFrameOffset, request);
    }

    private void Update()
    {
        if (lastFrame < commandFrame.CurrentFrame)
        {
            lastFrame = commandFrame.CurrentFrame;

            if (clientFrameOffset < 0)
            {
                return;
            }

            if (lastFrame < firstFrame)
            {
                // Debug.LogFormat("Current Frame {0} < First frame {1}", lastFrame, firstFrame);
                return;
            }

            if (clientInputs.ContainsKey(lastFrame))
            {
                var input = clientInputs[lastFrame];
                // Debug.LogFormat("[Server] Apply {0} ({1})", lastFrame, input.Timestamp);
                clientInputs.Remove(lastFrame);
                logOut.AppendLine(
                    string.Format("[{0}] Before: {1}", ToClientCf(lastFrame), controller.transform.position));
                MyMovementUtils.ApplyMovement(controller, input, GetVelocity(lastFrame));
                logOut.AppendLine(
                    string.Format("[{0}] After: {1}", ToClientCf(lastFrame), controller.transform.position));
                SaveMovementState();
                SendMovement();

                // Remove movement state from 10 frames ago
                movementState.Remove(lastFrame - 10);
            }
            else
            {
                // Debug.LogFormat("[Server] No client input found for cf={0}", ToClienfCf(lastFrame));
            }
        }
    }

    private void SaveMovementState()
    {
        // Debug.LogFormat("[Server] {0} = {1}", ToClienfCf(lastFrame), controller.transform.position - origin);
        movementState.Add(lastFrame, controller.transform.position);
    }

    private void SendMovement()
    {
        var position = controller.transform.position - origin;

        var response = new ServerResponse
        {
            Position = position.ToIntAbsolute(),
            Timestamp = ToClientCf(lastFrame),
        };
        server.SendServerMovement(response);
        // Debug.LogFormat("[Server] Sent {0}", response.Timestamp);
        spatialPosition.Send(new Position.Update()
        {
            Coords = new Option<Coordinates>(new Coordinates(position.x, position.y, position.z))
        });
    }

    private int ToClientCf(int serverCf)
    {
        return serverCf - clientFrameOffset;
    }

    private Vector3 GetVelocity(int frame)
    {
        // return the difference of the previous 2 movement states.
        if (movementState.TryGetValue(frame - 2, out var before) &&
            movementState.TryGetValue(frame - 1, out var after))
        {
            return after - before;
        }

        return Vector3.zero;
    }

    private void OnGUI()
    {
        // Print Current movement states on right side of screen.
        // var frames = new List<int>(movementState.Keys);
        // frames.Sort();
        //
        // var i = 10;
        // const int lineHeight = 30;
        //
        // foreach (var frame in frames)
        // {
        //     GUI.Label(new Rect(10, i, 300, lineHeight), string.Format("[{0}] {1}", frame, movementState[frame]));
        //     i += lineHeight + 2;
        // }
        GUI.Label(new Rect(10, 10, 300, 30), string.Format("S.Grounded: {0}", MyMovementUtils.IsGrounded(controller)));
    }
}
