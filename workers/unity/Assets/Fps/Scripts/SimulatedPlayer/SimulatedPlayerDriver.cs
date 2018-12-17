using System.Collections;
using Improbable.Common;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class SimulatedPlayerDriver : MonoBehaviour, MyMovementUtils.IMovementStateRestorer
{
    public enum PlayerState
    {
        Unknown,
        Dead,
        LookingForTarget,
        ShootingTarget
    }

    private PlayerState state = PlayerState.Unknown;

    [Require] private HealthComponent.Requirable.Reader HealthReader;
    [Require] private HealthComponent.Requirable.CommandRequestSender HealthCommands;

    private MyClientMovementDriver movementDriver;
    private ClientShooting shooting;
    private SpatialOSComponent spatial;
    private NavMeshAgent agent;
    private SimulatedPlayerCoordinatorWorkerConnector coordinator;
    private CommandFrameSystem commandFrame;
    private CharacterController controller;

    private readonly JumpMovement jumpMovement = new JumpMovement();
    private readonly MyMovementUtils.SprintCooldown sprintCooldown = new MyMovementUtils.SprintCooldown();
    private readonly MyMovementUtils.RemoveWorkerOrigin removeOrigin = new MyMovementUtils.RemoveWorkerOrigin();

    private Vector3 anchorPoint;
    private const float MovementRadius = 50f;
    private const float NavMeshSnapDistance = 5f;
    private const float MinRemainingDistance = 0.3f;
    private const float MaxAgentDeviation = 1f;
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
        controller = GetComponent<CharacterController>();
        movementDriver = GetComponent<MyClientMovementDriver>();
        movementDriver.SetMovementProcessors(new MyMovementUtils.IMovementProcessor[]
        {
            new StandardMovement(),
            sprintCooldown,
            jumpMovement,
            new MyMovementUtils.Gravity(),
            new MyMovementUtils.TerminalVelocity(),
            new MyMovementUtils.CharacterControllerMovement(controller),
            removeOrigin,
            new IsGroundedMovement(),
            new MyMovementUtils.AdjustVelocity(),
        });
        movementDriver.SetStateRestorer(this);
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
        HealthReader.OnHealthModified += OnHealthModified;
        HealthReader.OnRespawn += OnRespawn;
        spatial = GetComponent<SpatialOSComponent>();
        commandFrame = spatial.World.GetExistingManager<CommandFrameSystem>();

        removeOrigin.Origin = spatial.Worker.Origin;

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
        agent.nextPosition = transform.position;

        var agentDeviation = (agent.nextPosition - transform.position).magnitude;

        if (agent.remainingDistance < MinRemainingDistance || agent.pathStatus == NavMeshPathStatus.PathInvalid || !agent.hasPath)
        {
            Debug.Log($"{name} Setting Random Destination");
            SetRandomDestination();
        }
        else if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            if (agentDeviation > MaxAgentDeviation)
            {
                agent.Warp(transform.position);
            }

            var desiredVelocity = agent.desiredVelocity;
            if (desiredVelocity == Vector3.zero)
            {
                SetRandomDestination();
            }
            else
            {
                var rot = transform.rotation;
                var flatVelocity = agent.desiredVelocity;
                flatVelocity.y = 0;
                var desiredRotation = Quaternion.LookRotation(flatVelocity, Vector3.up);

                rot = Quaternion.RotateTowards(rot, desiredRotation, 360 * Time.deltaTime);
                transform.rotation = rot;

                var diff = Mathf.Abs(rot.eulerAngles.y - desiredRotation.eulerAngles.y);

                // prob navmesh in front to try and stay on it.
                var potentialNewPosition = transform.position + transform.forward
                    * MyMovementUtils.GetMovmentSpeedVelocity(MovementSpeed.Sprint) * Time.deltaTime;

                movementDriver.AddInput(
                    forward: (diff < 30),
                    jump: jumpNext,
                    sprint: sprintNext,
                    yaw: transform.rotation.eulerAngles.y,
                    pitch: transform.rotation.eulerAngles.x);

                jumpNext = false;
            }
        }
    }

    private void Update_ShootingTarget()
    {
        if (TargetIsValid())
        {
            var fudgeFactor = Random.insideUnitSphere * 2f;
            var gunOrigin = transform.position + Vector3.up;
            var targetCenter = target.transform.position + Vector3.up + fudgeFactor;

            var targetRotation = Quaternion.LookRotation(targetCenter - gunOrigin);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);

            var destination = transform.position + (strafeRight ? transform.right : -transform.right) *
                MyMovementUtils.GetMovmentSpeedVelocity(MovementSpeed.Run) * Time.deltaTime;
            var canStrafe = NavMesh.SamplePosition(destination, out var hit, 0.25f, NavMesh.AllAreas);

            movementDriver.AddInput(yaw: transform.rotation.eulerAngles.y, pitch: transform.rotation.eulerAngles.x,
                right: (canStrafe && strafeRight), left: (canStrafe && !strafeRight));

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

    private bool TargetIsValid()
    {
        if (target == null)
        {
            return false;
        }

        var targetSpatial = target.GetComponent<SpatialOSComponent>();
        var entityManager = targetSpatial.World.GetExistingManager<EntityManager>();
        var entity = targetSpatial.Entity;
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
                HealthCommands.SendRequestRespawnRequest(spatial.SpatialEntityId, new Empty());
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
                if (nearbyCollider.GetComponent<SpatialOSComponent>() != null)
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
                    Debug.LogWarning($"{name} stuck at {transform.position} (agent: {agent.nextPosition}) in state {state}");
                    Debug.DrawLine(transform.position, transform.position + Vector3.up * 100, Color.red, 200);
                    Debug.DrawLine(transform.position + Vector3.up * 100,
                        agent.nextPosition + Vector3.up * 100, Color.yellow, 200);
                    Debug.DrawLine(agent.nextPosition, agent.nextPosition + Vector3.up * 100, Color.yellow, 200);

                    agent.isStopped = true;
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

    public void Restore(MovementState movementState)
    {
        controller.transform.position = movementState.Position.ToVector3() + spatial.Worker.Origin;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        var style = new GUIStyle
        {
            alignment = TextAnchor.MiddleCenter,
            fixedWidth = 400,
            fixedHeight = 300,
            wordWrap = true,
            normal = { textColor = Color.white }
        };

        Handles.Label(transform.position + new Vector3(0, 3, 0),
            $"Fr: {(CommandFrameSystem.FrameLength * 1000f)}," +
            $"OnNavmesh: {agent.isOnNavMesh}," +
            $"PlayerState: {state}," +
            $"Path Status: {agent.pathStatus}," +
            $"Target location: {agent.destination}," +
            $"Desired Velocity: {agent.desiredVelocity}," +
            $"{(commandFrame.IsPaused() ? "NET PAUSED" : "")}", style);
    }
#endif
}
