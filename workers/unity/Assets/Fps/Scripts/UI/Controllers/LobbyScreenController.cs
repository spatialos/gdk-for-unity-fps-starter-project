using UnityEngine;

namespace Fps
{
    public class LobbyScreenController : MonoBehaviour
    {
        [SerializeField] private FpsUIButton startButton;
        [SerializeField] private FpsUIButton cancelButton;
        [SerializeField] private PlayerNameInputController playerNameInputController;
        private bool nameIsValid;
        private bool gameBegun;

        private FrontEndUIController frontEndUIController;

        private void Awake()
        {
            Debug.Assert(startButton != null);
            startButton.onClick.AddListener(StartButtonPressed);

            Debug.Assert(cancelButton != null);
            cancelButton.onClick.AddListener(CancelButtonPressed);

            frontEndUIController = GetComponentInParent<FrontEndUIController>();
            Debug.Assert(frontEndUIController != null);

            Debug.Assert(playerNameInputController != null);
            playerNameInputController.OnNameChanged += OnNameUpdated;

            ConnectionStateReporter.OnConnectionStateChange += ConnectionStateChanged;
        }

        private void OnEnable()
        {
            RefreshButtons();
        }

        private void ConnectionStateChanged(ConnectionStateReporter.State state, string information)
        {
            gameBegun = state == ConnectionStateReporter.State.GameReady;
            playerNameInputController.DisplayEnterNameHint = gameBegun;
            playerNameInputController.UpdateHintText();
            RefreshButtons();
        }

        private void OnNameUpdated(bool isValid)
        {
            nameIsValid = isValid;
            RefreshButtons();
        }

        private void RefreshButtons()
        {
            startButton.enabled = gameBegun && nameIsValid;
        }

        public void StartButtonPressed()
        {
            ConnectionStateReporter.TrySpawn();
        }

        public void CancelButtonPressed()
        {
            ConnectionStateReporter.TryDisconnect();
            frontEndUIController.SwitchToSessionScreen();
        }
    }
}
