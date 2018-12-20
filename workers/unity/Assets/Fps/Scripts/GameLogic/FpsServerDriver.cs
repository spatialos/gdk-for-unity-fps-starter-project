using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Movement;
using UnityEngine;

public class FpsServerDriver : MonoBehaviour
{
    [Require] private ServerMovement.Requirable.Writer writer;

    private CharacterController controller;
    private MyServerMovementDriver driver;
    private FpsMovement fpsMovement;

    private void OnEnable()
    {
        controller = GetComponent<CharacterController>();
        driver = GetComponent<MyServerMovementDriver>();
        fpsMovement = new FpsMovement(controller, GetComponent<SpatialOSComponent>().Worker.Origin, "Server");
        driver.SetCustomProcessor(fpsMovement);
    }

    public void Teleport(Vector3 spawnPosition)
    {
        fpsMovement.TeleportProcessor.Teleport(spawnPosition);
    }
}
