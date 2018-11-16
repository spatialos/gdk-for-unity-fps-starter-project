using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Experimental.XR;

[UpdateInGroup(typeof(PostLateUpdate))]
public class CommandFrameSystem : ComponentSystem
{
    public int CurrentFrame = 0;
    public const float FrameLength = 1 / 30f;
    public bool NewFrame = false;

    public float ManualFudge = 1f;
    public float ServerAdjustment = 1f;

    private float remainder = 0;

    protected override void OnUpdate()
    {
        remainder += Time.deltaTime;

        var currentFramelength = FrameLength;
        var frameIncrement = 1;
        var isNewFrame = true;
        if (ServerAdjustment > 0)
        {
            frameIncrement = 0;
            isNewFrame = false;
        }
        else if (ServerAdjustment < 0)
        {
            frameIncrement = 2;
            isNewFrame = true;
        }

        if (remainder > currentFramelength)
        {
            remainder -= currentFramelength;
            CurrentFrame += frameIncrement;
            NewFrame = isNewFrame;

            if (remainder > currentFramelength)
            {
                while (remainder > currentFramelength)
                {
                    // Debug.LogWarningFormat("Missed Frame {0}", CurrentFrame);
                    remainder -= currentFramelength;
                    CurrentFrame += frameIncrement;
                    ServerAdjustment += (frameIncrement - 1);
                }
            }
        }
        else if (remainder + Time.deltaTime > currentFramelength)
        {
            var deltaNow = currentFramelength - remainder;
            var deltaNext = remainder + Time.deltaTime - currentFramelength;
            // Debug.LogFormat("[{0}] Will Overshoot next frame. now: {1:00.0} next: {2:00.0}",
            //     CurrentFrame, deltaNow * 1000f, deltaNext * 1000f);
            if (deltaNow < deltaNext)
            {
                // Debug.LogFormat("[{0}] Closer this frame. Undershoot.", CurrentFrame);
                remainder -= currentFramelength;
                CurrentFrame += frameIncrement;
                ServerAdjustment += (frameIncrement - 1);
                NewFrame = isNewFrame;
            }
            else
            {
                NewFrame = false;
            }
        }
        else
        {
            NewFrame = false;
        }
    }

    public float GetRemainder()
    {
        return remainder;
    }
}
