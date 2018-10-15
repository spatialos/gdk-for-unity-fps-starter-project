using System.Collections.Generic;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Health;
using UnityEngine;

namespace Fps
{
    public class VisibilityAndCollision : MonoBehaviour
    {
        [Require] private HealthComponent.Requirable.Reader health;

        private bool isVisible = true;

        private CharacterController characterController;
        [SerializeField] private List<Renderer> renderersToIgnore = new List<Renderer>();

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            health.HealthUpdated += HealthUpdated;
            SetIsVisible(health.Data.Health > 0);
        }

        private void SetIsVisible(bool visible)
        {
            if (visible == isVisible)
            {
                return;
            }

            isVisible = visible;

            if (characterController)
            {
                characterController.enabled = visible;
            }

            foreach (var r in GetComponentsInChildren<Renderer>())
            {
                if (!renderersToIgnore.Contains(r))
                {
                    r.enabled = visible;
                }
            }
        }

        private void HealthUpdated(float newHealth)
        {
            SetIsVisible(newHealth > 0);
        }
    }
}
