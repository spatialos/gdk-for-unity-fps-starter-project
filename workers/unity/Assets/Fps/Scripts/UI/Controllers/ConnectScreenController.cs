using System;
using UnityEngine;

namespace Fps
{
    public class ConnectScreenController : MonoBehaviour
    {
        public event Action OnConnectClicked;

        public void ConnectClicked()
        {
            OnConnectClicked.Invoke();
        }
    }
}
