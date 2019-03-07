using UnityEngine;

namespace Fps
{
    public class SessionScreenController : MonoBehaviour
    {
        [SerializeField] private FpsUIButton quickJoinButton;
        [SerializeField] private FpsUIButton browseButton;

        private FrontEndUIController frontEndUIController;

        private void Awake()
        {
            Debug.Assert(quickJoinButton != null);
            quickJoinButton.onClick.AddListener(QuickJoinPressed);

            Debug.Assert(browseButton != null);
            browseButton.onClick.AddListener(BrowseButtonPressed);

            frontEndUIController = GetComponentInParent<FrontEndUIController>();
            Debug.Assert(frontEndUIController != null);

            ConnectionStateReporter.OnConnectionStateChange += ConnectionStateChanged;
        }

        private void OnEnable()
        {
            RefreshButtons();
        }

        private void ConnectionStateChanged(ConnectionStateReporter.State state, string information)
        {
            if (state == ConnectionStateReporter.State.Connected)
            {
                frontEndUIController.SwitchToLobbyScreen();
                return;
            }

            RefreshButtons();
        }

        private void RefreshButtons()
        {
            var enableButtons = ConnectionStateReporter.HaveDeployments;

            quickJoinButton.enabled = enableButtons;
            browseButton.enabled = enableButtons;
        }


        public void QuickJoinPressed()
        {
            ConnectionStateReporter.TryConnect();
        }

        public void BrowseButtonPressed()
        {
            frontEndUIController.SwitchToDeploymentListScreen();
        }
    }
}
