using UnityEngine;

[ExecuteInEditMode]
public class UITableGrid : MonoBehaviour
{
    public int numRows;
    public float numcolumns;

    public float rowHeight = 20;
    public Texture backgroundTexture;
    private Rect screenRect;
    public RectTransform viewportRect;
    public Color BGCol1 = new Color(1, 0, 1);
    public Color BGCol2 = new Color(0, 1, 1, .2f);

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Debug.Log(rectTransform.offsetMin + ", " + rectTransform.offsetMax);
    }

    private void OnValidate()
    {
        SetContentHeight();
    }

    private void Start()
    {
        var viewHeight = viewportRect.rect.height;
        Debug.Log("View height:" + viewHeight);
    }

    private void SetContentHeight()
    {
        rectTransform.sizeDelta = new Vector2(0, numRows * rowHeight);
        var desiredHeight = numRows * rowHeight;
        var parentRectHeight = viewportRect.rect.height;
    }

    private void Update()
    {
        screenRect = GetScreenCoordinates(rectTransform);
    }

    private void OnGUI()
    {
        var bg1 = BGCol1;
        var bg2 = BGCol2;

        for (var i = 0; i < 4; i++)
        {
            GUI.DrawTexture(new Rect(screenRect.x, screenRect.y + screenRect.height / 4 * i, screenRect.width,
                    screenRect.height / 4),
                backgroundTexture,
                ScaleMode.StretchToFill, true, 1, bg1, 0, 0);

            var bgx = bg1;
            bg1 = bg2;
            bg2 = bgx;
        }
    }

    private Rect GetScreenCoordinates(RectTransform uiElement)
    {
        var worldCorners = new Vector3[4];
        uiElement.GetWorldCorners(worldCorners);
        worldCorners[0].y = Screen.height - worldCorners[0].y;
        worldCorners[2].y = Screen.height - worldCorners[2].y;
        var result = new Rect(
            worldCorners[0].x,
            worldCorners[0].y,
            worldCorners[2].x - worldCorners[0].x,
            worldCorners[2].y - worldCorners[0].y);
        return result;
    }
}
