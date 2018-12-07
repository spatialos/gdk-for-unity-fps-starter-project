using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MobileUIAnalogueControls))]
public class MobileUIController : MonoBehaviour
{
    public StandardButton JumpButton;
    public StandardButton ADSButton;
    public StandardButton FireButtonLeft;
    public StandardButton FireButtonRight;
    public StandardButton MenuButton;

    public RectTransform LeftStickKnob;
    public float LeftStickMaxDistance = 100;


    private void OnEnable()
    {
        JumpButton.OnButtonDown += Jump;
        ADSButton.OnButtonDown += ToggleADS;
        FireButtonLeft.OnButtonDown += StartFiringLeft;
        FireButtonRight.OnButtonDown += StartFiringRight;
        FireButtonLeft.OnButtonUp += StopFiring;
        FireButtonRight.OnButtonUp += StopFiring;
        MenuButton.OnButtonDown += OpenMenu;
    }

    private void OnDisable()
    {
        JumpButton.OnButtonDown -= Jump;
        ADSButton.OnButtonDown -= ToggleADS;
        FireButtonLeft.OnButtonDown -= StartFiringLeft;
        FireButtonRight.OnButtonDown -= StartFiringRight;
        FireButtonLeft.OnButtonUp -= StopFiring;
        FireButtonRight.OnButtonUp -= StopFiring;
        MenuButton.OnButtonDown -= OpenMenu;
        numActiveFireButtons = 0;
    }

    private void Jump(PointerEventData data)
    {
        JumpPressed = true;
    }

    private void ToggleADS(PointerEventData data)
    {
        AreAiming = !AreAiming;
    }

    private void StartFiringRight(PointerEventData data)
    {
        numActiveFireButtons++;
    }

    private void StartFiringLeft(PointerEventData data)
    {
        analogueControls.AddBlacklistedFingerId(data.pointerId);
        numActiveFireButtons++;
    }

    private void StopFiring(PointerEventData data)
    {
        numActiveFireButtons--;
    }

    private void OpenMenu(PointerEventData data)
    {
        MenuPressed = true;
    }

    private int numActiveFireButtons;

    public bool AreFiring => numActiveFireButtons > 0;


    public bool JumpPressed { get; private set; }
    public bool ShootPressed { get; private set; }
    public bool MenuPressed { get; private set; }
    public bool AreAiming { get; private set; }

    public Vector2 LookDelta => analogueControls.LookDelta;
    public Vector2 MoveTotal => analogueControls.MoveTotal;

    private MobileUIAnalogueControls analogueControls;

    private void Awake()
    {
        analogueControls = GetComponent<MobileUIAnalogueControls>();
    }

    private void Update()
    {
        LeftStickKnob.localPosition = Vector3.ClampMagnitude(MoveTotal, LeftStickMaxDistance);
    }

    private void LateUpdate()
    {
        JumpPressed = false;
        ShootPressed = false;
    }

    private void OnGUI()
    {
        GUI.color = Color.magenta;
        GUI.Label(new Rect(0, Screen.height / 2f, Screen.width, Screen.height / 2f), numActiveFireButtons.ToString());
    }
}
