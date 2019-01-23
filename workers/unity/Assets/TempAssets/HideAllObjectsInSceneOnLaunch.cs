using UnityEngine;

public class HideAllObjectsInSceneOnLaunch : MonoBehaviour
{
    private void Start()
    {
        foreach (var obj in gameObject.scene.GetRootGameObjects())
        {
            obj.SetActive(false);
        }
    }
}
