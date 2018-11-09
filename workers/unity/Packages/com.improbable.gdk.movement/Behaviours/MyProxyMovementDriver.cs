using System.Collections.Generic;
using System.Linq;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using Unity.Collections;
using UnityEditor.Graphs;
using UnityEngine;

public class MyProxyMovementDriver : MonoBehaviour
{
    [Require] private ServerMovement.Requirable.Reader server;

    private SpatialOSComponent spatial;
    private CommandFrameSystem commandFrame;
    public CharacterController Controller;

    private readonly List<ServerResponse> movementBuffer = new List<ServerResponse>();

    private float pitch;
    private Vector3 velocity;

    private void OnEnable()
    {
        spatial = GetComponent<SpatialOSComponent>();
        commandFrame = spatial.World.GetExistingManager<CommandFrameSystem>();
        server.OnServerMovement += OnServerMovement;
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

        if (movementBuffer.Count < 3)
        {
            return;
        }

        var from = movementBuffer[0];
        var to = movementBuffer[1];
        var fromPosition = from.Position.ToVector3() + spatial.Worker.Origin;
        var toPosition = to.Position.ToVector3() + spatial.Worker.Origin;

        var t = commandFrame.GetRemainder() / commandFrame.FrameLength;

        var oldPosition = Controller.transform.position;
        var newPosition = Vector3.Lerp(fromPosition, toPosition, t);
        Controller.transform.position = newPosition;
        velocity = (newPosition - oldPosition) / Time.deltaTime;
        var rotation = Controller.transform.rotation.eulerAngles;
        rotation.y = Mathf.Lerp(from.Yaw, to.Yaw, t) / 100000f;
        Controller.transform.rotation = Quaternion.Euler(rotation);
        pitch = Mathf.Lerp(from.Pitch, to.Pitch, t) / 100000f;

        if (commandFrame.NewFrame)
        {
            movementBuffer.RemoveAt(0);
        }
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
        for (var i = 0; i < movementBuffer.Count; i++)
        {
            var state = movementBuffer[i];
            GUI.Label(new Rect(10, i * 31 + 300, 700, 30), string.Format("[{0}]: {1} | {2}",
                state.Timestamp, state.Position.ToVector3().ToString(), state.Yaw / 100000f));
        }
    }
}
