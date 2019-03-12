using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class PlayerNameInputController : MonoBehaviour
    {
        public delegate void OnNameChangeDelegate(bool isValid);

        public OnNameChangeDelegate OnNameChanged;

        public Text HintText;
        public bool AllowEdit;
        public bool NameIsValid { get; private set; }
        [NonSerialized] public bool DisplayEnterNameHint;

        public int CurrentNameLength => inputField.text.Length;

        public InputField inputField;

        private readonly Color HintTextColor = new Color(1f, .4f, .4f);

        private void Awake()
        {
            HintText.text = string.Empty;
            SendOnNameChanged(false);
            inputField.onValueChanged.AddListener(OnValueChanged);
            inputField.onEndEdit.AddListener(OnEnd);
            HintText.color = HintTextColor;
        }

        private void OnValueChanged(string value)
        {
            UpdateHintText();
            NameIsValid = value.Trim().Length >= 3;
            SendOnNameChanged(NameIsValid);
        }

        public void UpdateHintText()
        {
            var nameLength = inputField.text.Trim().Length;

            if (nameLength == 0)
            {
                HintText.text = DisplayEnterNameHint ? "You must enter a name to play" : string.Empty;
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

        private void OnEnable()
        {
            inputField.enabled = AllowEdit;

            HintText.gameObject.SetActive(AllowEdit);

            if (!AllowEdit)
            {
                if (inputField.text == string.Empty)
                {
                    inputField.text = "<name missing>";
                }

                return;
            }

            inputField.Select();
            inputField.ActivateInputField();
        }

        public string GetPlayerName()
        {
            return inputField.text;
        }

        private void OnDisable()
        {
            inputField.enabled = false;
            HintText.gameObject.SetActive(false);
        }

        private void OnEnd(string theString)
        {
            inputField.text = inputField.text.Trim();
        }

        private void SendOnNameChanged(bool value)
        {
            OnNameChanged?.Invoke(value);
        }
    }
}
