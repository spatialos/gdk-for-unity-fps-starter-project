using UnityEngine;

namespace Fps
{
    public class UIManager : MonoBehaviour
    {
        public ScreenManager FrontEndController;
        public InGameScreenManager inGameManager;

        private void Awake()
        {
            FrontEndController.gameObject.SetActive(false);
            inGameManager.gameObject.SetActive(false);
        }

        private void Start()
        {
            ShowFrontEnd();
        }

        public void ShowGameView()
        {
            FrontEndController.gameObject.SetActive(false);
            inGameManager.gameObject.SetActive(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void ShowFrontEnd()
        {
            inGameManager.gameObject.SetActive(false);
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
