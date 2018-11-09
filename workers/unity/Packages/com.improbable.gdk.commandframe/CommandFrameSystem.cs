using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

[UpdateInGroup(typeof(PostLateUpdate))]
public class CommandFrameSystem : ComponentSystem
{
    public int CurrentFrame = 0;
    public float FrameLength = 1 / 30f;
    public bool NewFrame = false;

    public float ManualFudge = 1f;
    public float ServerAdjustment = 1f;

    private float remainder = 0;

    protected override void OnUpdate()
    {
        remainder += Time.deltaTime;

        var currentFramelength = FrameLength * ManualFudge * ServerAdjustment;

        if (remainder > currentFramelength)
        {
            remainder -= currentFramelength;
            CurrentFrame += 1;
            NewFrame = true;
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
