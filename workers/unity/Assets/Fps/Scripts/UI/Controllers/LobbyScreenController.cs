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
            startButton.onClick.AddListener(OnStartButtonPressed);

            Debug.Assert(cancelButton != null);
            cancelButton.onClick.AddListener(OnCancelButtonPressed);

            frontEndUIController = GetComponentInParent<FrontEndUIController>();
            Debug.Assert(frontEndUIController != null);

            Debug.Assert(playerNameInputController != null);
            playerNameInputController.OnNameChanged += OnNameUpdated;

            ConnectionStateReporter.OnConnectionStateChange += OnConnectionStateChanged;
        }

        private void OnEnable()
        {
            RefreshButtons();
        }

        private void OnConnectionStateChanged(ConnectionStateReporter.State state, string information)
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

        public void OnStartButtonPressed()
        {
            ConnectionStateReporter.SetState(ConnectionStateReporter.State.Spawning, playerNameInputController.GetPlayerName());
        }

        public void OnCancelButtonPressed()
        {
            ConnectionStateReporter.SetState(ConnectionStateReporter.State.None);
            frontEndUIController.SwitchToSessionScreen();
        }
    }
}
