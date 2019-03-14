using Improbable.Gdk.Health;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Fps
{
    public class HealthBarHandler : MonoBehaviour
    {
        [Require] private HealthComponentReader healthReader;

        private ScreenUIController screenUIController;
        private HealthBarController healthBarController;

        private void Awake()
        {
            healthBarController = ClientWorkerHandler.ScreenUIController.GetComponentInChildren<HealthBarController>(true);
        }

        private void OnEnable()
        {
            healthReader.OnHealthUpdate += OnHealthUpdate;
        }

        private void OnHealthUpdate(float health)
        {
            var fraction = health / healthReader.Data.MaxHealth;
            SetHealthBar(fraction);
        }

        public void SetHealthBar(float healthFraction)
        {
            if (healthBarController != null && healthBarController.isActiveAndEnabled)
            {
                healthBarController.SetHealthBar(healthFraction);
            }
        }
    }
}
