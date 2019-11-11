using UnityEngine;
using UnityEngine.UI;

namespace Fps.UI
{
    public class GameUITimer : MonoBehaviour
    {
        [SerializeField] private Text minutes;
        [SerializeField] private Text seconds;
        [SerializeField] private Text divider;

        [SerializeField] private Color defaultColor;
        [SerializeField] private Color lowTimeColor;
        [SerializeField] private int lowSecondsThreshold;

        private uint maxTime;

        private void OnValidate()
        {
            if (minutes == null)
            {
                throw new MissingReferenceException("Missing reference to the minutes text.");
            }

            if (seconds == null)
            {
                throw new MissingReferenceException("Missing reference to the seconds text.");
            }

            if (divider == null)
            {
                throw new MissingReferenceException("Missing reference to the divider text.");
            }
        }

        private void UpdateTime(int updatedSeconds)
        {
            var col = updatedSeconds <= lowSecondsThreshold ? lowTimeColor : defaultColor;

            minutes.text = (updatedSeconds / 60).ToString("D2");
            seconds.text = (updatedSeconds % 60).ToString("D2");

            minutes.color = col;
            seconds.color = col;
            divider.color = col;
        }

        public void SynchronizeTime(uint seconds)
        {
            UpdateTime((int) (maxTime - seconds));
        }

        public void SetMaxTime(uint seconds)
        {
            maxTime = seconds;
        }
    }
}
