using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class LobbyScreenManager : ConnectionStatusManager
    {
        public InputField inputField;
        public FpsUIButton startButton;
        public FpsUIButton cancelButton;
        public Text HintText;


        private readonly Color HintTextColor = new Color(1f, .4f, .4f);

        public void UpdateHintText(bool hasGameBegun)
        {
            var nameLength = inputField.text.Trim().Length;

            if (nameLength == 0 && !hasGameBegun)
            {
                HintText.text = "You must enter a name to play";
            }
            else if (nameLength < 3)
            {
                HintText.text = "Minimum 3 characters required";
            }
            else
            {
                HintText.text = string.Empty;
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
            HintText.text = string.Empty;
            HintText.color = HintTextColor;
        }

        private void OnEnable()
        {
            inputField.Select();
            inputField.ActivateInputField();
        }
    }
}
