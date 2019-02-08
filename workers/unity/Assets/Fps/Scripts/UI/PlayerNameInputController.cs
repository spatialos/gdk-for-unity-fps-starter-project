using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class PlayerNameInputController : MonoBehaviour
    {
        public Text HintText;

        public bool AllowEdit;

        public delegate void OnNameChangeDelegate(bool isValid);

        public OnNameChangeDelegate OnNameChanged;

        public bool NameIsValid { get; private set; }

        private InputField inputField;

        private void Awake()
        {
            inputField = GetComponentInChildren<InputField>();
            Validate(false);
            HintText.text = "";
            inputField.onEndEdit.AddListener(OnEnd);
        }

        private void OnEnable()
        {
            inputField.enabled = AllowEdit;
            HintText.gameObject.SetActive(AllowEdit);

            if (!AllowEdit && inputField.text == "")
            {
                inputField.text = "<player name>  ";
            }
        }

        private void OnDisable()
        {
            inputField.enabled = false;
            HintText.gameObject.SetActive(false);
        }

        private void OnEnd(string theString)
        {
            Validate(false);
        }

        private void Validate(bool areEditing)
        {
            NameIsValid = true;
            HintText.text = "";

            if (!areEditing)
            {
                inputField.text = inputField.text.Trim();
            }

            if (inputField.text.Length == 0)
            {
                NameIsValid = false;
            }

            if (inputField.text.Length < 3)
            {
                if (!areEditing)
                {
                    HintText.text = "Minimum 3 characters required";
                }

                NameIsValid = false;
            }

            SendOnNameChanged(NameIsValid);
        }

        private void SendOnNameChanged(bool value)
        {
            OnNameChanged?.Invoke(value);
        }
    }
}
