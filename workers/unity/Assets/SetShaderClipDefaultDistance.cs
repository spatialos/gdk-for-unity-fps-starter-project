using UnityEngine;

public class SetShaderClipDefaultDistance : MonoBehaviour
{
    private void Start()
    {
        Shader.SetGlobalFloat("_GlobalClipDistance", 300f);
    }
}
