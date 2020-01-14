using UnityEngine;

namespace Fps.Config
{
    public class SettingsPublisher : MonoBehaviour
    {
        private void Awake()
        {
            foreach (var component in GetComponentsInParent<MonoBehaviour>())
            {
                if (component is ISettingsPublisher publisher)
                {
                    publisher.Publish();
                }
            }
        }
    }
}
