using Fps.Guns;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Fps.Visibility
{
    public class FovManager : MonoBehaviour
    {
        [Require] private GunComponentReader gun;
        [Require] private GunStateComponentReader gunState;
        [Require] private HealthComponentReader health;

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
            gun.OnGunIdUpdate += OnGunChanged;
            gunState.OnIsAimingUpdate += OnAimingChanged;
            health.OnHealthModifiedEvent += CheckForDeathModifier;
            health.OnRespawnEvent += OnRespawn;
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

        private void OnAimingChanged(bool _)
        {
            RecalculateFov();
        }

        private void OnGunChanged(int _)
        {
            RecalculateFov();
        }

        private void RecalculateFov()
        {
            var currentGun = GunDictionary.Get(gun.Data.GunId);

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
