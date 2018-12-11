using UnityEngine;

public class FlyController : MonoBehaviour
{
    public MobileControls controls;
    public float moveMultiplier;
    public float LookMultiplier;
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (mainCam == null)
        {
            return;
        }

        var move = controls.Movement * moveMultiplier;
        mainCam.transform.Translate(mainCam.transform.rotation * move * Time.deltaTime, Space.World);
        mainCam.transform.Rotate(0, controls.YawDelta * LookMultiplier, 0, Space.World);
        mainCam.transform.Rotate(controls.PitchDelta * -LookMultiplier, 0, 0, Space.Self);

        if (controls.UpHeld)
        {
            mainCam.transform.Translate(0, moveMultiplier * Time.deltaTime, 0);
        }
        else if (controls.DownHeld)
        {
            mainCam.transform.Translate(0, moveMultiplier * -Time.deltaTime, 0);
        }
    }
}
