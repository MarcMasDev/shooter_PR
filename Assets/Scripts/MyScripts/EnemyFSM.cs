using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Patrol, Search, Chase, Attack }

[RequireComponent(typeof(EnemyInput), typeof(EnemySensors))]
public class EnemyFSM : AgentFSM
{
    [Header("FSM Settings")]
    public EnemyState currentState = EnemyState.Patrol;
    [SerializeField] private EnemyState onHurtState = EnemyState.Search;

    [Header("Wander Settings")]
    public float wanderRadius = 15f;
    public float wanderTimer = 5f;
    private float timer;

    [Header("Search Settings")]
    public float rotationSpeed = 120f;
    private float rotatedSum = 0f;


    [Header("Attack Settings")]
    public float attackRange = 10f;

    private float patrolSpeed = 1.5f;
    private float chaseSpeed = 3.5f;

    private EnemyInput virtualInput;
    private EnemySensors sensors;
    [SerializeField] private bool flying = false;
    public bool forceChase = false;
    private Transform target;

    protected override void Awake()
    {
        base.Awake();
        virtualInput = GetComponent<EnemyInput>();
        sensors = GetComponent<EnemySensors>();
    }
    private void Start()
    {
        EnterState(currentState);
    }

    protected override void Update()
    {
        if (isDeath) return;

        target = sensors.GetCurrentTarget();

        if (blackboard != null)
        {
            blackboard.SetNormalizedSpeed(agent.velocity.magnitude, agent.speed);
            virtualInput.LookDirection = transform.forward;
        }

        switch (currentState)
        {
            case EnemyState.Patrol: HandlePatrol(); break;
            case EnemyState.Search: HandleSearch(); break;
            case EnemyState.Chase: HandleChase(); break;
            case EnemyState.Attack: HandleAttack(); break;
        }

        base.Update();
        CheckAttackState();
    }
    private void CheckAttackState()
    {
        if (target == null) return;
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackRange) ChangeState(EnemyState.Attack);
    }

    private void EnterState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Patrol: OnPatrolEnter(); break;
            case EnemyState.Search: OnSearchEnter(); break;
            case EnemyState.Chase: OnChaseEnter(); break;
            case EnemyState.Attack: OnAttackEnter(); break;
        }
    }

    private void ExitState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Patrol: OnPatrolExit(); break;
            case EnemyState.Search: OnSearchExit(); break;
            case EnemyState.Chase: OnChaseExit(); break;
            case EnemyState.Attack: OnAttackExit(); break;
        }
    }

    //ENTERS (cuando el enemigo entra al extado x)
    private void OnPatrolEnter()
    {
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        timer = wanderTimer; //Forzar que busque un punto inmediatamente
    }
    private void OnSearchEnter()
    {
        blackboard.TriggerSearch(true);
        agent.isStopped = true;
        rotatedSum = 0f;
    }
    private void OnChaseEnter()
    {
        agent.ResetPath();
        agent.speed = chaseSpeed;
        agent.isStopped = false;
    }
    private void OnAttackEnter()
    {
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        virtualInput.IsAiming = true;
        blackboard.TriggerAttack(true);
    }

    //EXITS (cuando el enemigo sale del extado x)
    private void OnPatrolExit(){}
    private void OnSearchExit()
    {
        rotatedSum = 0f;
        blackboard.TriggerSearch(false);
    }
    private void OnChaseExit() {}
    private void OnAttackExit()
    {
        virtualInput.IsShooting = false;
        virtualInput.IsAiming = false;

        if (blackboard != null)
        {
            blackboard.m_IsPerformingAction = false; //reset input
            blackboard.TriggerAttack(false);
        }
    }

    //Handles (updates de los estados)
    private void HandlePatrol()
    {
        if (sensors.CanSeePlayer()) { ChangeState(EnemyState.Chase); return; }
        if (sensors.CanHearPlayer()) { ChangeState(EnemyState.Search); return; }

        timer += Time.deltaTime;

        if (timer >= wanderTimer || (agent.remainingDistance < 0.5f && !agent.pathPending))
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    private void HandleSearch()
    {
        if (sensors.CanSeePlayer()) { ChangeState(EnemyState.Chase); return; }

        float step = rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, step);
        rotatedSum += step;

        if (rotatedSum >= 360f) ChangeState(EnemyState.Patrol);
    }

    private void HandleChase()
    {
        if (target == null) { ChangeState(EnemyState.Search); return; }
        agent.SetDestination(target.position);

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackRange && sensors.CanSeePlayer()) ChangeState(EnemyState.Attack);
        else if (!sensors.CanSeePlayer() && !sensors.CanHearPlayer() && !forceChase) ChangeState(EnemyState.Search);
    }

    private void HandleAttack()
    {
        if (target == null) { ChangeState(EnemyState.Patrol); return; }

        //Smoothly rotate toward target
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

        virtualInput.IsShooting = true;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackRange+ 0.25f) ChangeState(EnemyState.Chase); //leave some margin
    }

    protected override void OnHurtBehavior(Vector2 health, Vector2 shield)
    {
        if (currentState != EnemyState.Chase && currentState != EnemyState.Attack) ChangeState(onHurtState);
    }
    protected override void OnDeathBehavior()
    {
        base.OnDeathBehavior();
        if (flying) GetComponent<Animator>().applyRootMotion = false;
    }

    //Función auxiliar para encontrar un punto válido en el NavMesh
    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * dist;
        randomDirection += origin;


        NavMeshHit navHit;
        //Busca el punto válido más cercano dentro de la distancia especificada
        if (NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask)) return navHit.position;

        //Si por algún motivo falla, se queda donde está
        return origin;
    }

    //El spawner llamará aquí nada más crear al zombie para decirle cómo de rápido es
    public void SetupSpeeds(float walkSpeed, float runSpeed)
    {
        patrolSpeed = walkSpeed;
        chaseSpeed = runSpeed;

        agent.speed = patrolSpeed;
    }

    public void ChangeState(EnemyState newState)
    {
        if (newState == currentState) return;

        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
    }


    private void OnDrawGizmosSelected()
    {
        //Rango de Ataque (Círculo Rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        //Línea hacia el destino actual del NavMesh
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, agent.destination);
        }

        //Cono de visión (si tienes acceso a la script de sensores)
        if (sensors != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, sensors.viewRadius);

            //Dibujar los límites del ángulo de visión
            Vector3 leftBoundary = Quaternion.Euler(0, -sensors.viewAngle / 2, 0) * transform.forward;
            Vector3 rightBoundary = Quaternion.Euler(0, sensors.viewAngle / 2, 0) * transform.forward;

            Gizmos.DrawRay(transform.position, leftBoundary * sensors.viewRadius);
            Gizmos.DrawRay(transform.position, rightBoundary * sensors.viewRadius);
        }
    }

}