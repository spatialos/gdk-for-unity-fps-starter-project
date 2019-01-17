using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlacerObjectSet.asset", menuName = "Placer Object Set")]
public class PlacerObjectSetInstance : ScriptableObject
{
    public PlacerObjectSet Content;
    void OnValidate()
    {
        Content.AlignMaxAngle= Mathf.Clamp(Content.AlignMaxAngle, 0, 180);

        FixMinMaxFloatValues(ref Content.YawMin, ref Content.YawMax);

        Content.RandomPitchValue = Mathf.Clamp(Content.RandomPitchValue, 0, 180);

        FixMinMaxFloatValues(ref Content.ValidSlopeMin, ref Content.ValidSlopeMax);
        Content.ValidSlopeMin = Mathf.Max(Content.ValidSlopeMin, 0);
        Content.ValidSlopeMax = Mathf.Min(Content.ValidSlopeMax, 180);

        FixMinMaxFloatValues(ref Content.ScaleMin, ref Content.ScaleMax);
        FixMinMaxFloatValues(ref Content.MinZOffset, ref Content.MaxZOffset);
    }

    void FixMinMaxFloatValues(ref float min, ref float max)
    {
        if (min <= max)
        {
            return;
        }
        var temp = max;
        max = min;
        min = temp;
    }

}

