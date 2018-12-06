using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[SelectionBase]
public class StandardButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public float CooldownTime;

    public bool RestrictPressToButtonArea;

    public Graphic Hitbox;

    private float lastPressedTime;

    private bool isPressed;

    public bool IsPressed { get; }

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
                }
            }

            Debug.LogWarning($"No hitbox found for button {gameObject.name}!");
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

        Press();
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!isPressed)
        {
            return;
        }

        EndPress();
    }

    private void Press()
    {
        isPressed = true;

        foreach (var anim in animator)
        {
            anim.PlayAnimation(StandardButtonAnimator.EAnimType.OnDown);
            anim.QueueAnimation(StandardButtonAnimator.EAnimType.Pressed);
        }

        //TODO Fire event
        lastPressedTime = Time.time;
    }

    private void EndPress()
    {
        foreach (var anim in animator)
        {
            anim.PlayAnimation(StandardButtonAnimator.EAnimType.OnUp);
            anim.QueueAnimation(StandardButtonAnimator.EAnimType.Idle);
        }

        // TODO Fire event
        isPressed = false;
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (!RestrictPressToButtonArea)
        {
            return;
        }

        var relativeCursorPoint = Hitbox.rectTransform.InverseTransformPoint(eventData.position);
        if (!Hitbox.rectTransform.rect.Contains(relativeCursorPoint))
        {
            EndPress();
        }
    }
}
