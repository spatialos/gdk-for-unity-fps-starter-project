using UnityEngine;

namespace Fps
{
    public class SessionScreenController : MonoBehaviour
    {
        [SerializeField] private FpsUIButton quickJoinButton;
        [SerializeField] private FpsUIButton browseButton;

        private FrontEndUIController frontEndUIController;
        private bool nameIsValid;

        private void Awake()
        {
            quickJoinButton.onClick.AddListener(OnQuickJoinPressed);
            browseButton.onClick.AddListener(OnBrowseButtonPressed);

            frontEndUIController = GetComponentInParent<FrontEndUIController>();

            ConnectionStateReporter.OnConnectionStateChange += OnConnectionStateChanged;
        }

        private void OnEnable()
        {
            RefreshButtons();
        }

        private void OnConnectionStateChanged(ConnectionStateReporter.State state, string information)
        {
            Debug.Log(state);
            if (state == ConnectionStateReporter.State.Connected)
            {
                frontEndUIController.SwitchToLobbyScreen();
                return;
            }

            RefreshButtons();
        }

        private void RefreshButtons()
        {
            var enableButtons = ConnectionStateReporter.State.DeploymentListAvailable == ConnectionStateReporter.CurrentState;

            quickJoinButton.enabled = enableButtons;
            browseButton.enabled = enableButtons;
        }


        public void OnQuickJoinPressed()
        {
            ConnectionStateReporter.SetState(ConnectionStateReporter.State.QuickJoin);
        }

        public void OnBrowseButtonPressed()
        {
            frontEndUIController.SwitchToDeploymentListScreen();
        }
    }
}
