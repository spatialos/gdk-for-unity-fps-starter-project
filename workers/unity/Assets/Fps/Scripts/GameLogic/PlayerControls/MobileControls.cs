using System.Collections;
using UnityEngine;
using Improbable.Gdk.Mobile;

public class MobileControls : MonoBehaviour, IControlProvider
{
    private IMobileInterface mobileInterface;

    public float MovementScalar = 0.01f;
    public float LookScalar = 0.33f;
    public float SprintMaxAngle = 30f;
    public float SprintDistanceThreshold = 100;

    // TODO Currently YawDelta/PitchDelta is untested on different devices. Probably needs some more love to resolve
    // DPI/physical screen size differences.
    public float YawDelta => mobileInterface.LookDelta.x * LookScalar;
    public float PitchDelta => mobileInterface.LookDelta.y * LookScalar;
    public bool AreAiming => mobileInterface.AreAiming;

    public bool JumpPressed => mobileInterface.JumpPressed;
    public bool ShootPressed => mobileInterface.ShootPressed;
    public bool ShootHeld => mobileInterface.AreFiring;

    public bool MenuPressed => mobileInterface.MenuPressed;
    public bool ConnectPressed { get; } // Not used

    // TODO Currently Movement is untested on different devices. Probably needs some more love to resolve
    // DPI/physical screen size differences.
    public Vector3 Movement
    {
        get
        {
            var totalDistance = mobileInterface.MoveTotal.magnitude;
            if (totalDistance <= 0f)
            {
                return Vector3.zero;
            }

            var speed = Mathf.Min(totalDistance, 1f / MovementScalar) * MovementScalar;
            return new Vector3(mobileInterface.MoveTotal.x, 0, mobileInterface.MoveTotal.y).normalized * speed;
        }
    }

    public bool AreSprinting
    {
        get
        {
            if (ShootHeld)
            {
                return false;
            }

            var totalDistance = mobileInterface.MoveTotal.magnitude;
            var angle = Vector2.Angle(mobileInterface.MoveTotal, Vector2.up);
            return angle <= SprintMaxAngle && totalDistance > SprintDistanceThreshold;
        }
    }

    // Respawn is triggered by a new touch on any part of screen
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
