using Improbable.Gdk.Movement;
using UnityEngine;
using UnityEngine.AI;

public class FakePlayerDriver : MonoBehaviour
{
    private ClientMovementDriver movementDriver;
    private NavMeshAgent agent;

    private Vector3 anchorPoint;
    private const float MovementRadius = 100;

    private void Start()
    {
        movementDriver = GetComponent<ClientMovementDriver>();
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        anchorPoint = transform.position;
    }

    private void Update()
    {
        if (movementDriver.enabled)
        {
            if (!agent.hasPath)
            {
                SetRandomDestination();
            }
            else
            {
                var vel = agent.desiredVelocity;
                if (vel != Vector3.zero)
                {
                    var rotation = Quaternion.LookRotation(vel, Vector3.up);
                    movementDriver.ApplyMovement(vel, rotation, MovementSpeed.Run, false);
                }

                // set agent position to current position;
                agent.nextPosition = transform.position;
            }
        }
    }

    private void SetRandomDestination()
    {
        var destination = anchorPoint + Random.insideUnitSphere * MovementRadius;
        destination.z = anchorPoint.z;
        agent.SetDestination(destination);
    }
}
