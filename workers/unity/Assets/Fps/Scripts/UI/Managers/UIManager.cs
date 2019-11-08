using UnityEngine;

namespace Fps.UI
{
    public class UIManager : MonoBehaviour
    {
        public ScreenManager ScreenManager;
        public InGameScreenManager InGameManager;

        private void OnValidate()
        {
            if (ScreenManager == null)
            {
                throw new MissingReferenceException("Missing reference to the screen manager.");
            }

            if (InGameManager == null)
            {
                throw new MissingReferenceException("Missing reference to the in-game screen manager.");
            }
        }

        private void Awake()
        {
            ScreenManager.gameObject.SetActive(false);
            InGameManager.gameObject.SetActive(false);
        }

        private void Start()
        {
            ShowFrontEnd();
            InGameManager.QuitButton.onClick.AddListener(Quit);
            ScreenManager.QuitButton.onClick.AddListener(Quit);
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

        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
