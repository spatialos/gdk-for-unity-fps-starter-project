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

    private float remainder;
    private int frame = 0;

    private readonly List<ServerResponse> movementBuffer = new List<ServerResponse>();

    private void OnEnable()
    {
        spatial = GetComponent<SpatialOSComponent>();
        commandFrame = spatial.World.GetExistingManager<CommandFrameSystem>();
        frame = commandFrame.CurrentFrame;
        Controller = GetComponent<CharacterController>();

        server.OnServerMovement += OnServerMovement;
    }

    private void OnServerMovement(ServerResponse response)
    {
        movementBuffer.Add(response);
    }

    private void Update()
    {
        if (movementBuffer.Count < 2)
        {
            return;
        }

        if (frame < commandFrame.CurrentFrame)
        {
            frame = commandFrame.CurrentFrame;
            while (movementBuffer.Count > 2)
            {
                movementBuffer.RemoveAt(0);
            }
        }

        if (movementBuffer.Count > 0)
        {
            var state = movementBuffer[0];
            Controller.transform.position = state.Position.ToVector3() + spatial.Worker.Origin;
            var rotation = Controller.transform.rotation.eulerAngles;
            rotation.y = state.Yaw / 100000f;
            Controller.transform.rotation = Quaternion.Euler(rotation);
        }
    }

    public Vector3 GetVelocity()
    {
        if (movementBuffer.Count > 2)
        {
            return (movementBuffer[0].Position.ToVector3() - movementBuffer[1].Position.ToVector3()) /
                MyMovementUtils.FrameLength;
        }

        return Vector3.zero;
    }

    public float GetPitch()
    {
        if (movementBuffer.Count > 0)
        {
            return movementBuffer[0].Pitch / 100000f;
        }

        return Quaternion.identity.eulerAngles.x;
    }

    private void OnGUI()
    {
        // for (var i = 0; i < movementBuffer.Count; i++)
        // {
        //     var state = movementBuffer[i];
        //     GUI.Label(new Rect(400, i * 31 + 10, 300, 30), string.Format("[{0}]: {1} | {2}",
        //         state.Timestamp, state.Position.ToVector3().ToString(), state.Yaw / 100000f));
        // }
    }
}
