using UnityEngine;
using UnityEngine.UI;

namespace Fps.UI
{
    public class ScreenManager : MonoBehaviour
    {
        [Header("Screens")]
        public GameObject DefaultScreen;

        [Header("Buttons")]
        public Button QuitButton;
        public Button DefaultConnectButton;

        [SerializeField] private GameObject FrontEndCamera;

        private GameObject currentScreen;

        private void OnValidate()
        {
            if (DefaultScreen == null)
            {
                throw new MissingReferenceException("Missing reference to the default screen.");
            }

            if (FrontEndCamera == null)
            {
                throw new MissingReferenceException("Missing reference to the front end camera.");
            }

            if (QuitButton == null)
            {
                throw new MissingReferenceException("Missing reference to the quit button.");
            }

            if (DefaultConnectButton == null)
            {
                throw new MissingReferenceException("Missing reference to the connect button.");
            }
        }

        public void SwitchToDefaultScreen()
        {
            SetScreenTo(DefaultScreen);
        }

        private void Awake()
        {
            DefaultScreen.SetActive(false);
        }

        private void OnEnable()
        {
            FrontEndCamera.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnDisable()
        {
            FrontEndCamera.SetActive(false);
        }

        private void SetScreenTo(GameObject screenObject)
        {
            if (currentScreen != null)
            {
                currentScreen.gameObject.SetActive(false);
            }

            currentScreen = screenObject;
            currentScreen.gameObject.SetActive(true);
        }
    }
}
