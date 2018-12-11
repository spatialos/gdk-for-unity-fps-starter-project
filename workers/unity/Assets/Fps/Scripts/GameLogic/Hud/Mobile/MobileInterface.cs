using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MobileAnalogueControls))]
public class MobileInterface : MonoBehaviour, IMobileInterface
{
    // TODO Could be possible to use Unity's inspector actions/events to hook these up instead?
    // I.e. StandardButtons are responsible for setting up links to MobileInterface

    public StandardButton UpButton;
    public StandardButton DownButton;
    public RectTransform LeftStickKnob;
    public float LeftStickMaxDistance = 100;

    public bool ShowHitboxes;

    public Vector2 LookDelta => analogueControls.LookDelta;
    public Vector2 MoveTotal => analogueControls.MoveTotal;
    public bool JumpPressed { get; private set; }
    public bool ShootPressed { get; private set; }
    public bool MenuPressed { get; private set; }
    public bool AreFiring => numActiveFireButtons > 0;
    public bool AreAiming { get; private set; }

    private int numActiveFireButtons;

    private MobileAnalogueControls analogueControls;

    private void OnValidate()
    {
        var buttons = GetComponentsInChildren<StandardButton>();
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
        UpButton.OnButtonDown += StartUp;
        DownButton.OnButtonDown += StartDown;
        UpButton.OnButtonUp += StopUp;
        DownButton.OnButtonUp += StopDown;
    }

    private void StopDown(PointerEventData eventdata)
    {
        DownHeld = false;
    }

    private void StopUp(PointerEventData eventdata)
    {
        UpHeld = false;
    }

    private void StartDown(PointerEventData eventdata)
    {
        DownHeld = true;
    }

    public bool UpHeld { get; private set; }
    public bool DownHeld { get; private set; }

    private void StartUp(PointerEventData eventdata)
    {
        UpHeld = true;
    }

    private void OnDisable()
    {
        UpButton.OnButtonDown -= StartUp;
        DownButton.OnButtonDown -= StartDown;
        UpButton.OnButtonUp -= StopUp;
        DownButton.OnButtonUp -= StopDown;
        numActiveFireButtons = 0;
    }


}
