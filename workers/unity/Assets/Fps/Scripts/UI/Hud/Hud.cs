using System.Collections;
using Improbable.Common;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.StandardTypes;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Fps
{
    public class Hud : MonoBehaviour
    {
        private GameObject postProcessObject;

        [SerializeField] private float hitIntensity = 1f;
        [SerializeField] private float hitEffectDuration = 1f;
        [SerializeField] private float hitEffectFocus = 16f;

        [Require] private HealthComponentReader healthReader;
        [Require] private GunStateComponentReader gunStateReader;

        private float currentFocus;
        private InGameUIController inGameUIController;
        private HealthBarController healthBarController;
        private LowHealthVignette damageVolumeSettings;

        private void Awake()
        {
            inGameUIController = ClientWorkerHandler.ScreenUIController.InGameController;
            healthBarController = inGameUIController.GetComponentInChildren<HealthBarController>(true);

            postProcessObject = GameObject.FindGameObjectWithTag("PostProcessing");
            if (postProcessObject != null)
            {
                var volume = postProcessObject.GetComponent<PostProcessVolume>();
                volume.profile.TryGetSettings(out damageVolumeSettings);
            }
        }

        private void OnEnable()
        {
            healthReader.OnHealthModifiedEvent += OnHealthModified;
            healthReader.OnRespawnEvent += OnRespawn;
            gunStateReader.OnIsAimingUpdate += AimingUpdated;
        }

        private void OnRespawn(Empty obj)
        {
            // Hide respawn screen
            inGameUIController.RespawnScreen.SetActive(false);
            inGameUIController.Hud.SetActive(true);
            SetHealthEffect(1);
        }

        private void OnHealthModified(HealthModifiedInfo healthModifiedInfo)
        {
            var healthModifier = healthModifiedInfo.Modifier;
            if (healthModifier.Amount < 0)
            {
                ShowTookDamageEffect(healthModifier.Origin.ToVector3());
            }

            if (healthModifiedInfo.Died)
            {
                // Show respawn screen on death
                inGameUIController.RespawnScreen.SetActive(true);
                inGameUIController.SetEscapeScreen(false);
                inGameUIController.Hud.SetActive(false);
            }

            var currentHealth = healthReader.Data.Health / healthReader.Data.MaxHealth;
            SetHealthEffect(currentHealth);
            SetHealthBar(currentHealth);
        }

        private void AimingUpdated(BlittableBool isAiming)
        {
            // Inform the ScreenUIController of the aiming update.
            inGameUIController.SetPlayerAiming(isAiming);
        }

        public void ShowTookDamageEffect(Vector3 origin)
        {
            var relativeYaw = GetRelativeYaw(origin, transform);
            damageVolumeSettings.damageYaw.value = relativeYaw;
            if (hitEffectFocus != currentFocus)
            {
                currentFocus = hitEffectFocus;
                damageVolumeSettings.damageFocus.value = currentFocus;
                damageVolumeSettings.damageIntensity.value = hitIntensity;
            }

            StopAllCoroutines();
            StartCoroutine(FadeEffect(damageVolumeSettings, hitIntensity, 0, hitEffectDuration));
        }

        private static float GetRelativeYaw(Vector3 origin, Transform transform)
        {
            var direction = (origin - transform.position).normalized;
            return Mathf.Rad2Deg * Mathf.Atan2(direction.x, direction.z) - transform.eulerAngles.y;
        }

        public void SetHealthBar(float healthFraction)
        {
            if (healthBarController != null && healthBarController.isActiveAndEnabled)
            {
                healthBarController.SetHealthBar(healthFraction);
            }
        }

        public void SetHealthEffect(float healthFraction)
        {
            damageVolumeSettings.health.value = healthFraction;
            SetHealthBar(healthFraction);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private static IEnumerator FadeEffect(LowHealthVignette damageVolumeSettings, float from, float to, float time)
        {
            if (time > 0)
            {
                for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
                {
                    damageVolumeSettings.damageIntensity.value = Mathf.Lerp(from, to, t);

                    yield return null;
                }
            }

            damageVolumeSettings.damageIntensity.value = to;
        }
    }
}
