using System.Collections;
using UnityEngine;

public class MobileControls : MonoBehaviour, IControlProvider
{
    private IMobileInterface mobileInterface;

    public float MovementScalar = 0.01f;
    public float LookScalar = 0.33f;
    public float SprintMaxAngle = 30f;
    public float SprintDistanceThreshold = 100;


    public float YawDelta => mobileInterface.LookDelta.x * LookScalar;
    public float PitchDelta => mobileInterface.LookDelta.y * LookScalar;
    public bool AreAiming => mobileInterface.AreAiming;

    public bool JumpPressed => mobileInterface.JumpPressed;
    public bool ShootPressed => mobileInterface.ShootPressed;
    public bool ShootHeld => mobileInterface.AreFiring;

    public bool MenuPressed => mobileInterface.MenuPressed; // TODO is this hooked up anywhere?
    public bool ConnectPressed { get; } // TODO is this used anywhere?

    public Vector3 Movement
    {
        get
        {
            var totalDistance = mobileInterface.MoveTotal.magnitude;
            var speed = Mathf.Min(totalDistance, 1f / MovementScalar) * MovementScalar;
            return new Vector3(mobileInterface.MoveTotal.x, 0, mobileInterface.MoveTotal.y).normalized * speed;
        }
    }

    public bool AreSprinting
    {
        get
        {
            var totalDistance = mobileInterface.MoveTotal.magnitude;
            var angle = Vector2.Angle(mobileInterface.MoveTotal, Vector2.up);
            return angle <= SprintMaxAngle && totalDistance > SprintDistanceThreshold;
        }
    }

    public bool RespawnPressed
    {
        get
        {
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    return true;
                }
            }

            return false;
        }
    }


    private void Awake()
    {
        // The InGameHUD enables a frame after the player spawns.
        // To prevent issues, a dummy mobileInterface is used until
        // the real one is found in the scene
        mobileInterface = new MobileInterfaceStandIn();
        StartCoroutine(LocateInterface());
    }

    private IEnumerator LocateInterface()
    {
        while (true)
        {
            var realMobileInterface = FindObjectOfType<MobileInterface>();
            if (realMobileInterface)
            {
                mobileInterface = realMobileInterface;
                break;
            }
            
            yield return null;
        }
    }
}
