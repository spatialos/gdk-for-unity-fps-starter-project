using UnityEngine;

namespace Fps
{
    public class ScreenUIController : MonoBehaviour
    {
        public static bool InEscapeMenu { get; private set; }

        public GameObject ConnectScreen;
        public GameObject InGameHud;
        public GameObject RespawnScreen;
        public GameObject EscapeScreen;
        public GameObject Reticle;
        public GameObject UICamera;
        private Animator connectAnimator;
        private bool isPlayerAiming;

        // Use this for initialization
        private void Start()
        {
            ShowConnectScreen();
            connectAnimator = ConnectScreen.GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        private void Update()
        {
            // Only allow escape screen if in-game, and not respawning.
            if (Input.GetKeyDown(KeyCode.Escape)
                && !ConnectScreen.activeInHierarchy
                && !RespawnScreen.activeInHierarchy)
            {
                SetEscapeScreen(!EscapeScreen.activeInHierarchy);
            }

            if (Input.GetKeyDown(KeyCode.Space)
                && ConnectScreen.activeInHierarchy)
            {
                ClientWorkerHandler.ConnectionController.ConnectAction();
            }
        }

        public void ConnectClicked()
        {
            ClientWorkerHandler.ConnectionController.ConnectAction();
        }

        public void SetPlayerAiming(bool isAiming)
        {
            isPlayerAiming = isAiming;
            Reticle.SetActive(!isPlayerAiming);
        }

        public void ShowConnectScreen()
        {
            ConnectScreen.SetActive(true);
            UICamera.SetActive(true);

            InGameHud.SetActive(false);
            RespawnScreen.SetActive(false);
            EscapeScreen.SetActive(false);
            Reticle.SetActive(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            InEscapeMenu = false;
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

        public void OnDisconnect()
        {
            // Worker has been disconnected.
            ShowConnectScreen();
            connectAnimator.SetTrigger("Disconnected");
        }
    }
}
