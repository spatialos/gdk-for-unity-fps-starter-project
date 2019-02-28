using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;

#endif


public class TileFootprintHelper : MonoBehaviour
{
#if UNITY_EDITOR
    public float InnerTileSize = 8 * 4;
    public float ConnectorLength = 8;
    public float ConnectorDepth = 4;
    public Color Color = new Color(.2f, .7f, .2f, .4f);

    private void OnDrawGizmos()
    {
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage()?.scene.name;

        if (prefabStage == null)
        {
            return;
        }

        var prefabName = PrefabUtility.GetCorrespondingObjectFromSource(gameObject)?.name;

        if (prefabName != null)
        {
            if (prefabStage != prefabName)
            {
                return;
            }
        }

        DrawFootprint();
    }

    private void OnDrawGizmosSelected()
    {
        var prefabName = PrefabUtility.GetCorrespondingObjectFromSource(gameObject)?.name;
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage()?.scene.name;

        if (prefabName == prefabStage)
        {
            // Already drawing in OnDraw
            return;
        }

        DrawFootprint();
    }

    private void DrawFootprint()
    {
        Gizmos.color = Color;

        DrawFlatSquare(transform.position, Vector2.one * InnerTileSize);
        DrawFlatSquare(
            transform.position + Quaternion.Euler(0, 0, 0) *
            new Vector3(.5f * ConnectorLength, 0, InnerTileSize * .5f + .5f * ConnectorDepth),
            new Vector2(ConnectorLength, ConnectorDepth));
        DrawFlatSquare(
            transform.position + Quaternion.Euler(0, 90, 0) *
            new Vector3(.5f * ConnectorLength, 0, InnerTileSize * .5f + .5f * ConnectorDepth),
            new Vector2(ConnectorDepth, ConnectorLength));
        DrawFlatSquare(
            transform.position + Quaternion.Euler(0, 180, 0) *
            new Vector3(.5f * ConnectorLength, 0, InnerTileSize * .5f + .5f * ConnectorDepth),
            new Vector2(ConnectorLength, ConnectorDepth));
        DrawFlatSquare(
            transform.position + Quaternion.Euler(0, 270, 0) *
            new Vector3(.5f * ConnectorLength, 0, InnerTileSize * .5f + .5f * ConnectorDepth),
            new Vector2(ConnectorDepth, ConnectorLength));
    }

    private void DrawFlatSquare(Vector3 position, Vector2 size)
    {
        var p0 = position + new Vector3(-size.x, 0, -size.y) * 0.5f;
        var p1 = position + new Vector3(-size.x, 0, size.y) * 0.5f;
        var p2 = position + new Vector3(size.x, 0, size.y) * 0.5f;
        var p3 = position + new Vector3(size.x, 0, -size.y) * 0.5f;

        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p0);
    }
#endif
}
