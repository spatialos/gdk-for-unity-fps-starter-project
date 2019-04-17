using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class DefaultConnectScreenManager : MonoBehaviour
    {
        public Button ConnectButton;

        private void OnValidate()
        {
            if (ConnectButton == null)
            {
                throw new MissingReferenceException("Missing reference to the connect button.");
            }
        }
    }
}
