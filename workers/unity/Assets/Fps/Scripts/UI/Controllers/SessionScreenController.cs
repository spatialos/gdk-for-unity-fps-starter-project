using UnityEngine;

namespace Fps
{
    public class SessionScreenController : MonoBehaviour
    {
        [SerializeField] private FpsUIButton quickJoinButton;
        [SerializeField] private FpsUIButton browseButton;
        [SerializeField] private PlayerNameInputController playerNameInputController;

        private FrontEndUIController frontEndUIController;

        private void Awake()
        {
            Debug.Assert(quickJoinButton != null);
            quickJoinButton.onClick.AddListener(QuickJoinPressed);

            Debug.Assert(browseButton != null);
            browseButton.onClick.AddListener(BrowseButtonPressed);

            frontEndUIController = GetComponentInParent<FrontEndUIController>();
            Debug.Assert(frontEndUIController != null);

            playerNameInputController.OnNameChanged += OnNameUpdated;
            Debug.Assert(playerNameInputController != null);

            ConnectionStateReporter.OnConnectionStateChange += ConnectionStateChanged;
        }

        private void ConnectionStateChanged(ConnectionStateReporter.State state, string information)
        {
            playerNameInputController.AllowEdit = ConnectionStateReporter.HaveDeployments;
            RefreshButtons();
        }


        private bool nameIsValid;

        private void OnNameUpdated(bool isValid)
        {
            nameIsValid = isValid;
            RefreshButtons();
        }

        private void RefreshButtons()
        {
            var enableButtons = nameIsValid && ConnectionStateReporter.HaveDeployments;

            quickJoinButton.enabled = enableButtons;
            browseButton.enabled = enableButtons;
        }


        public void QuickJoinPressed()
        {
            ConnectionStateReporter.TryConnect();
        }

        public void BrowseButtonPressed()
        {
            frontEndUIController.SwitchToLobbyScreen();
        }
    }
}
