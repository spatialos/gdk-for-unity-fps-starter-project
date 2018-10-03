using System;
using System.Threading.Tasks;
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
        driver.SetMovementSpeed(MovementSpeed.Run);
        driver.SetRandomDestination();
    }

    private async void OnDeath(DeathInfo info)
    {
        driver.Stop();
        await Task.Delay(TimeSpan.FromSeconds(5));
        Commands?.SendRequestRespawnRequest(GetComponent<SpatialOSComponent>().SpatialEntityId, new Empty());
    }
}
