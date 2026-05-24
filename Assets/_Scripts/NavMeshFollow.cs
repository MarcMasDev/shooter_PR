using UnityEngine;
using UnityEngine.AI;

public class NavMeshFollow : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    [Header("Chase Settings")]
    public float stopDistance = 1.5f;


    private Animator animator;

    private int speedAnimatorSetter = 0;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        agent.stoppingDistance = stopDistance;
        animator.SetFloat("Velocity", GenerateRandomVelocity());
    }

    private void Update()
    {
        if (!agent.enabled) return;

        agent.SetDestination(player.position);

        bool stop = agent.remainingDistance <= stopDistance;
        agent.isStopped = stop;
        agent.updateRotation = !stop;
        animator.SetBool("Attack", stop);


        if (stop) FacePlayer();
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation,lookRotation, Time.deltaTime * 10f);
        }
    }

    private int GenerateRandomVelocity()
    {
        if (agent.speed <= 1)
        {
            return Random.Range(0, 3);
        }
        return Random.Range(3, 5);
    }
}
