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

    private float lastPressedTime;
    private bool isPressed;

    public bool IsPressed => isPressed;

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
					Debug.LogWarning($"Automatically located and added Hitbox reference to button {gameObject.name}.");
					break;
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

		// TODO Drag event
        var relativeCursorPoint = Hitbox.rectTransform.InverseTransformPoint(eventData.position);
        if (!Hitbox.rectTransform.rect.Contains(relativeCursorPoint))
        {
            EndPress();
        }
    }
}
