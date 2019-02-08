using UnityEngine;

namespace Fps
{
    [RequireComponent(typeof(KillNotificationProvider))]
    public class KillNotificationProviderTester : MonoBehaviour
    {
        public bool GenerateName;

        private void Update()
        {
            if (!GenerateName)
            {
                return;
            }

            GenerateName = false;
            GetComponent<KillNotificationProvider>().AddKill(RandomNameGenerator.GetName());
        }
    }
}
