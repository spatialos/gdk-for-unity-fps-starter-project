using System.Collections;
using Improbable.Common;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using Unity.Entities;
using UnityEngine;

public class FakePlayerShootingBehaviour : MonoBehaviour
{
    private FakePlayerDriver driver;
    private ClientShooting shooting;
    private SpatialOSComponent spatial;

    private GameObject target;
    private bool strafeRight = true;

    [Require] private HealthComponent.Requirable.Reader Health;

    private void OnEnable()
    {
        Health.OnHealthModified += OnHealthModified;
        Health.OnDeath += OnDeath;
        Health.OnRespawn += OnRespawn;
        driver = GetComponent<FakePlayerDriver>();
        shooting = GetComponent<ClientShooting>();
        spatial = GetComponent<SpatialOSComponent>();

        StartCoroutine(CheckForNearbyTargets());
        StartCoroutine(CheckTargetLineOfSight());
    }

    private void OnDisable()
    {
        target = null;
        StopAllCoroutines();
    }

    private IEnumerator CheckTargetLineOfSight()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            if (target != null)
            {
                var gunPosition = transform.position + Vector3.up;
                var targetPosition = target.transform.position + Vector3.up;
                if (Vector3.Distance(gunPosition, targetPosition) > 50f)
                {
                    target = null;
                    driver.SetRandomDestination();
                }
                else if (Physics.Linecast(gunPosition, targetPosition, out var hitInfo))
                {
                    if (hitInfo.transform.root != target.transform.root)
                    {
                        target = null;
                        driver.SetRandomDestination();
                    }
                }
                else
                {
                    target = null;
                    driver.SetRandomDestination();
                }
            }
        }
    }

    private IEnumerator CheckForNearbyTargets()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            if (target == null)
            {
                var gunPosition = transform.position + Vector3.up;
                foreach (var nearbyCollider in Physics.OverlapSphere(transform.position, 50f))
                {
                    if (nearbyCollider.GetComponent<SpatialOSComponent>() != null)
                    {
                        if (Physics.Linecast(gunPosition, nearbyCollider.transform.position + Vector3.up,
                            out var hitInfo))
                        {
                            if (hitInfo.transform.root == nearbyCollider.transform.root)
                            {
                                target = nearbyCollider.gameObject;
                                driver.Stop();
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnDeath(DeathInfo info)
    {
        target = null;
        StopAllCoroutines();
    }

    private void OnRespawn(Empty empty)
    {
        StartCoroutine(CheckForNearbyTargets());
        StartCoroutine(CheckTargetLineOfSight());
    }

    private void OnHealthModified(HealthModifier modifier)
    {
        if (Health.Data.Health == 0)
        {
            // We are dead.
            target = null;
            return;
        }

        var shotOrigin = modifier.Origin.ToVector3() + spatial.Worker.Origin;
        var gunPosition = transform.position + new Vector3(0, 1f, 0);
        if (Physics.Linecast(gunPosition, shotOrigin, out var hit))
        {
            var enemySpatial = hit.transform.root.GetComponent<SpatialOSComponent>();
            if (enemySpatial != null)
            {
                target = enemySpatial.gameObject;
                driver.Stop();
            }
        }
    }

    private void Update()
    {
        CheckTargetIsValid();
        if (target != null && shooting.enabled)
        {
            var fudgeFactor = Random.insideUnitSphere * 2f;
            var gunOrigin = transform.position + Vector3.up;
            var targetCenter = target.transform.position + Vector3.up + fudgeFactor;

            var targetRotation = Quaternion.LookRotation(targetCenter - gunOrigin);
            var rotationAmount = Quaternion.RotateTowards(transform.rotation, targetRotation, 10f);

            GetComponent<ClientMovementDriver>().ApplyMovement(
                strafeRight ? transform.right : -transform.right, rotationAmount, MovementSpeed.Walk, false);

            if (shooting.IsShooting(true) && Mathf.Abs(Quaternion.Angle(targetRotation, transform.rotation)) < 5)
            {
                shooting.FireShot(200f, new Ray(gunOrigin, transform.forward));
                shooting.InitiateCooldown(0.2f);
            }

            if (Random.value < 0.001f)
            {
                strafeRight = !strafeRight;
            }
        }
    }

    private void CheckTargetIsValid()
    {
        if (target != null)
        {
            var targetSpatial = target.GetComponent<SpatialOSComponent>();
            var entityManager = targetSpatial.World.GetExistingManager<EntityManager>();
            var entity = targetSpatial.Entity;
            if (entityManager.Exists(entity) && entityManager.HasComponent<HealthComponent.Component>(entity))
            {
                var targetHealth = entityManager.GetComponentData<HealthComponent.Component>(entity);
                if (targetHealth.Health == 0)
                {
                    if (driver.enabled)
                    {
                        driver.SetRandomDestination();
                    }

                    target = null;
                }
            }
        }
    }
}
