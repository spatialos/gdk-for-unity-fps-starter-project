using UnityEngine;

public class MobileControls : MonoBehaviour, IControlProvider
{
    private MobileUIController ui;

    // Mobile controls polls state of the UI control class and filters data to be used by other classes
    // UI control class subscribes to buttons underneath it
    public float MovementScalar = 0.01f;
    public float LookScalar = 0.33f;

    public Vector3 Movement
    {
        get
        {
            var totalDistance = ui.moveTotal.magnitude;
            var speed = Mathf.Min(totalDistance, 1f / MovementScalar) * MovementScalar;
            return ui.AreMoving
                ? new Vector3(ui.moveTotal.x, 0, ui.moveTotal.y).normalized * speed
                : Vector3.zero;
        }
    }

    public bool MenuPressed { get; }
    public bool RespawnPressed { get; }
    public float YawDelta => ui.AreLooking ? ui.lookDelta.x * LookScalar : 0;
    public float PitchDelta => ui.AreLooking ? ui.lookDelta.y * LookScalar : 0;
    public bool AreAiming { get; }
    public bool AreSprinting { get; }
    public bool JumpPressed { get; }
    public bool ShootPressed { get; }
    public bool ShootHeld { get; }
    public bool ConnectPressed { get; }

    private void Awake()
    {
        ui = FindObjectOfType<MobileUIController>();
        if (!ui)
        {
            Debug.LogError("Failed to find MobileUIController");
        }
        else
        {
            Debug.Log($"ui set to {ui.gameObject.name}");
        }
    }

    private void LateUpdate()
    {
        ui.lookDelta = Vector2.zero;
    }
}
