using System.Collections;
using Fps;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Fps.Guns;
using Improbable.Gdk.Health;
using Fps.Movement;
using UnityEngine;
using UnityEngine.AI;

public class SimulatedPlayerDriver : MonoBehaviour
{
    public enum PlayerState
    {
        Unknown,
        Dead,
        LookingForTarget,
        ShootingTarget
    }

    private PlayerState state = PlayerState.Unknown;

    [Require] private HealthComponentReader HealthReader;
    [Require] private HealthComponentCommandSender HealthCommands;
    [Require] private EntityId entityId;

    private ClientMovementDriver movementDriver;
    private ClientShooting shooting;
    private LinkedEntityComponent spatial;
    private NavMeshAgent agent;
    private SimulatedPlayerCoordinatorWorkerConnector coordinator;

    private Vector3 anchorPoint;
    private const float MovementRadius = 50f;
    private const float NavMeshSnapDistance = 5f;
    private const float MinRemainingDistance = 0.3f;
    private bool jumpNext;
    private bool sprintNext;

    private int similarPositionsCount = 0;
    private Vector3 lastPosition;

    private GameObject target;
    private float lastShotTime;
    private bool strafeRight;

    private Bounds worldBounds;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        movementDriver = GetComponent<ClientMovementDriver>();
        shooting = GetComponent<ClientShooting>();
        coordinator = FindObjectOfType<SimulatedPlayerCoordinatorWorkerConnector>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.Warp(transform.position);
        anchorPoint = transform.position;
        worldBounds = coordinator.GetWorldBounds();
    }

    private void OnEnable()
    {
        HealthReader.OnHealthModifiedEvent += OnHealthModified;
        HealthReader.OnRespawnEvent += OnRespawn;
        spatial = GetComponent<LinkedEntityComponent>();
        SetPlayerState(PlayerState.LookingForTarget);
    }

    private void OnHealthModified(HealthModifiedInfo info)
    {
        if (info.Died)
        {
            SetPlayerState(PlayerState.Dead);
        }
    }

    private void OnRespawn(Empty empty)
    {
        if (state == PlayerState.Dead)
        {
            anchorPoint = transform.position;
            agent.Warp(transform.position);
            SetPlayerState(PlayerState.LookingForTarget);
        }
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
            sprintNext = true;
            yield return new WaitForSeconds(Random.Range(2, 6));
            sprintNext = false;
        }
    }

    private IEnumerator RandomlyStrafe()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, 3));
            strafeRight = !strafeRight;
        }
    }

    private void Update()
    {
        if (movementDriver.enabled)
        {
            switch (state)
            {
                case PlayerState.Dead:
                    // Do Nothing.
                    break;
                case PlayerState.LookingForTarget:
                    Update_LookingForTarget();
                    break;
                case PlayerState.ShootingTarget:
                    Update_ShootingTarget();
                    break;
            }
        }
    }

    private void Update_LookingForTarget()
    {
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out var hit, 0.5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
        else if (agent.remainingDistance < MinRemainingDistance || agent.pathStatus == NavMeshPathStatus.PathInvalid ||
            !agent.hasPath)
        {
            SetRandomDestination();
        }
        else if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            var velocity = agent.desiredVelocity;
            velocity.y = 0;
            if (velocity != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(velocity, Vector3.up);
                var speed = sprintNext ? MovementSpeed.Sprint : MovementSpeed.Run;
                MoveTowards(transform.position + velocity, speed, rotation, jumpNext);
                jumpNext = false;
            }
        }

        agent.nextPosition = transform.position;
    }

    private void Update_ShootingTarget()
    {
        if (TargetIsValid())
        {
            var fudgeFactor = Random.insideUnitSphere * 2f;
            var gunOrigin = transform.position + Vector3.up;
            var targetCenter = target.transform.position + Vector3.up + fudgeFactor;

            var targetRotation = Quaternion.LookRotation(targetCenter - gunOrigin);
            var rotationAmount = Quaternion.RotateTowards(transform.rotation, targetRotation, 10f);

            var destination = transform.position + (strafeRight ? transform.right : -transform.right);

            MoveTowards(destination, MovementSpeed.Run, rotationAmount, false);

            if (shooting.IsShooting(true) && Mathf.Abs(Quaternion.Angle(targetRotation, transform.rotation)) < 5)
            {
                shooting.FireShot(200f, new Ray(gunOrigin, transform.forward));
                shooting.InitiateCooldown(0.2f);
                lastShotTime = Time.time;
            }

            if (lastShotTime < Time.time - 10f)
            {
                target = null;
                SetPlayerState(PlayerState.LookingForTarget);
            }
        }
        else
        {
            target = null;
            SetPlayerState(PlayerState.LookingForTarget);
        }
    }

    private void MoveTowards(Vector3 destination, MovementSpeed speed, Quaternion rotation, bool jump = false)
    {
        var agentPosition = agent.nextPosition;
        var direction = (destination - agentPosition).normalized;
        var desiredVelocity = direction * movementDriver.GetSpeed(speed);

        // Setting nextPosition will move the agent towards destination, but is constrained by the navmesh
        agent.nextPosition += desiredVelocity * Time.deltaTime;

        // Getting nextPosition here will return the constrained position of the agent
        var actualDirection = (agent.nextPosition - agentPosition).normalized;

        movementDriver.ApplyMovement(actualDirection, rotation, speed, jump);
    }

    private bool TargetIsValid()
    {
        if (target == null)
        {
            return false;
        }

        var linkedComponent = target.GetComponent<LinkedEntityComponent>();
        var entityManager = linkedComponent.World.EntityManager;
        linkedComponent.Worker.TryGetEntity(linkedComponent.EntityId, out var entity);
        if (entityManager.Exists(entity) && entityManager.HasComponent<HealthComponent.Component>(entity))
        {
            var targetHealth = entityManager.GetComponentData<HealthComponent.Component>(entity);
            if (targetHealth.Health == 0)
            {
                return false;
            }
        }

        return true;
    }

    public void SetPlayerState(PlayerState newState)
    {
        if (state != newState)
        {
            StopAllCoroutines();
            state = newState;
            switch (newState)
            {
                case PlayerState.Dead:
                    StartCoroutine(RequestRespawn());
                    break;
                case PlayerState.ShootingTarget:
                    lastShotTime = Time.time;
                    StartCoroutine(RandomlyStrafe());
                    break;
                case PlayerState.LookingForTarget:
                    agent.isStopped = false;
                    StartCoroutine(RandomlyJump());
                    StartCoroutine(RandomlySprint());
                    StartCoroutine(CheckForNearbyTargets());
                    StartCoroutine(CheckImmobility());
                    break;
            }
        }
    }

    public void SetRandomDestination()
    {
        var destination = anchorPoint + Random.insideUnitSphere * MovementRadius;
        destination.y = anchorPoint.y;
        if (NavMesh.SamplePosition(destination, out var hit, NavMeshSnapDistance, NavMesh.AllAreas))
        {
            if (worldBounds.Contains(hit.position))
            {
                agent.isStopped = false;
                agent.nextPosition = transform.position;
                agent.SetDestination(hit.position);
            }
        }
    }

    private IEnumerator RequestRespawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            if (HealthCommands != null && spatial != null)
            {
                HealthCommands.SendRequestRespawnCommand(entityId, new Empty());
            }
        }
    }

    private IEnumerator CheckForNearbyTargets()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            var gunPosition = transform.position + Vector3.up;
            foreach (var nearbyCollider in Physics.OverlapSphere(transform.position, 50f))
            {
                if (nearbyCollider.GetComponent<LinkedEntityComponent>() != null)
                {
                    if (Physics.Linecast(gunPosition, nearbyCollider.transform.position + Vector3.up,
                        out var hitInfo))
                    {
                        if (hitInfo.transform.root == nearbyCollider.transform.root)
                        {
                            target = nearbyCollider.gameObject;
                            agent.isStopped = true;
                            SetPlayerState(PlayerState.ShootingTarget);
                            break;
                        }
                    }
                }
            }
        }
    }

    private IEnumerator CheckImmobility()
    {
        const float maxDistance = 0.1f;
        const int maxSimilarPositions = 5;
        while (true)
        {
            yield return new WaitForSeconds(3);
            if (Vector3.Distance(lastPosition, transform.position) < maxDistance)
            {
                similarPositionsCount++;
                if (similarPositionsCount >= maxSimilarPositions)
                {
                    Debug.LogWarningFormat("{0} got stuck at {1}: agent at {2}, respawning",
                        name, transform.position, agent.nextPosition);

                    SetPlayerState(PlayerState.Dead);
                }
            }
            else
            {
                lastPosition = transform.position;
                similarPositionsCount = 0;
            }
        }
    }
}
