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

    public bool IsPressed { get; private set; }

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
                    Debug.LogWarning($"Automatically located and added Hitbox reference to button {gameObject.name}. " +
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
            anim.PlayAnimation(StandardButtonAnimator.EAnimType.Idle);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (Time.time - lastPressedTime < CooldownTime)
        {
            return;
        }

        Press(data);
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!IsPressed)
        {
            return;
        }

        EndPress(data);
    }

    private void Press(PointerEventData eventData)
    {
        IsPressed = true;
        foreach (var anim in animator)
        {
            anim.PlayAnimation(StandardButtonAnimator.EAnimType.OnDown);
            anim.QueueAnimation(StandardButtonAnimator.EAnimType.Pressed);
        }

        lastPressedTime = Time.time;
        OnButtonDown?.Invoke(eventData);
    }

    private void EndPress(PointerEventData eventData)
    {
        foreach (var anim in animator)
        {
            anim.PlayAnimation(StandardButtonAnimator.EAnimType.OnUp);
            anim.QueueAnimation(StandardButtonAnimator.EAnimType.Idle);
        }

        IsPressed = false;
        OnButtonUp?.Invoke(eventData);
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (!IsPressed)
        {
            return;
        }

        OnButtonDrag?.Invoke(eventData);

        if (RestrictPressToButtonArea)
        {
            var relativeCursorPoint = Hitbox.rectTransform.InverseTransformPoint(eventData.position);
            if (!Hitbox.rectTransform.rect.Contains(relativeCursorPoint))
            {
                EndPress(eventData);
            }
        }
    }
}
