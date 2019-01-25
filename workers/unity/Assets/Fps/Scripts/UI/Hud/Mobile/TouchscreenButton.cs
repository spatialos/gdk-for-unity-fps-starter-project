using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[SelectionBase]
public class TouchscreenButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Tooltip("How long after pressing the button will register another press")]
    public float CooldownTime;

    [Tooltip("Whether a click dragged out of the button area ends the press or not")]
    public bool RestrictPressToButtonArea;

    [Tooltip("The hitbox used to detect if click is dragged out of the button area")]
    public Graphic Hitbox;

    public bool Togglable;

    public bool IsPressed { get; private set; }

    public delegate void ButtonEvent();

    public ButtonEvent OnButtonDown;
    public ButtonEvent OnButtonUp;
    public ButtonEvent OnButtonDrag;

    private float lastPressedTime;

    private TouchscreenButtonAnimator[] animators;

    private void OnValidate()
    {
        CooldownTime = Mathf.Max(0, CooldownTime);
        OnValidateCheckHitbox();
    }

    private void OnValidateCheckHitbox()
    {
        if (Hitbox != null)
        {
            return;
        }

        var allGraphics = GetComponentsInChildren<Graphic>();
        foreach (var graphic in allGraphics)
        {
            if (!graphic.raycastTarget)
            {
                continue;
            }

            Hitbox = graphic;
            Debug.LogWarning(
                $"Automatically located and added Hitbox reference to button {gameObject.name}.\n" +
                $"You may want to assign your own!\n");
            break;
        }

        Debug.LogWarning($"No hitbox found for button {gameObject.name}!\n");
    }

    private void Awake()
    {
        animators = GetComponentsInChildren<TouchscreenButtonAnimator>();
    }

    // Presently it is assumed the player is dead if the UI is disabled, so reset everything
    private void OnDisable()
    {
        Release();
    }

    private void Start()
    {
        foreach (var anim in animators)
        {
            anim.PlayAnimation(IsPressed
                ? TouchscreenButtonAnimator.AnimType.Pressed
                : TouchscreenButtonAnimator.AnimType.Idle);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (Time.time - lastPressedTime < CooldownTime)
        {
            return;
        }

        if (Togglable)
        {
            Toggle();
        }
        else
        {
            Press();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsPressed || Togglable)
        {
            return;
        }

        Release();
    }

    private void Toggle()
    {
        if (IsPressed)
        {
            Release();
        }
        else
        {
            Press();
        }
    }

    private void Press()
    {
        if (IsPressed)
        {
            return;
        }

        IsPressed = true;
        foreach (var anim in animators)
        {
            anim.PlayAnimation(TouchscreenButtonAnimator.AnimType.OnDown);
            anim.QueueAnimation(TouchscreenButtonAnimator.AnimType.Pressed);
        }

        lastPressedTime = Time.time;
        OnButtonDown?.Invoke();
    }

    private void Release()
    {
        if (!IsPressed)
        {
            return;
        }

        IsPressed = false;
        foreach (var anim in animators)
        {
            anim.PlayAnimation(TouchscreenButtonAnimator.AnimType.OnUp);
            anim.QueueAnimation(TouchscreenButtonAnimator.AnimType.Idle);
        }

        OnButtonUp?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsPressed || Togglable)
        {
            return;
        }

        if (RestrictPressToButtonArea)
        {
            var relativeCursorPoint = Hitbox.rectTransform.InverseTransformPoint(eventData.position);
            if (!Hitbox.rectTransform.rect.Contains(relativeCursorPoint))
            {
                Release();
            }

            return;
        }

        OnButtonDrag?.Invoke();
    }
}
