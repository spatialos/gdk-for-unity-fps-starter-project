using System;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Health;
using Improbable.Gdk.Safezone;
using Improbable.Worker;
using UnityEngine;

namespace Fps
{
    public class ZoneExitDetector : MonoBehaviour
    {
        [Require] private HealthComponent.Requirable.CommandRequestSender CommandSender;
        [Require] private HealthComponent.Requirable.Writer DenotesAuthority;

        private EntityId playerEntityId;
        public GameObject SafeZone = null;
        private long nextDamage;

        private void OnEnable()
        {
            playerEntityId = gameObject.GetComponent<SpatialOSComponent>().SpatialEntityId;

            float minDist = Mathf.Infinity;
            Vector3 currentPos = transform.position;
            foreach (ZoneShrinker zoneShrinker in FindObjectsOfType<ZoneShrinker>())
            {
                float dist = Vector3.Distance(zoneShrinker.gameObject.transform.position, currentPos);
                if (dist < minDist)
                {
                    SafeZone = zoneShrinker.gameObject;
                    minDist = dist;
                }
            }

            nextDamage = DateTime.Now.Ticks;
        }

        private void OnDisable()
        {
            SafeZone = null;
        }

        private void Update()
        {
            if (DateTime.Now.Ticks > nextDamage)
            {
                if (SafeZone == null ||
                    Vector3.Distance(transform.position, SafeZone.transform.position) > SafeZone.transform.localScale.x)
                {
                    ApplyDamage();
                }
            }
        }

        private void ApplyDamage()
        {
            nextDamage = DateTime.Now.AddSeconds(ZoneSettings.DamageInterval).Ticks;
            CommandSender.SendModifyHealthRequest(playerEntityId, new HealthModifier()
            {
                Amount = -1 * ZoneSettings.Damage
            });
        }
    }
}
