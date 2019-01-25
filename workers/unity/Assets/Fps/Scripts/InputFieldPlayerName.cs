using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class InputFieldPlayerName : MonoBehaviour
{
    private InputField field;
    public Text HintText;

    public bool NameIsValid { get; private set; }

    public delegate void OnNameChangeDelegate(bool isValid);

    public OnNameChangeDelegate OnNameChanged;

    private void Awake()
    {
        field = GetComponent<InputField>();
        Validate(false);
        HintText.text = "";

        field.onValueChanged.AddListener(OnChanged);
        field.onEndEdit.AddListener(OnEnd);
    }

    private void OnChanged(string huh)
    {
        field.onValueChanged.RemoveListener(OnChanged); // Don't retrigger event as we modify the text...

        // TODO: This didn't work correctly. Need to account for field caret? Better to use onValidateInput?

        Validate(true);
        field.onValueChanged.AddListener(OnChanged);
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
            field.text = field.text.Trim();
        }

        if (field.text.Length == 0)
        {
            SendOnNameChanged(NameIsValid);
            return;
        }

        if (field.text.Length < 3)
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
