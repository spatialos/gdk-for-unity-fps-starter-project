using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MobileAnalogueControls))]
public class MobileInterface : MonoBehaviour, IMobileInterface
{
    // TODO Could be possible to use Unity's inspector actions/events to hook these up instead?
    // I.e. StandardButtons are responsible for setting up links to MobileInterface

    public StandardButton JumpButton;
    public StandardButton ADSButton;
    public StandardButton FireButtonLeft;
    public StandardButton FireButtonRight;
    public StandardButton MenuButton;
    public RectTransform LeftStickKnob;
    public float LeftStickMaxDistance = 100;

    public bool ShowHitboxes;

    private void OnValidate()
    {
        var buttons = GetComponentsInChildren<StandardButton>();
        foreach (var button in buttons)
        {
            button.Hitbox.color = new Color(1, 0, 0, ShowHitboxes ? .3f : 0);
        }
    }

    private void OnEnable()
    {
        JumpButton.OnButtonDown += Jump;
        ADSButton.OnButtonDown += ToggleADS;
        ADSButton.OnButtonUp += ToggleADS;
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
        ADSButton.OnButtonUp -= ToggleADS;
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
        if (data != null)
        {
            analogueControls.AddBlacklistedFingerId(data.pointerId);
        }

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

    private MobileAnalogueControls analogueControls;

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
}
