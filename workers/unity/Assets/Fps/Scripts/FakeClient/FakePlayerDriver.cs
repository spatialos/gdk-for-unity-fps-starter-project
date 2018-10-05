using System.Collections;
using System.Runtime.Serialization;
using Improbable.Common;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

public class FakePlayerDriver : MonoBehaviour
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

    private ClientMovementDriver movementDriver;
    private ClientShooting shooting;
    private SpatialOSComponent spatial;
    private NavMeshAgent agent;

    private Vector3 anchorPoint;
    private MovementSpeed movementSpeed = MovementSpeed.Run;
    private const float MovementRadius = 200;
    private bool jumpNext;
    private bool sprintNext;

    private GameObject target;
    private float lastShotTime;
    private bool strafeRight;

    private void Start()
    {
        movementDriver = GetComponent<ClientMovementDriver>();
        shooting = GetComponent<ClientShooting>();
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        anchorPoint = transform.position;
    }

    private void OnEnable()
    {
        HealthReader.OnHealthModified += OnHealthModified;
        HealthReader.OnRespawn += OnRespawn;
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
        if (!agent.hasPath)
        {
            SetRandomDestination();
        }
        else
        {
            // set agent position to current position.
            agent.nextPosition = transform.position;

            var velocity = agent.desiredVelocity;
            if (velocity != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(velocity, Vector3.up);
                movementDriver.ApplyMovement(velocity, rotation, movementSpeed, jumpNext);
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
            var rotationAmount = Quaternion.RotateTowards(transform.rotation, targetRotation, 10f);

            GetComponent<ClientMovementDriver>().ApplyMovement(
                strafeRight ? transform.right : -transform.right, rotationAmount, MovementSpeed.Run, false);

            if (shooting.IsShooting(true) && Mathf.Abs(Quaternion.Angle(targetRotation, transform.rotation)) < 5)
            {
                shooting.FireShot(200f, new Ray(gunOrigin, transform.forward));
                shooting.InitiateCooldown(0.2f);
                lastShotTime = Time.time;
            }

            if (lastShotTime < Time.time - 10f)
            {
                Debug.LogFormat("{0} Giving up shooting {1}", name, target.name);
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
        Debug.LogFormat("{0} {1} -> {2}", transform.name, state, newState);
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
                    StartCoroutine(RandomlyJump());
                    StartCoroutine(RandomlySprint());
                    StartCoroutine(CheckForNearbyTargets());
                    break;
            }
        }
    }

    public void SetRandomDestination()
    {
        var destination = anchorPoint + Random.insideUnitSphere * MovementRadius;
        destination.z = anchorPoint.z;
        agent.isStopped = false;
        agent.nextPosition = transform.position;
        agent.SetDestination(destination);
    }

    private IEnumerator RequestRespawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            var spatial = GetComponent<SpatialOSComponent>();
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
                            SetPlayerState(PlayerState.ShootingTarget);
                            break;
                        }
                    }
                }
            }
        }
    }
}
