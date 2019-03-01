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

        public int RedSeconds = 60;
        public int TestSeconds;

        public bool CountUp;
        public int TestStartTime = 330;

        private int time;

        private void OnValidate()
        {
            if (!gameObject.scene.isLoaded)
            {
                return;
            }

            TestSeconds = Mathf.Clamp(TestSeconds, 0, 100 * 60 - 1);
            UpdateTime(TestSeconds);
        }

        private void Start()
        {
            time = TestStartTime;
        }

        private void Update()
        {
            if (CountUp)
            {
                RedSeconds = -1;
                UpdateTime((int) Time.time);
            }
            else
            {
                UpdateTime(Mathf.Max(0, (int) (TestStartTime - Time.time)));
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
    }
}
