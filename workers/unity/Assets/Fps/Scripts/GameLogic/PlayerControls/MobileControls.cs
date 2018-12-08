using System.Collections;
using UnityEngine;

public class MobileControls : MonoBehaviour, IControlProvider
{
    private IMobileInterface uiInterface;

    public float MovementScalar = 0.01f;
    public float LookScalar = 0.33f;
    public float SprintMaxAngle = 30f;
    public float SprintDistanceThreshold = 100;

    public Vector3 Movement
    {
        get
        {
            var totalDistance = uiInterface.MoveTotal.magnitude;
            var speed = Mathf.Min(totalDistance, 1f / MovementScalar) * MovementScalar;
            return new Vector3(uiInterface.MoveTotal.x, 0, uiInterface.MoveTotal.y).normalized * speed;
        }
    }

    public float YawDelta => uiInterface.LookDelta.x * LookScalar;
    public float PitchDelta => uiInterface.LookDelta.y * LookScalar;
    public bool AreAiming => uiInterface.AreAiming;

    public bool AreSprinting
    {
        get
        {
            var totalDistance = uiInterface.MoveTotal.magnitude;
            var angle = Vector2.Angle(uiInterface.MoveTotal, Vector2.up);
            return angle <= SprintMaxAngle && totalDistance > SprintDistanceThreshold;
        }
    }

    public bool JumpPressed => uiInterface.JumpPressed;
    public bool ShootPressed => uiInterface.ShootPressed;
    public bool ShootHeld => uiInterface.AreFiring;

    public bool MenuPressed => uiInterface.MenuPressed;
    public bool ConnectPressed { get; } // TODO is this used anywhere?
    public bool RespawnPressed { get; } // TODO Use any touch?

    private void Awake()
    {
        StartCoroutine(LocateInterface());
        uiInterface = new MobileInterfaceStandIn();
    }

    private IEnumerator LocateInterface()
    {
        while (true)
        {
            var realUiInterface = FindObjectOfType<MobileInterface>();
            if (realUiInterface)
            {
                uiInterface = realUiInterface;
        Debug.Log("Hurrah, found the interface!");
                break;
            }

            yield return null;
        }

    }
}
