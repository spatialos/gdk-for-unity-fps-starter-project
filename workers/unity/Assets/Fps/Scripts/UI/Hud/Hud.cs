using System.Collections;
using Fps.SchemaExtensions;
using Fps.UI;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Fps.UI
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
        private InGameScreenManager inGameScreenManager;
        private HealthBarController healthBarController;
        private LowHealthVignette damageVolumeSettings;

        private void Awake()
        {
            var uiManager = GameObject.FindGameObjectWithTag("OnScreenUI");
            if (uiManager == null)
            {
                throw new MissingReferenceException("Missing reference to the UI manager.");
            }

            inGameScreenManager = uiManager.GetComponentInChildren<InGameScreenManager>(true);
            if (inGameScreenManager == null)
            {
                throw new MissingReferenceException("Missing reference to the in game screen manager.");
            }

            healthBarController = inGameScreenManager.GetComponentInChildren<HealthBarController>(true);


            postProcessObject = GameObject.FindGameObjectWithTag("PostProcessing");
            if (postProcessObject == null)
            {
                throw new MissingReferenceException("Missing reference to the post process object.");
            }

            var volume = postProcessObject.GetComponent<PostProcessVolume>();
            volume.profile.TryGetSettings(out damageVolumeSettings);
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
            inGameScreenManager.RespawnScreen.SetActive(false);
            inGameScreenManager.Hud.SetActive(true);
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
                inGameScreenManager.RespawnScreen.SetActive(true);
                inGameScreenManager.SetEscapeScreen(false);
                inGameScreenManager.Hud.SetActive(false);
            }

            var currentHealth = healthReader.Data.Health / healthReader.Data.MaxHealth;
            SetHealthEffect(currentHealth);
            SetHealthBar(currentHealth);
        }

        private void AimingUpdated(bool isAiming)
        {
            // Inform the ScreenUIController of the aiming update.
            inGameScreenManager.SetPlayerAiming(isAiming);
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
