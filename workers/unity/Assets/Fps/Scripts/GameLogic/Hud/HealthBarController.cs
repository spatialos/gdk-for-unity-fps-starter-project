using UnityEngine;

namespace Fps
{
    public class HealthBarController : MonoBehaviour
    {
        public RectTransform HealthFill;
        private float fullHealthWidth;

        // Use this for initialization
        private void Awake()
        {
            fullHealthWidth = HealthFill.rect.width;
        }

        public void SetHealthBar(float healthFraction)
        {
            HealthFill.sizeDelta = new Vector2(fullHealthWidth * healthFraction, HealthFill.rect.height);
        }
    }
}
