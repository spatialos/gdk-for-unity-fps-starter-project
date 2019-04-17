using System;
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
                throw new NullReferenceException("Missing reference to the connect button.");
            }
        }
    }
}
