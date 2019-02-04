using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class PlayerNameInputController : MonoBehaviour
    {
        public Text HintText;

        public bool AllowEdit;

        public delegate void OnNameChangeDelegate(bool isValid);

        public bool NameIsValid { get; private set; }
        public OnNameChangeDelegate OnNameChanged;
        private InputField inputField;

        private void Awake()
        {
            inputField = GetComponentInChildren<InputField>();
            Validate(false);
            HintText.text = "";
            inputField.onValueChanged.AddListener(OnChanged);
            inputField.onEndEdit.AddListener(OnEnd);
        }

        private void OnEnable()
        {
            inputField.enabled = AllowEdit;
            HintText.gameObject.SetActive(AllowEdit);
        }

        private void OnDisable()
        {
            inputField.enabled = false;
            HintText.gameObject.SetActive(false);
        }

        private void OnChanged(string huh)
        {
            inputField.onValueChanged.RemoveListener(OnChanged); // Don't retrigger event as we modify the text...

            // TODO: This didn't work correctly. Need to account for field caret? Better to use onValidateInput?

            Validate(true);
            inputField.onValueChanged.AddListener(OnChanged);
        }

        private void OnEnd(string huh)
        {
            Validate(false);
        }

        private void Validate(bool areEditing)
        {
            NameIsValid = false;

            if (!areEditing)
            {
                inputField.text = inputField.text.Trim();
            }

            if (inputField.text.Length == 0)
            {
                SendOnNameChanged(NameIsValid);
                return;
            }

            if (inputField.text.Length < 3)
            {
                if (!areEditing)
                {
                    HintText.text = "Minimum 3 characters required";
                }

                SendOnNameChanged(NameIsValid);
                return;
            }

            HintText.text = "";

            NameIsValid = true;
            SendOnNameChanged(true);
        }

        private void SendOnNameChanged(bool value)
        {
            if (OnNameChanged == null)
            {
                return;
            }

            OnNameChanged(value);
        }
    }
}
