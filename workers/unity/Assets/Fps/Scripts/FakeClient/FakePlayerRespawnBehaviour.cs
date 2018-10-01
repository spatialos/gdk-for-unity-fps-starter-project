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
    }

    private async void OnDeath(DeathInfo info)
    {
        driver.Stop();
        driver.SetMovementSpeed(MovementSpeed.Run);
        await Task.Delay(TimeSpan.FromSeconds(5));
        Commands?.SendRequestRespawnRequest(GetComponent<SpatialOSComponent>().SpatialEntityId, new Empty());
    }
}
