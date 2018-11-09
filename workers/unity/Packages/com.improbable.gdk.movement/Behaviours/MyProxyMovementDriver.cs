using System.Collections.Generic;
using System.Linq;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

public class MyProxyMovementDriver : MonoBehaviour
{
    [Require] private ServerMovement.Requirable.Reader server;

    private SpatialOSComponent spatial;
    private CommandFrameSystem commandFrame;
    public CharacterController Controller;

    private readonly List<ServerResponse> movementBuffer = new List<ServerResponse>();
    private readonly MyMovementUtils.PidController pidController =
        new MyMovementUtils.PidController(0.1f, 0.01f, 0f, 1f, 100f);

    private const int BufferSize = 3;

    private float pitch;
    private Vector3 velocity;

    private float remainder = 0f;
    private float frameLengthModifier = 1.0f;
    private float frameLength;

    private void OnEnable()
    {
        spatial = GetComponent<SpatialOSComponent>();
        commandFrame = spatial.World.GetExistingManager<CommandFrameSystem>();
        server.OnServerMovement += OnServerMovement;
        frameLength = commandFrame.FrameLength;
    }

    private void OnServerMovement(ServerResponse response)
    {
        movementBuffer.Add(response);
    }

    private void Update()
    {
        if (Controller == null)
        {
            return;
        }

        if (movementBuffer.Count < BufferSize)
        {
            return;
        }

        var from = movementBuffer[0];
        var to = movementBuffer[1];
        var fromPosition = from.Position.ToVector3() + spatial.Worker.Origin;
        var toPosition = to.Position.ToVector3() + spatial.Worker.Origin;

        var t = remainder / frameLength;

        var oldPosition = Controller.transform.position;
        var newPosition = Vector3.Lerp(fromPosition, toPosition, t);
        Controller.transform.position = newPosition;
        velocity = (newPosition - oldPosition) / Time.deltaTime;
        var rotation = Controller.transform.rotation.eulerAngles;
        rotation.y = Mathf.Lerp(from.Yaw, to.Yaw, t) / 100000f;
        Controller.transform.rotation = Quaternion.Euler(rotation);
        pitch = Mathf.Lerp(from.Pitch, to.Pitch, t) / 100000f;

        remainder += Time.deltaTime;
        if (remainder > frameLength)
        {
            remainder -= frameLength;
            movementBuffer.RemoveAt(0);
        }

        UpdatePid();
    }

    private void UpdatePid()
    {
        var current = movementBuffer.Count;
        var error = BufferSize - current;

        frameLengthModifier = pidController.Update(error, Time.deltaTime);
        frameLength = commandFrame.FrameLength * Mathf.Clamp(frameLengthModifier, 0.5f, 1.5f);
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public float GetPitch()
    {
        return pitch;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 300, 700, 30), string.Format("Buffer Size: {0}, modifier: {1:00.00}, length: {2}",
            movementBuffer.Count, frameLengthModifier, frameLength * 1000f));
    }
}
