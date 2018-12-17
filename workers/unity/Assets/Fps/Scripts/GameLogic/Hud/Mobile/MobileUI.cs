using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MobileAnalogueControls))]
public class MobileUI : MonoBehaviour, IMobileUI
{
    // TODO Could be possible to use Unity's inspector actions/events to hook these up instead?
    // I.e. StandardButtons are responsible for setting up links to MobileUI

    public TouchscreenButton JumpButton;
    public TouchscreenButton ADSButton;
    public TouchscreenButton FireButton;
    public TouchscreenButton MenuButton;
    public RectTransform LeftStickKnob;
    public float LeftStickMaxDistance = 100;

    public bool ShowHitboxes;

    public Vector2 LookDelta => analogueControls.LookDelta;
    public Vector2 MoveTotal => analogueControls.MoveTotal;
    public bool JumpPressed { get; private set; }
    public bool ShootPressed { get; private set; }
    public bool MenuPressed { get; private set; }
    public bool IsAiming { get; private set; }
    public bool ShootHeld { get; private set; }

    private MobileAnalogueControls analogueControls;

    private void OnValidate()
    {
        var buttons = GetComponentsInChildren<TouchscreenButton>();
        foreach (var button in buttons)
        {
            button.Hitbox.color = new Color(1, 0, 0, ShowHitboxes ? .3f : 0);
        }
    }

    private void Awake()
    {
        analogueControls = GetComponent<MobileAnalogueControls>();
    }

    private void Update()
    {
        LeftStickKnob.localPosition = Vector3.ClampMagnitude(MoveTotal, LeftStickMaxDistance);
    }

    private void LateUpdate()
    {
        JumpPressed = false;
        ShootPressed = false;
        MenuPressed = false;
    }

    private void OnEnable()
    {
        JumpButton.OnButtonDown += Jump;
        ADSButton.OnButtonDown += ToggleADS;
        ADSButton.OnButtonUp += ToggleADS;
        FireButton.OnButtonDown += StartFiring;
        FireButton.OnButtonUp += StopFiring;
        MenuButton.OnButtonDown += OpenMenu;
    }

    private void OnDisable()
    {
        JumpButton.OnButtonDown -= Jump;
        ADSButton.OnButtonDown -= ToggleADS;
        ADSButton.OnButtonUp -= ToggleADS;
        FireButton.OnButtonDown -= StartFiring;
        FireButton.OnButtonUp -= StopFiring;
        MenuButton.OnButtonDown -= OpenMenu;
    }


    private void Jump(PointerEventData data)
    {
        JumpPressed = true;
    }

    private void ToggleADS(PointerEventData data)
    {
        IsAiming = !IsAiming;
    }

    private void StartFiring(PointerEventData data)
    {
        ShootHeld = true;
    }

    private void StopFiring(PointerEventData data)
    {
        ShootHeld = false;
    }

    private void OpenMenu(PointerEventData data)
    {
        MenuPressed = true;
    }
}
