using UnityEngine;

namespace Fps
{
    public class LobbyScreenController : MonoBehaviour
    {
        public ConnectionStatusUIController ConnectionStatusUIController;
        public FpsUIButton startButton;
        public FpsUIButton cancelButton;
        public PlayerNameInputController playerNameInputController;

        public string GetPlayerName()
        {
            return playerNameInputController.GetPlayerName();
        }

        private void Awake()
        {
            playerNameInputController.OnNameChanged += OnNameUpdated;
        }

        private void OnEnable()
        {
            RefreshButtons(true);
        }

        private void OnNameUpdated(bool isValid)
        {
            RefreshButtons(isValid);
        }

        private void RefreshButtons(bool isValid)
        {
            playerNameInputController.DisplayEnterNameHint = isValid;
            playerNameInputController.UpdateHintText();
        }
    }
}
