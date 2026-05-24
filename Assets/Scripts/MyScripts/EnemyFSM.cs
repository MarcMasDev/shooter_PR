using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Patrol, Search, Chase, Attack }

[RequireComponent(typeof(NavMeshAgent), typeof(EnemyInput), typeof(EnemySensors))]
public class EnemyFSM : MonoBehaviour
{
    [Header("FSM Settings")]
    public EnemyState currentState = EnemyState.Patrol;
    [SerializeField] private EnemyState onHurtState = EnemyState.Search;
    public float attackRange = 10f;

    [Header("Search Settings")]
    public float rotationSpeed = 120f;
    private float rotatedSum = 0f;

    [Header("Dissolve Settings")]
    [SerializeField] private float dissolveDuration = 4f;
    [SerializeField] private float dissolveDelay = 4f;
    private static readonly int DissolveProperty = Shader.PropertyToID("_DissolveAmount");
    private Renderer[] characterRenderers;

    private NavMeshAgent agent;
    private EnemyInput virtualInput;
    private EnemySensors sensors;
    [SerializeField] private CharacterBlackboard blackboard;
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private bool isDeath = false;
    [SerializeField] private bool flying = false;
    public bool forceChase = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        virtualInput = GetComponent<EnemyInput>();
        sensors = GetComponent<EnemySensors>();
        characterRenderers = GetComponentsInChildren<Renderer>();
    }


    private void Start()
    {
        EnterState(currentState);
    }

    private void Update()
    {
        if (isDeath) return;
        if (blackboard != null)
            blackboard.SetNormalizedSpeed(agent.velocity.magnitude, agent.speed);

        virtualInput.LookDirection = transform.forward;

        // Ejecutar lógica continua del estado
        switch (currentState)
        {
            case EnemyState.Patrol: HandlePatrol(); break;
            case EnemyState.Search: HandleSearch(); break;
            case EnemyState.Chase: HandleChase(); break;
            case EnemyState.Attack: HandleAttack(); break;
        }

        HandleOffMeshLinks();
    }

    public void ChangeState(EnemyState newState)
    {
        if (newState == currentState) return;

        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
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
        if (waypoints.Length > 0) agent.SetDestination(waypoints[currentWaypointIndex].position);
    }
    private void OnPatrolExit()
    {

    }
    private void OnSearchEnter()
    {
        blackboard.TriggerSearch(true);
        agent.isStopped = true;
        rotatedSum = 0f;
    }
    private void OnSearchExit()
    {
        rotatedSum = 0f;
        blackboard.TriggerSearch(false);
    }
    private void OnChaseEnter()
    {
        agent.ResetPath();
        agent.isStopped = false;
    }

    //EXITS (cuando el enemigo sale del extado x)
    private void OnChaseExit()
    {

    }
    private void OnAttackEnter()
    {
        agent.isStopped = true;
        virtualInput.IsAiming = true;
    }
    private void OnAttackExit()
    {
        virtualInput.IsShooting = false;
        virtualInput.IsAiming = false;
    }

    //Handles (updates de los estados)
    private void HandlePatrol()
    {
        if (sensors.CanSeePlayer()) { ChangeState(EnemyState.Chase); return; }
        if (sensors.CanHearPlayer()) { ChangeState(EnemyState.Search); return; }

        if (agent.remainingDistance < 0.5f && !agent.pathPending && waypoints.Length > 0)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
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
        Transform target = sensors.GetPlayerTransform();
        if (target == null) 
        {
            if (forceChase)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.transform;
            }
            else
            {
                ChangeState(EnemyState.Search);
                return;
            }
        }

        agent.SetDestination(target.position);

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackRange && sensors.CanSeePlayer()) ChangeState(EnemyState.Attack);
        else if (!sensors.CanSeePlayer() && !sensors.CanHearPlayer() && !forceChase) ChangeState(EnemyState.Search);
    }

    private void HandleAttack()
    {
        Transform target = sensors.GetPlayerTransform();
        if (target == null) { ChangeState(EnemyState.Search); return; }

        //Mirar al objetivo
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

        virtualInput.IsShooting = true;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackRange || !sensors.CanSeePlayer()) ChangeState(EnemyState.Chase);
    }

 
    private void HandleOffMeshLinks()
    {
        if (agent.isOnOffMeshLink)
        {
            if (blackboard != null) blackboard.TriggerJump();
            agent.CompleteOffMeshLink();
        }
    }

    public void TriggerEnemyFootstep() => blackboard.TriggerFootstep(); 

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

        //Visualización de los Waypoints de patrulla
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;

                //Esfera en el waypoint
                Gizmos.DrawSphere(waypoints[i].position, 0.3f);

                //Línea entre waypoints para ver la ruta
                int nextIndex = (i + 1) % waypoints.Length;
                if (waypoints[nextIndex] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
                }
            }
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

    //EVENTOS
    private void OnEnable() 
    {
        if (blackboard != null)
        {
            blackboard.OnHurt += OnHurtBehavior;
            blackboard.OnDeath += OnDeathBehavior;
        }
    }
    private void OnDisable()
    {
        if (blackboard != null)
        {
            blackboard.OnHurt -= OnHurtBehavior;
            blackboard.OnDeath -= OnDeathBehavior;
        }
    }
    private void OnHurtBehavior(Vector2 health, Vector2 shield)
    {
        if (currentState != EnemyState.Chase && currentState != EnemyState.Attack)
            ChangeState(onHurtState);
    }
    private void OnDeathBehavior()
    {
        isDeath = true;

        //Detener física
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        //solo para que puedan caer los enemigos voladores, si no, sobreescribe la física
        if (flying) GetComponent<Animator>().applyRootMotion = false;

        //Desactivar Colision
        if (TryGetComponent<Collider>(out var col)) col.enabled = false;


        //Ejecutar la desaparición
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        //En HDRP, si el material no tenía el clipping activo, esto lo fuerza
        foreach (Renderer r in characterRenderers)
        {
            if (r != null)
            {
                r.material.EnableKeyword("_ALPHATEST_ON");
            }
        }

        float elapsedTime = 0;

        yield return new WaitForSeconds(dissolveDelay); //Esperamos a que haya pasado un tiempo (animación de morir, cuerpo en el suelo
        while (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpTarget = Mathf.Clamp01(elapsedTime / dissolveDuration);

            foreach (Renderer r in characterRenderers)
            {
                if (r != null)
                {
                    //Al usar .material (y no .sharedMaterial) creamos una instancia única del material para cada enemigo.
                    r.material.SetFloat(DissolveProperty, lerpTarget);
                }
            }

            yield return null;
        }

        //Finalmente destruimos el objeto
        Destroy(gameObject);
    }
}