using System;
using Unity.Entities;
using UnityEngine;

public class CommandFrameSystem : ComponentSystem
{
    public int CurrentFrame = 0;
    public float FrameLength = 1 / 30f;

    private float remainder = 0;

    protected override void OnUpdate()
    {
        remainder += Time.deltaTime;

        if (remainder > FrameLength)
        {
            remainder -= FrameLength;
            CurrentFrame += 1;
        }
    }

    public float GetPartial()
    {
        return remainder / FrameLength;
    }
}
