using System;
using UnityEngine;

public class MobileAnalogueControls : MonoBehaviour
{
    public Vector2 MoveDelta { get; private set; }
    public Vector2 MoveTotal { get; private set; }
    public Vector2 LookDelta { get; private set; }
    public Vector2 LookTotal { get; private set; }


    private bool areMoving;
    private bool areLooking;

    private int moveFingerId;
    private Vector2 moveStartPosition;
    private Vector2 moveLastPosition;

    private int lookFingerId;
    private Vector2 lookStartPosition;
    private Vector2 lookLastPosition;

    private void Update()
    {
        CheckForStoppedTouches();
        CheckForStartedTouches();

        UpdateMoveTouch();
        UpdateLookTouch();
    }

    private void OnDisable()
    {
        LookDelta = Vector2.zero;
        LookTotal = Vector2.zero;
        MoveDelta = Vector2.zero;
        MoveTotal = Vector2.zero;
    }

    public void AdjustMoveStartPosition(Vector2 offset)
    {
        moveStartPosition += offset;
    }

    private void CheckForStoppedTouches()
    {
        if (areMoving)
        {
            if (!TouchExists(moveFingerId))
            {
                StopTrackingMove();
            }
            else
            {
                var phase = GetTouchFromFingerId(moveFingerId).phase;
                if (phase == TouchPhase.Ended || phase == TouchPhase.Canceled)
                {
                    StopTrackingMove();
                }
            }
        }

        if (areLooking)
        {
            if (!TouchExists(lookFingerId))
            {
                StopTrackingLook();
            }
            else
            {
                var phase = GetTouchFromFingerId(lookFingerId).phase;
                if (phase == TouchPhase.Ended || phase == TouchPhase.Canceled)
                {
                    StopTrackingLook();
                }
            }
        }
    }

    private void CheckForStartedTouches()
    {
        foreach (var touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Began)
            {
                continue;
            }

            var leftOfScreen = touch.position.x < Screen.width / 2f;
            if (leftOfScreen)
            {
                if (!areMoving)
                {
                    StartTrackingMove(touch);
                }
                else if (!areLooking)
                {
                    StartTrackingLook(touch);
                }
            }
            else
            {
                if (!areLooking)
                {
                    StartTrackingLook(touch);
                }
                else if (!areMoving)
                {
                    StartTrackingMove(touch);
                }
            }
        }
    }


    private void UpdateMoveTouch()
    {
        if (!areMoving)
        {
            return;
        }

        var position = GetTouchFromFingerId(moveFingerId).position;
        MoveTotal = position - moveStartPosition;


        MoveDelta = position - moveLastPosition;
        moveLastPosition = position;
    }

    private void UpdateLookTouch()
    {
        if (!areLooking)
        {
            return;
        }

        var position = GetTouchFromFingerId(lookFingerId).position;
        LookTotal = position - lookStartPosition;
        LookDelta = position - lookLastPosition;
        lookLastPosition = position;
    }


    private void StartTrackingMove(Touch touch)
    {
        areMoving = true;
        moveFingerId = touch.fingerId;
        moveStartPosition = touch.position;
        moveLastPosition = moveStartPosition;
    }

    private void StartTrackingLook(Touch touch)
    {
        areLooking = true;
        lookFingerId = touch.fingerId;
        lookStartPosition = touch.position;
        lookLastPosition = lookStartPosition;
    }


    private void StopTrackingMove()
    {
        areMoving = false;
        MoveDelta = Vector2.zero;
        MoveTotal = Vector2.zero;
    }

    private void StopTrackingLook()
    {
        areLooking = false;
        LookDelta = Vector2.zero;
        LookTotal = Vector2.zero;
    }


    private bool TouchExists(int fingerId)
    {
        for (var i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].fingerId == fingerId)
            {
                return true;
            }
        }

        return false;
    }

    private Touch GetTouchFromFingerId(int fingerId)
    {
        for (var i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].fingerId == fingerId)
            {
                return Input.touches[i];
            }
        }

        throw new ArgumentException($"No touch found with fingerId {fingerId}");
    }

    private void DrawMoveAreaGUI()
    {
        GUI.Box(new Rect(moveStartPosition.x - 100, Screen.height - (moveStartPosition.y + 100), 200, 200), "");
    }
}
