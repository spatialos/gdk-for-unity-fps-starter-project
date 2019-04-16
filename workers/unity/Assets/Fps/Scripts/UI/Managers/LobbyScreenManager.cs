using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class LobbyScreenManager : ConnectionStatusManager
    {
        public FpsUIButton StartButton;
        public FpsUIButton CancelButton;

        [SerializeField] private Text hintText;
        [SerializeField] private InputField inputField;

        private readonly Color HintTextColor = new Color(1f, .4f, .4f);

        public void UpdateHintText(bool hasGameBegun)
        {
            var nameLength = inputField.text.Trim().Length;

            if (nameLength == 0 && !hasGameBegun)
            {
                hintText.text = "You must enter a name to play";
            }
            else if (nameLength < 3)
            {
                hintText.text = "Minimum 3 characters required";
            }
            else
            {
                hintText.text = string.Empty;
            }
        }

        public string GetPlayerName()
        {
            return inputField.text;
        }

        public bool IsValidName()
        {
            return inputField.text.Length >= 3;
        }

        private void Awake()
        {
            hintText.text = string.Empty;
            hintText.color = HintTextColor;
        }

        private void OnEnable()
        {
            inputField.Select();
            inputField.ActivateInputField();
        }
    }
}
