using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class ConnectScreenController : MonoBehaviour
    {
        public Button ConnectButton;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && ConnectButton.enabled)
            {
                ConnectButton.onClick.Invoke();
            }
        }
    }
}
