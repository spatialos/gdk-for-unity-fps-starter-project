using System.Collections.Generic;
using Improbable.Gdk.Health;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Fps
{
    public class VisibilityAndCollision : MonoBehaviour
    {
        [Require] private HealthComponentReader health;

        private bool isVisible = true;

        private CharacterController characterController;
        [SerializeField] private List<Renderer> renderersToIgnore = new List<Renderer>();

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            health.OnHealthUpdate += HealthUpdated;
            UpdateVisibility();
        }

        private void HealthUpdated(float newHealth)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (health == null)
            {
                return;
            }

            var visible = (health.Data.Health > 0);

            if (visible == isVisible)
            {
                return;
            }

            isVisible = visible;

            if (characterController)
            {
                characterController.enabled = visible;
            }

            foreach (var childRenderer in GetComponentsInChildren<Renderer>())
            {
                if (!renderersToIgnore.Contains(childRenderer))
                {
                    childRenderer.enabled = visible;
                }
            }
        }
    }
}
