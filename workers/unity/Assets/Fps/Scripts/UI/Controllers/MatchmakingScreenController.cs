using UnityEngine;

namespace Fps
{
    public class MatchmakingScreenController : MonoBehaviour
    {
        public InputFieldPlayerName PlayerNameInputField;
        public FpsUIButton QuickJoinButton;
        public FpsUIButton BrowseButton;
        public FrontEndUIController frontEndUIController;

        private const int quickJoinDefaultIndex = 0;
        private const int quickJoinCancelIndex = 1;
        private const int quickJoinJoiningIndex = 2;

        private void Awake()
        {
            frontEndUIController = GetComponentInParent<FrontEndUIController>();
            PlayerNameInputField.OnNameChanged += SetButtonsEnabled;
            Debug.Assert(PlayerNameInputField != null);
            Debug.Assert(BrowseButton != null);
            Debug.Assert(QuickJoinButton != null);
        }

        private void SetButtonsEnabled(bool isValid)
        {
            isValid = true;
            Debug.LogFormat("<b>Setting button enableds to " + isValid + "</b>");
            QuickJoinButton.enabled = isValid;
            BrowseButton.enabled = isValid;
        }

        //TODO Move these to anim?
        public void FindingQuickGame()
        {
            BrowseButton.enabled = false;
            QuickJoinButton.SetText(quickJoinCancelIndex);
        }

        public void FoundQuickGame()
        {
            QuickJoinButton.enabled = false;
            QuickJoinButton.SetText(quickJoinJoiningIndex);
        }

        public void CancelQuickJoin()
        {
            QuickJoinButton.SetText(quickJoinDefaultIndex);
            SetButtonsEnabled(true);
        }

        public void QuickJoinPressed()
        {

        }

        public void BrowsePressed()
        {
            frontEndUIController.SetScreenTo(FrontEndUIController.ScreenType.Lobby);
        }
    }
}
