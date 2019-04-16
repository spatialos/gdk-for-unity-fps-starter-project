using UnityEngine;

namespace Fps
{
    public class UIManager : MonoBehaviour
    {
        public ScreenManager ScreenManager;
        public InGameScreenManager InGameManager;

        private void Awake()
        {
            ScreenManager.gameObject.SetActive(false);
            InGameManager.gameObject.SetActive(false);
        }

        private void Start()
        {
            ShowFrontEnd();
        }

        public void ShowGameView()
        {
            ScreenManager.gameObject.SetActive(false);
            InGameManager.gameObject.SetActive(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void ShowFrontEnd()
        {
            InGameManager.gameObject.SetActive(false);
            ScreenManager.gameObject.SetActive(true);
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
