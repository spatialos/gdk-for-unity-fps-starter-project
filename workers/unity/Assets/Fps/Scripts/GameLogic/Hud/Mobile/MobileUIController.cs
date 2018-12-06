using UnityEngine;
using UnityEngine.EventSystems;

public class MobileUIController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public bool AreMoving;
    public bool AreLooking;
    public bool AreAiming;
    public bool JumpPressed;

    public StandardButton MoveButton;
    public StandardButton LookButton;
    public StandardButton JumpButton;

    public Vector2 moveDelta;
    public Vector2 moveTotal;
    private int moveTouchId;
    private Vector2 moveStartPoint;

    public Vector2 lookDelta;
    public Vector2 lookTotal;
    private int lookTouchId;
    private Vector2 lookStartPoint;

    public void OnPointerDown(PointerEventData eventData)
    {
        var pointOnLeftOfScreen = eventData.position.x < Screen.width / 2f;
        if (pointOnLeftOfScreen)
        {
            if (!AreMoving)
            {
                StartTrackingMove(eventData);
            }
            else if (!AreLooking)
            {
                StartTrackingLook(eventData);
            }
        }
        else
        {
            if (!AreLooking)
            {
                StartTrackingLook(eventData);
            }
            else if (!AreMoving)
            {
                StartTrackingMove(eventData);
            }
        }

        Debug.Log(
            $"OnPointerDown id " +
            $"{eventData.pointerId} on " +
            $"{(pointOnLeftOfScreen ? "Left" : "Right")} " +
            $"of screen.");
    }


    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log($"Dragging with id {eventData.pointerId}");
        if (AreMoving && eventData.pointerId == moveTouchId)
        {
            moveDelta = eventData.delta;
            moveTotal = eventData.position - moveStartPoint;
            Debug.Log($"moveDelta: {moveDelta} moveTotal: {moveTotal}");
        }

        if (AreLooking && eventData.pointerId == lookTouchId)
        {
            lookDelta = eventData.delta;
            lookTotal = eventData.position - lookStartPoint;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (AreMoving && eventData.pointerId == moveTouchId)
        {
            StopTrackingMove();
        }

        if (AreLooking && eventData.pointerId == lookTouchId)
        {
            StopTrackingLook();
        }
    }

    private void StartTrackingMove(PointerEventData eventData)
    {
        AreMoving = true;
        moveTouchId = eventData.pointerId;
        moveStartPoint = eventData.position;
        Debug.Log($"Tracking movement for id {moveTouchId}");
    }

    private void StartTrackingLook(PointerEventData eventData)
    {
        AreLooking = true;
        lookTouchId = eventData.pointerId;
        lookStartPoint = eventData.position;
    }

    private void StopTrackingMove()
    {
        AreMoving = false;
        moveDelta = Vector2.zero;
        moveTotal = Vector2.zero;
    }

    private void StopTrackingLook()
    {
        lookDelta = Vector2.zero;
        lookTotal = Vector2.zero;
        AreLooking = false;
    }
}
