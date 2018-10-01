using System;
using System.Threading.Tasks;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Health;
using Improbable.Gdk.Movement;
using UnityEngine;

public class FakePlayerRunAwayBehaviour : MonoBehaviour
{
    private FakePlayerDriver driver;

    [Require] private HealthComponent.Requirable.Reader Health;

    private void Start()
    {
        driver = GetComponent<FakePlayerDriver>();
    }

    private void OnEnable()
    {
        Health.OnHealthModified += OnHealthModified;
    }

    private async void OnHealthModified(HealthModifier modifier)
    {
        if (modifier.Amount < 0)
        {
            driver.SetRandomDestination();
            driver.SetMovementSpeed(MovementSpeed.Sprint);
            await Task.Delay(TimeSpan.FromSeconds(10));
            driver.SetMovementSpeed(MovementSpeed.Run);
        }
    }
}
