using UnityEngine;

namespace Fps
{
    public class SessionScreenController : MonoBehaviour
    {
        [SerializeField] private FpsUIButton quickJoinButton;
        [SerializeField] private FpsUIButton browseButton;
        [SerializeField] private PlayerNameInputController playerNameInputController;

        private FrontEndUIController frontEndUIController;

        private const int QuickJoinDefaultIndex = 0;
        private const int QuickJoinCancelIndex = 1;
        private const int QuickJoinJoiningIndex = 2;

        private void Awake()
        {
            Debug.Assert(quickJoinButton != null);
            quickJoinButton.onClick.AddListener(QuickJoinPressed);

            Debug.Assert(browseButton != null);
            browseButton.onClick.AddListener(BrowseButtonPressed);

            frontEndUIController = GetComponentInParent<FrontEndUIController>();
            Debug.Assert(frontEndUIController != null);

            playerNameInputController.OnNameChanged += SetButtonsEnabled;
            Debug.Assert(playerNameInputController != null);
        }

        private void SetButtonsEnabled(bool isValid)
        {
            quickJoinButton.enabled = isValid;
            browseButton.enabled = isValid;
        }

        //TODO Move these to anim?
        public void FindingQuickGame()
        {
            browseButton.enabled = false;
            quickJoinButton.SetText(QuickJoinCancelIndex);
        }

        public void FoundQuickGame()
        {
            quickJoinButton.enabled = false;
            quickJoinButton.SetText(QuickJoinJoiningIndex);
        }

        public void CancelQuickJoin()
        {
            quickJoinButton.SetText(QuickJoinDefaultIndex);
            SetButtonsEnabled(true);
        }

        public void QuickJoinPressed()
        {
            // TODO Implement connection call
            frontEndUIController.SwitchToResultsScreen();
        }

        public void BrowseButtonPressed()
        {
            frontEndUIController.SwitchToLobbyScreen();
        }
    }
}
