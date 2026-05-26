using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(CharacterBlackboard))]
public class AgentFSM : MonoBehaviour
{
    protected NavMeshAgent agent;
    [SerializeField] protected CharacterBlackboard blackboard;
    protected bool isDeath = false;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Update()
    {
        if (isDeath) return;
        HandleOffMeshLinks();
    }

    protected void HandleOffMeshLinks()
    {
        if (agent.isOnOffMeshLink)
        {
            if (blackboard != null) blackboard.TriggerJump();
            agent.CompleteOffMeshLink();
        }
    }
    public void TriggerAgentFootstep() => blackboard.TriggerFootstep();

    protected virtual void OnEnable()
    {
        if (blackboard != null)
        {
            blackboard.OnDeath += OnDeathBehavior;
            blackboard.OnHurt += OnHurtBehavior;
        }
    }

    protected virtual void OnDisable()
    {
        if (blackboard != null)
        {
            blackboard.OnDeath -= OnDeathBehavior;
            blackboard.OnHurt -= OnHurtBehavior;
        }
    }

    protected virtual void OnDeathBehavior()
    {
        isDeath = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        enabled = false;
    }
    protected virtual void OnHurtBehavior(Vector2 health, Vector2 shield) {}
}
