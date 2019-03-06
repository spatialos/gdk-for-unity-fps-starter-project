using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float deltaTime = 0.0f;

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        var style = new GUIStyle();

        var rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = new Color(1.0f, 0.2f, 0.1f, 1.0f);
        var msec = deltaTime * 1000.0f;
        var fps = 1.0f / deltaTime;
        var text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
