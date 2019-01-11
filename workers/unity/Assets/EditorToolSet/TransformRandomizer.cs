using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class TransformRandomizer  {

    private static void PerformRandomizePosition(Transform[] objects, bool relative, Space space, bool ignoreX, bool ignoreY, bool ignoreZ, Vector3 min, Vector3 max)
    {
        Undo.RecordObjects(Selection.transforms, "Randomize Position");
        foreach (var transform in objects)
        {
     
                    var position = new Vector3
                    {
                        x = !ignoreX
                            ? Random.Range(min.x, max.x)
                            : relative
                                ? 0
                                : space == Space.Self
                                    ? transform.localPosition.x
                                    : transform.position.x,
                        y = !ignoreY
                            ? Random.Range(min.y, max.y)
                            : relative
                                ? 0
                                : space == Space.Self
                                    ? transform.localPosition.y
                                    : transform.position.y,
                        z = !ignoreZ
                            ? Random.Range(min.z, max.z)
                            : relative
                                ? 0
                                : space == Space.Self
                                    ? transform.localPosition.z
                                    : transform.position.z
                    };

                    switch (relative)
                    {
                        case true:
                            transform.Translate(position, space);
                            break;
                        case false:
                            switch (space)
                            {
                                case Space.World:
                                    transform.position = position;
                                    break;
                                case Space.Self:
                                    transform.localPosition = position;
                                    break;
                                default:
                                    throw new System.ArgumentOutOfRangeException();
                            }
                            break;
                    }
            
        }
    }
}
