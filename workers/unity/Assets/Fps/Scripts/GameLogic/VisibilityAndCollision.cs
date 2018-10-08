using System.Collections.Generic;
using Improbable.Common;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Health;
using UnityEngine;

namespace Fps
{
    public class VisibilityAndCollision : MonoBehaviour
    {
        [Require] private HealthComponent.Requirable.Reader health;

        private bool isDead = false;

        private CharacterController characterController;
        [SerializeField] private List<Renderer> renderersToIgnore = new List<Renderer>();

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            health.OnRespawn += OnRespawn;
            health.OnHealthModified += OnHealthModified;
            ApplyState();
            SetIsDeadState(health.Data.Health <= 0);
        }

        private void Start()
        {
            ApplyState();
        }

        private void OnRespawn(Empty empty)
        {
            SetIsDeadState(false);
        }

        private void OnHealthModified(HealthModifiedInfo info)
        {
            if (info.Died)
            {
                SetIsDeadState(true);
            }
        }

        private void SetIsDeadState(bool newDeadState)
        {
            if (isDead != newDeadState)
            {
                isDead = newDeadState;
                ApplyState();
            }
        }

        private void ApplyState()
        {
            SetRenderersEnabled(transform, !isDead);
        }

        private void SetRenderersEnabled(Transform root, bool enabled)
        {
            // Enable/Disable the character controller (to turn on/off collision)
            if (characterController)
            {
                characterController.enabled = enabled;
            }

            // Enable/Disable the renderers to turn on/off the model's visibility.
            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                if (!renderersToIgnore.Contains(renderer))
                {
                    renderer.enabled = enabled;
                }
            }
        }
    }
}
