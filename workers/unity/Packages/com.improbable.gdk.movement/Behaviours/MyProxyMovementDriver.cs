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

    private MyMovementUtils.PidController pidController =
        new MyMovementUtils.PidController(0.1f, 0.01f, 0f, 1f, 100f);

    private const int BufferSize = 3;
    private const int MaxBufferSize = 6;

    private float pitch;
    private Vector3 velocity;
    private bool aiming;

    private float remainder = 0f;
    private float frameLength = 1f;

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

        if (movementBuffer.Count < BufferSize)
        {
            return;
        }

        var from = movementBuffer[0];
        var to = movementBuffer[1];
        var fromPosition = from.MovementState.Position.ToVector3() + spatial.Worker.Origin;
        var toPosition = to.MovementState.Position.ToVector3() + spatial.Worker.Origin;
        var rot = Controller.transform.rotation.eulerAngles;
        var fromRot = Quaternion.Euler(from.Pitch / 100000f, from.Yaw / 100000f, rot.z);
        var toRot = Quaternion.Euler(to.Pitch / 100000f, to.Yaw / 100000f, rot.z);

        velocity = (toPosition - fromPosition) / CommandFrameSystem.FrameLength;
        aiming = from.Aiming;

        var t = remainder / CommandFrameSystem.FrameLength;

        var newRot = Quaternion.Slerp(fromRot, toRot, t);
        var newRotEuler = newRot.eulerAngles;

        Controller.transform.position = Vector3.Lerp(fromPosition, toPosition, t);

        Controller.transform.rotation = Quaternion.Euler(rot.x, newRotEuler.y, rot.z);
        pitch = newRotEuler.x;

        remainder += Time.deltaTime * frameLength;

        while (remainder > CommandFrameSystem.FrameLength)
        {
            remainder -= CommandFrameSystem.FrameLength;
            movementBuffer.RemoveAt(0);
        }

        UpdatePid();
    }

    private Queue<float> bufferSizeQueue = new Queue<float>(20);

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

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public float GetPitch()
    {
        return pitch;
    }

    public bool GetAiming()
    {
        return aiming;
    }

    private void OnGUI()
    {
        if (!MyMovementUtils.ShowDebug)
        {
            return;
        }

        var bufferAverage = (bufferSizeQueue.Count > 0) ? bufferSizeQueue.Average() : 0;

        GUI.Label(new Rect(10, 500, 700, 30), string.Format("Buffer Size: {0}, average: {1:00.00}, length: {2}",
            movementBuffer.Count, bufferAverage, frameLength));
    }
}
