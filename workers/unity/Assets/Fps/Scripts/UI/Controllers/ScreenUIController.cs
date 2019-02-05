using UnityEngine;

namespace Fps
{
    public class ScreenUIController : MonoBehaviour
    {
        public FrontEndUIController FrontEndController;
        public InGameUIController InGameController;

        private void Awake()
        {
            Debug.Log("ScreenUIC Woke");
            FrontEndController.gameObject.SetActive(false);
            InGameController.gameObject.SetActive(false);
            ConnectionStateReporter.InformOfUIController(this);
            ConnectionStateReporter.OnConnectionStateChange += OnConnectionStateChanged;
        }

        private void OnConnectionStateChanged(ConnectionStateReporter.State state, string information)
        {
            if (state == ConnectionStateReporter.State.Spawned)
            {
                ShowGameView();
            }
            else if (state == ConnectionStateReporter.State.WorkerDisconnected)
            {
                ShowFrontEnd();
            }
        }

        private void Start()
        {
            FrontEndController.gameObject.SetActive(true);
        }

        public void ShowGameView()
        {
            FrontEndController.gameObject.SetActive(false);
            InGameController.gameObject.SetActive(true);
        }

        public void ShowFrontEnd()
        {
            InGameController.gameObject.SetActive(false);
            FrontEndController.gameObject.SetActive(true);
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
