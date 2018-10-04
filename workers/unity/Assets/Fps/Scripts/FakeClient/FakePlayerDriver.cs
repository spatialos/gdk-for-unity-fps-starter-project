using System.Collections;
using Improbable.Gdk.Movement;
using UnityEngine;
using UnityEngine.AI;

public class FakePlayerDriver : MonoBehaviour
{
    private ClientMovementDriver movementDriver;
    private NavMeshAgent agent;

    private Vector3 anchorPoint;
    private MovementSpeed movementSpeed;
    private const float MovementRadius = 200;
    private bool jumpNext = false;

    private void Start()
    {
        movementDriver = GetComponent<ClientMovementDriver>();
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        anchorPoint = transform.position;

        StartCoroutine(nameof(RandomlyJump));
        StartCoroutine(nameof(RandomlySprint));
    }

    private IEnumerator RandomlyJump()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5, 25));
            jumpNext = true;
        }
    }

    private IEnumerator RandomlySprint()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5, 10));
            if (movementSpeed == MovementSpeed.Run)
            {
                SetMovementSpeed(MovementSpeed.Sprint);
                yield return new WaitForSeconds(Random.Range(2, 6));
                if (movementSpeed == MovementSpeed.Sprint)
                {
                    SetMovementSpeed(MovementSpeed.Run);
                }
            }
        }
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
                // set agent position to current position;
                agent.nextPosition = transform.position;

                var vel = agent.desiredVelocity;
                if (vel != Vector3.zero)
                {
                    var rotation = Quaternion.LookRotation(vel, Vector3.up);
                    movementDriver.ApplyMovement(vel, rotation, movementSpeed, jumpNext);
                    jumpNext = false;
                }
            }
        }
    }

    public void SetMovementSpeed(MovementSpeed speed)
    {
        movementSpeed = speed;
    }

    public void SetRandomDestination()
    {
        var destination = anchorPoint + Random.insideUnitSphere * MovementRadius;
        destination.z = anchorPoint.z;
        agent.isStopped = false;
        agent.nextPosition = transform.position;
        agent.SetDestination(destination);
    }

    public void Stop()
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }
}
