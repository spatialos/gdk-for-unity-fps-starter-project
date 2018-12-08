using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[SelectionBase]
public class StandardButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Tooltip("How long after pressing the button will register another press")]
    public float CooldownTime;

    [Tooltip("Whether a click dragged out of the button area ends the press or not")]
    public bool RestrictPressToButtonArea;

    [Tooltip("The hitbox used to detect if click is dragged out of the button area")]
    public Graphic Hitbox;

    public bool Togglable;

    private bool isPressed;

    public bool IsPressed => isPressed;

    public delegate void ButtonEvent(PointerEventData eventData);

    public ButtonEvent OnButtonDown;
    public ButtonEvent OnButtonUp;
    public ButtonEvent OnButtonDrag;

    private float lastPressedTime;

    private StandardButtonAnimator[] animator;

    private void OnValidate()
    {
        CooldownTime = Mathf.Max(0, CooldownTime);

        if (Hitbox == null)
        {
            var allGraphics = GetComponentsInChildren<Graphic>();
            foreach (var graphic in allGraphics)
            {
                if (graphic.raycastTarget)
                {
                    Hitbox = graphic;
                    Debug.LogWarning(
                        $"Automatically located and added Hitbox reference to button {gameObject.name}.\n" +
                        $"You may want to assign your own!\n");
                    break;
                }
            }

            Debug.LogWarning($"No hitbox found for button {gameObject.name}!\n");
        }
    }

    private void Awake()
    {
        animator = GetComponentsInChildren<StandardButtonAnimator>();
    }


    private void Start()
    {
        foreach (var anim in animator)
        {
            anim.PlayAnimation(IsPressed
                ? StandardButtonAnimator.AnimType.Pressed
                : StandardButtonAnimator.AnimType.Idle);
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
            Toggle(data);
        }
        else
        {
            Press(data);
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!IsPressed || Togglable)
        {
            return;
        }

        Release(data);
    }

    private void Toggle(PointerEventData eventData)
    {
        if (IsPressed)
        {
            Release(eventData);
        }
        else
        {
            Press(eventData);
        }
    }

    private void Press(PointerEventData eventData)
    {
        if (isPressed)
        {
            return;
        }

        isPressed = true;
        foreach (var anim in animator)
        {
            anim.PlayAnimation(StandardButtonAnimator.AnimType.OnDown);
            anim.QueueAnimation(StandardButtonAnimator.AnimType.Pressed);
        }

        lastPressedTime = Time.time;
        OnButtonDown?.Invoke(eventData);
    }

    private void Release(PointerEventData eventData)
    {
        if (!isPressed)
        {
            return;
        }

        isPressed = false;
        foreach (var anim in animator)
        {
            anim.PlayAnimation(StandardButtonAnimator.AnimType.OnUp);
            anim.QueueAnimation(StandardButtonAnimator.AnimType.Idle);
        }

        OnButtonUp?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsPressed || Togglable)
        {
            return;
        }

        OnButtonDrag?.Invoke(eventData);

        if (!RestrictPressToButtonArea)
        {
            return;
        }

        var relativeCursorPoint = Hitbox.rectTransform.InverseTransformPoint(eventData.position);
        if (!Hitbox.rectTransform.rect.Contains(relativeCursorPoint))
        {
            Release(eventData);
        }
    }
}
