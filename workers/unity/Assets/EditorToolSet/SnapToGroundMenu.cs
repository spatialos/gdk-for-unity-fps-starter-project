using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEditor;
public class SnapToGroundMenu : MonoBehaviour
{
    [MenuItem("ImprobaTools/Snap to floor %END")]
    private static void PerformSnapToGroundMenu()
    {
        Undo.RecordObjects(Selection.transforms, "Snap to floor");
        SnapToGround(Selection.transforms, 0.1f);
    }
    //TODO Move this function into general static class
    private static void SnapToGround(Transform[] transforms, float skinWidth = 0.1f)
    {
        foreach (var t in transforms)
        {
            RaycastHit[] hits;

            hits = Physics.RaycastAll(t.position + Vector3.up * skinWidth, Vector3.down, 1000);

            var nearestHit = new RaycastHit();
            nearestHit.distance = float.MaxValue;

            for (int i = 0; i < hits.Length; i++)
            {
                var hitValid = true;

                // Check against objects we're snapping - we want to ignore them!
                for (int j = 0; j < Selection.transforms.Length; j++)
                {
                    if (hits[i].collider.transform == Selection.transforms[j])
                    {
                        hitValid = false;
                        break;
                    }
                }

                if (!hitValid)
                {
                    continue;
                }

                if (hits[i].distance < nearestHit.distance)
                {
                    nearestHit = hits[i];
                }
            }

            if (nearestHit.distance < float.MaxValue)
            {
                t.position = nearestHit.point;
            }
        }
    }
}
