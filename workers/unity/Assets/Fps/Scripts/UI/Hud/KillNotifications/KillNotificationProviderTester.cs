using UnityEngine;

[RequireComponent(typeof(KillNotificationProvider))]
public class KillNotificationProviderTester : MonoBehaviour
{
    public bool GenerateName;

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (GenerateName)
        {
            GenerateName = false;
            GetComponent<KillNotificationProvider>().AddKill(RandomNameGenerator.GetName());
        }
    }
}
