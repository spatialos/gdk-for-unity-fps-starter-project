using UnityEngine;

public class TargetFramerate : MonoBehaviour
{
    public int TargetFrameRate;

    private void Start()
    {
        Application.targetFrameRate = TargetFrameRate;
    }
}
