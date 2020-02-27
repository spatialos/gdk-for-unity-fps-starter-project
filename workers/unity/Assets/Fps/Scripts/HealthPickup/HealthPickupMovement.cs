using System;
using Fps.Config;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Fps.HealthPickup
{
    [WorkerType(WorkerUtils.UnityClient)]
    public class HealthPickupMovement : MonoBehaviour
    {
        private float hoverPeriod;
        private float timeSinceStart;
        private float initialHeight;

        public float HoverTimePeriodSecs = 3.0f;
        public float RotationSpeedDegreesPerSec = 60.0f;
        public float HoverRange = 0.25f;

        private void Start()
        {
            hoverPeriod = 2 * (float) Math.PI / HoverTimePeriodSecs;
            initialHeight = transform.position.y;
        }

        private void Update()
        {
            var currentRotation = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.AngleAxis(currentRotation + RotationSpeedDegreesPerSec
                * Time.deltaTime, Vector3.up);

            timeSinceStart += Time.deltaTime;

            var position = transform.position;
            position.y = HoverRange / 2 * Mathf.Sin(timeSinceStart * hoverPeriod) + initialHeight;
            transform.position = position;
        }
    }
}
