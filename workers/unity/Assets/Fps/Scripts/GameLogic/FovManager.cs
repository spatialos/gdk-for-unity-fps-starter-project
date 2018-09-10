using Improbable.Common;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using UnityEngine;

namespace Fps
{
    public class FovManager : MonoBehaviour
    {
        [Require] private GunComponent.Requirable.Reader gun;
        [Require] private GunStateComponent.Requirable.Reader gunState;
        [Require] private HealthComponent.Requirable.Reader health;

        private new Camera camera;
        private float defaultFov;
        private float CurrentTargetFov;
        private float timeLeftToChange;
        private bool currentlyChanging;

        private void Awake()
        {
            camera = GetComponentInChildren<Camera>();
            defaultFov = camera.fieldOfView;
        }

        private void OnEnable()
        {
            gun.CurrentSlotUpdated += OnGunChanged;
            gunState.IsAimingUpdated += OnAimingChanged;
            health.OnHealthModified += CheckForDeathModifier;
            health.OnRespawn += OnRespawn;
            RecalculateFov();
        }

        private void OnRespawn(Empty _)
        {
            RecalculateFov();
        }

        private void CheckForDeathModifier(HealthModifiedInfo info)
        {
            if (info.Died)
            {
                RecalculateFov();
            }
        }

        private void OnAimingChanged(BlittableBool _)
        {
            RecalculateFov();
        }

        private void OnGunChanged(int _)
        {
            RecalculateFov();
        }

        private void RecalculateFov()
        {
            var currentGun = GunDictionary.GetCurrentGun(gun.Data);

            var newFov = health.Data.Health > 0 && gunState.Data.IsAiming ? currentGun.AimFov : defaultFov;

            if (newFov != CurrentTargetFov)
            {
                CurrentTargetFov = newFov;
                if (currentlyChanging)
                {
                    timeLeftToChange = currentGun.FovChangeTime - timeLeftToChange;
                }
                else
                {
                    timeLeftToChange = currentGun.FovChangeTime;
                }

                currentlyChanging = true;
            }
        }

        private void Update()
        {
            if (currentlyChanging)
            {
                if (timeLeftToChange <= Time.deltaTime)
                {
                    camera.fieldOfView = CurrentTargetFov;
                    currentlyChanging = false;
                }
                else
                {
                    var fovChangeGradient = (CurrentTargetFov - camera.fieldOfView) / timeLeftToChange;
                    camera.fieldOfView = Mathf.MoveTowards(camera.fieldOfView, CurrentTargetFov,
                        Mathf.Abs(fovChangeGradient) * Time.deltaTime);
                    timeLeftToChange -= Time.deltaTime;
                }
            }
        }
    }
}
