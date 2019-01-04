using System.Collections.Generic;
using System.Linq;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Movement;
using UnityEngine;

public class MyProxyMovementDriver : MonoBehaviour
{
    [Require] private ServerMovement.Requirable.Reader server;

    private readonly List<ServerResponse> movementBuffer = new List<ServerResponse>();

    private readonly MyMovementUtils.PidController pidController =
        new MyMovementUtils.PidController(0.1f, 0.01f, 0f, 1f, 100f);

    private const int BufferSize = 3;

    private float pitch;
    private Vector3 velocity;
    private bool aiming;

    private float remainder = 0f;
    private float frameLength = 1f;

    private void OnEnable()
    {
        server.LatestUpdated += ServerOnLatestUpdated;
    }

    private void ServerOnLatestUpdated(ServerResponse response)
    {
        movementBuffer.Add(response);
    }

    public bool GetInterpState(out float t, out byte[] from, out byte[] to)
    {
        t = remainder / CommandFrameSystem.FrameLength;
        if (movementBuffer.Count >= 2)
        {
            from = movementBuffer[0].MovementState.RawState;
            to = movementBuffer[1].MovementState.RawState;
        }
        else if (movementBuffer.Count == 1)
        {
            from = movementBuffer[0].MovementState.RawState;
            to = movementBuffer[0].MovementState.RawState;
        }
        else
        {
            from = null;
            to = null;
            return false;
        }

        return true;
    }

    private void Update()
    {
        if (movementBuffer.Count < BufferSize)
        {
            return;
        }

        remainder += Time.deltaTime * frameLength;

        while (remainder > CommandFrameSystem.FrameLength)
        {
            remainder -= CommandFrameSystem.FrameLength;
            movementBuffer.RemoveAt(0);
        }

        UpdatePid();
    }

    private readonly Queue<float> bufferSizeQueue = new Queue<float>(20);

    private void UpdatePid()
    {
        if (bufferSizeQueue.Count >= 20)
        {
            bufferSizeQueue.Dequeue();
        }

        bufferSizeQueue.Enqueue(movementBuffer.Count);
        var error = bufferSizeQueue.Average() - BufferSize;

        if (Mathf.Abs(error) < 1.0f)
        {
            frameLength = 1.0f;
            return;
        }

        frameLength = Mathf.Clamp(pidController.Update(error, Time.deltaTime), 1 / 1000f, 2f);
    }

    private void OnGUI()
    {
        if (!MyMovementUtils.ShowDebug)
        {
            return;
        }

        var bufferAverage = (bufferSizeQueue.Count > 0) ? bufferSizeQueue.Average() : 0;

        GUI.Label(new Rect(10, 500, 700, 30),
            $"Buffer Size: {movementBuffer.Count}, average: {bufferAverage:00.00}, length: {frameLength}");
    }
}
