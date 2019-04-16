using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class GameUITimer : MonoBehaviour
    {
        [SerializeField] private Text minutes;
        [SerializeField] private Text seconds;
        [SerializeField] private Text divider;

        [SerializeField] private Color defaultColor;
        [SerializeField] private Color lowTimeColor;
        [SerializeField] private int redSeconds;

        private uint maxTime;

        private void UpdateTime(int seconds)
        {
            var col = seconds <= redSeconds ? lowTimeColor : defaultColor;

            minutes.text = (seconds / 60).ToString("D2");
            this.seconds.text = (seconds % 60).ToString("D2");

            minutes.color = col;
            this.seconds.color = col;
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
