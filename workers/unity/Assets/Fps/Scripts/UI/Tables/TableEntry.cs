using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class TableEntry : MonoBehaviour
    {
        public Image Background;

        [SerializeField] private Text[] textVisuals;

        private void OnValidate()
        {
            if (Background == null)
            {
                throw new MissingReferenceException("Missing reference to the background image for this table entry.");
            }

            textVisuals = GetComponentsInChildren<Text>();

            if (textVisuals == null)
            {
                throw new MissingReferenceException("Missing reference to the text components for this table entry.");
            }
        }

        public void SetAllTextVisuals(Color color, bool bold)
        {
            foreach (var text in textVisuals)
            {
                text.color = color;
                text.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
            }
        }
    }
}
