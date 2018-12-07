using UnityEngine;

public class MobileControls : MonoBehaviour, IControlProvider
{
    private MobileUIController controller;

    public float MovementScalar = 0.01f;
    public float LookScalar = 0.33f;
    public float SprintMaxAngle = 30f;
    public float SprintDistanceThreshold = 100;

    public Vector3 Movement
    {
        get
        {
            var totalDistance = controller.MoveTotal.magnitude;
            var speed = Mathf.Min(totalDistance, 1f / MovementScalar) * MovementScalar;
            return new Vector3(controller.MoveTotal.x, 0, controller.MoveTotal.y).normalized * speed;
        }
    }

    public float YawDelta => controller.LookDelta.x * LookScalar;
    public float PitchDelta => controller.LookDelta.y * LookScalar;
    public bool AreAiming => controller.AreAiming;

    public bool AreSprinting
    {
        get
        {
            var totalDistance = controller.MoveTotal.magnitude;
            var angle = Vector2.Angle(controller.MoveTotal, Vector2.up);
            return angle <= SprintMaxAngle && totalDistance > SprintDistanceThreshold;
        }
    }

    public bool JumpPressed => controller.JumpPressed;
    public bool ShootPressed => controller.ShootPressed;
    public bool ShootHeld => controller.AreFiring;

    public bool ConnectPressed { get; }
    public bool MenuPressed => controller.MenuPressed;
    public bool RespawnPressed { get; }

    private void Awake()
    {
        controller = FindObjectOfType<MobileUIController>();
        if (!controller)
        {
            Debug.LogError("Failed to find MobileUIController");
        }
    }
}
