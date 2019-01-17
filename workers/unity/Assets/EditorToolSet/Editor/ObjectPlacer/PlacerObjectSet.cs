using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlacerObjectSet
{
    // You must update PlacerObjectPropertyDrawer.cs if adding/removing/renaming these settings!
    public List<GameObject> Objects;

    public bool AlignToNormal;
    public float AlignMaxAngle = 45f;

    public bool RandomYaw;
    public float YawMin = 0f;
    public float YawMax = 360f;

    public bool RandomPitch;
    public float RandomPitchValue;

    public bool LimitSlope;
    public float ValidSlopeMin = 0f;
    public float ValidSlopeMax = 45f;

    public bool ApplyScale;
    public float ScaleMin = .9f;
    public float ScaleMax = 1.1f;

    public bool ApplyZOffset;
    public float MinZOffset = 0;
    public float MaxZOffset = 0;
}
