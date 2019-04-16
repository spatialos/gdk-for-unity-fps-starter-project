using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class InGameScreenManager : MonoBehaviour
    {
        public GameObject RespawnScreen;
        public GameObject Reticle;
        public GameObject Hud;
        public GameObject EscapeScreen;
        public Button QuitButton;

        public static bool InEscapeMenu { get; private set; }
        private bool isPlayerAiming;

        private void Awake()
        {
            QuitButton.onClick.AddListener(UIManager.Quit);
        }

        public void OnEnable()
        {
            Hud.SetActive(true);
            Reticle.SetActive(true);
            RespawnScreen.SetActive(false);
            SetEscapeScreen(false);
            SetPlayerAiming(false);
        }

        public void OnDisable()
        {
            InEscapeMenu = false;
            isPlayerAiming = false;
        }

        public void TryOpenSettingsMenu()
        {
            if (!gameObject.activeInHierarchy || RespawnScreen.activeInHierarchy)
            {
                return;
            }

            SetEscapeScreen(!EscapeScreen.activeInHierarchy);
        }

        public void SetEscapeScreen(bool inEscapeScreen)
        {
            EscapeScreen.SetActive(inEscapeScreen);
            Reticle.SetActive(!inEscapeScreen && !isPlayerAiming);

            Cursor.visible = inEscapeScreen;
            Cursor.lockState = inEscapeScreen ? CursorLockMode.Confined : CursorLockMode.Locked;

            // Inform the static variable, for FpsDriver etc.
            InEscapeMenu = inEscapeScreen;
        }

        public void SetPlayerAiming(bool isAiming)
        {
            isPlayerAiming = isAiming;
            Reticle.SetActive(!isPlayerAiming);
        }
    }
}
