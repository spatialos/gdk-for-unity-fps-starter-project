using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class GameUITimer : MonoBehaviour
    {
        [SerializeField] private Text Minutes;
        [SerializeField] private Text Seconds;
        [SerializeField] private Text Divider;

        [SerializeField] private Color DefaultColor;
        [SerializeField] private Color LowTimeColor;
        [SerializeField] private int RedSeconds;

        private uint maxTime;

        private void UpdateTime(int seconds)
        {
            var col = seconds <= RedSeconds ? LowTimeColor : DefaultColor;

            Minutes.text = (seconds / 60).ToString("D2");
            Seconds.text = (seconds % 60).ToString("D2");

            Minutes.color = col;
            Seconds.color = col;
            Divider.color = col;
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
