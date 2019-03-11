using UnityEngine;

namespace Fps
{
    public class ScreenUIController : MonoBehaviour
    {
        public FrontEndUIController FrontEndController;
        public InGameUIController InGameController;

        private void Awake()
        {
            FrontEndController.gameObject.SetActive(false);
            InGameController.gameObject.SetActive(false);
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
            else if (state == ConnectionStateReporter.State.GatherResults)
            {
                ShowFrontEnd();
                FrontEndController.SwitchToResultsScreen();
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
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void ShowFrontEnd()
        {
            InGameController.gameObject.SetActive(false);
            FrontEndController.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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
