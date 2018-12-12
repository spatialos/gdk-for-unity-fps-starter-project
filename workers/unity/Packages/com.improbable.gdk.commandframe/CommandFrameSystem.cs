using System.Security.Cryptography;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

[UpdateInGroup(typeof(PostLateUpdate))]
public class CommandFrameSystem : ComponentSystem
{
    public int CurrentFrame = 0;
    public const float FrameLength = 1 / 28f;
    public const int AdjustmentFrames = 20;
    public const float FrameAdjustment = FrameLength / AdjustmentFrames;
    public bool NewFrame;

    public float ServerAdjustment = 0f;

    private float remainder = 0;
    public float currentFrameAdjustment = 0f;
    public int adjustmentFramesLeft = 0;

    private bool isPaused;

    protected override void OnUpdate()
    {
        if (isPaused)
        {
            return;
        }

        remainder += Time.deltaTime;
        NewFrame = false;

        if (ServerAdjustment > 0)
        {
            ServerAdjustment--;
            adjustmentFramesLeft = AdjustmentFrames;
            currentFrameAdjustment = FrameAdjustment;
        }
        else if (ServerAdjustment < 0)
        {
            ServerAdjustment++;
            adjustmentFramesLeft = AdjustmentFrames;
            currentFrameAdjustment = -FrameAdjustment;
        }

        var currentFrameLength = FrameLength + currentFrameAdjustment;

        if (remainder > currentFrameLength)
        {
            remainder -= currentFrameLength;
            CurrentFrame += 1;
            NewFrame = true;

            if (remainder > currentFrameLength)
            {
                while (remainder > currentFrameLength)
                {
                    remainder -= currentFrameLength;
                    CurrentFrame += 1;
                }
            }
        }
        else if (remainder + Time.deltaTime > currentFrameLength)
        {
            var deltaNow = currentFrameLength - remainder;
            var deltaNext = remainder + Time.deltaTime - currentFrameLength;
            if (deltaNow < deltaNext)
            {
                remainder -= currentFrameLength;
                CurrentFrame += 1;
                NewFrame = true;
            }
            else
            {
                NewFrame = false;
            }
        }

        if (NewFrame && adjustmentFramesLeft > 0)
        {
            adjustmentFramesLeft--;
            if (adjustmentFramesLeft == 0)
            {
                currentFrameAdjustment = 0;
            }
        }
    }

    public float GetRemainder()
    {
        return remainder;
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
