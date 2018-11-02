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
    private CharacterController controller;

    private float remainder;
    private int frame = 0;

    private readonly List<ServerResponse> movementBuffer = new List<ServerResponse>();

    private void OnEnable()
    {
        spatial = GetComponent<SpatialOSComponent>();
        commandFrame = spatial.World.GetExistingManager<CommandFrameSystem>();
        frame = commandFrame.CurrentFrame;
        controller = GetComponent<CharacterController>();

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
            movementBuffer.RemoveAt(0);
        }

        if (movementBuffer.Count > 0)
        {
            controller.transform.position = movementBuffer[0].Position.ToVector3() + spatial.Worker.Origin;
        }
    }

    private void OnGUI()
    {
        for (var i = 0; i < movementBuffer.Count; i++)
        {
            GUI.Label(new Rect(400, i * 31 + 10, 300, 30), string.Format("[{0}]: {1}",
                i, movementBuffer[i].Position.ToVector3().ToString()));
        }
    }
}
