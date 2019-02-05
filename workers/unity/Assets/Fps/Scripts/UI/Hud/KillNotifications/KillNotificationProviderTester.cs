using UnityEngine;

[RequireComponent(typeof(KillNotificationProvider))]
public class KillNotificationProviderTester : MonoBehaviour
{
    public bool GenerateName;

    void Update()
    {
        if (!GenerateName)
        {
            return;
        }

        GenerateName = false;
        GetComponent<KillNotificationProvider>().AddKill(RandomNameGenerator.GetName());
    }
}
