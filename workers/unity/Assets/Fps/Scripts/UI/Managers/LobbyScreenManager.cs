using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class LobbyScreenManager : ConnectionStatusManager
    {
        public InputField InputField;
        public FpsUIButton StartButton;
        public FpsUIButton CancelButton;
        public Text HintText;


        private readonly Color HintTextColor = new Color(1f, .4f, .4f);

        public void UpdateHintText(bool hasGameBegun)
        {
            var nameLength = InputField.text.Trim().Length;

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
            return InputField.text;
        }

        public bool IsValidName()
        {
            return InputField.text.Length >= 3;
        }

        private void Awake()
        {
            HintText.text = string.Empty;
            HintText.color = HintTextColor;
        }

        private void OnEnable()
        {
            InputField.Select();
            InputField.ActivateInputField();
        }
    }
}
