using UnityEngine;
using UnityEngine.AI;

public enum PedestrianState { Patrol, Flee }

[RequireComponent(typeof(EnemyInput))]
public class PedestrianFSM : AgentFSM
{
    [Header("FSM Settings")]
    public PedestrianState currentState = PedestrianState.Patrol;

    [Header("Wander Settings")]
    public float wanderRadius = 15f;
    public float wanderTimer = 6f;
    private float timer;

    [Header("Flee Settings")]
    public float detectionRadius = 10f;
    public float patrolSpeed = 2f;
    public float fleeSpeed = 5f;
    public LayerMask zombieMask;

    private EnemyInput virtualInput;
    private Transform threatZombie;

    protected override void Awake()
    {
        base.Awake();
        virtualInput = GetComponent<EnemyInput>();
    }

    private void Start()
    {
        agent.speed = patrolSpeed;
        ChangeState(currentState);
    }

    protected override void Update()
    {
        if (isDeath) return;

        if (blackboard != null)
        {
            blackboard.SetNormalizedSpeed(agent.velocity.magnitude, agent.speed);
            virtualInput.LookDirection = transform.forward;
        }

        switch (currentState)
        {
            case PedestrianState.Patrol: HandlePatrol(); break;
            case PedestrianState.Flee: HandleFlee(); break;
        }

        base.Update();
    }

    public void ChangeState(PedestrianState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case PedestrianState.Patrol:
                agent.speed = patrolSpeed;
                timer = wanderTimer;
                break;
            case PedestrianState.Flee:
                agent.speed = fleeSpeed;
                break;
        }
    }

    private void HandlePatrol()
    {
        if (IsZombieNearby())
        {
            ChangeState(PedestrianState.Flee);
            return;
        }

        timer += Time.deltaTime;
        if (timer >= wanderTimer || (agent.remainingDistance < 0.5f && !agent.pathPending))
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    private void HandleFlee()
    {
        if (!IsZombieNearby())
        {
            ChangeState(PedestrianState.Patrol);
            return;
        }

        // Calcular dirección opuesta al peligro
        Vector3 fleeDirection = (transform.position - threatZombie.position).normalized;
        Vector3 targetDestination = transform.position + fleeDirection * 6f;

        if (NavMesh.SamplePosition(targetDestination, out NavMeshHit hit, 6f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private bool IsZombieNearby()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, zombieMask);
        if (hitColliders.Length > 0)
        {
            // Tomar el primer zombie detectado como amenaza
            threatZombie = hitColliders[0].transform;
            return true;
        }
        threatZombie = null;
        return false;
    }

    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist + origin;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, dist, layermask)) return navHit.position;
        return origin;
    }
}