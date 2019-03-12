using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class GameUITimer : MonoBehaviour
    {
        public Text Minutes;
        public Text Seconds;
        public Text Divider;

        public Color DefaultColor;
        public Color LowTimeColor;

        private int RedSeconds = 60;

        public bool CountUp;

        private float time;

        private uint maxTime;

        private void Update()
        {
            time -= Time.deltaTime;
            if (CountUp)
            {
                RedSeconds = -1;
                UpdateTime(Mathf.RoundToInt(time));
            }
            else
            {
                UpdateTime(Mathf.Max(0, (int) (maxTime - time)));
            }
        }

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
            time = seconds;
        }

        public void SetMaxTime(uint seconds)
        {
            maxTime = seconds;
        }
    }
}
