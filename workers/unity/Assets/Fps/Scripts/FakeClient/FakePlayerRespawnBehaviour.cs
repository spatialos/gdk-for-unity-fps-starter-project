using System.Collections;
using Improbable.Common;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Health;
using Improbable.Gdk.Movement;
using UnityEngine;

public class FakePlayerRespawnBehaviour : MonoBehaviour
{
    private FakePlayerDriver driver;

    [Require] private HealthComponent.Requirable.Reader Health;
    [Require] private HealthComponent.Requirable.CommandRequestSender Commands;

    private void Start()
    {
        driver = GetComponent<FakePlayerDriver>();
    }

    private void OnEnable()
    {
        Health.OnDeath += OnDeath;
        Health.OnRespawn += OnRespawn;
    }

    private void OnRespawn(Empty empty)
    {
        StopCoroutine(TryRespawn());
        driver.SetMovementSpeed(MovementSpeed.Run);
        driver.SetRandomDestination();
    }

    private void OnDeath(DeathInfo info)
    {
        driver.Stop();
        StartCoroutine(TryRespawn());
    }

    private IEnumerator TryRespawn()
    {
        yield return new WaitForSeconds(5);
        Commands?.SendRequestRespawnRequest(GetComponent<SpatialOSComponent>().SpatialEntityId, new Empty());
    }
}
