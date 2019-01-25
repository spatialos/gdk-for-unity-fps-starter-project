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
        }

        private void Start()
        {
            FrontEndController.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Forced disconnect TRUE");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Forced disconnect FALSE");
            }
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

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
