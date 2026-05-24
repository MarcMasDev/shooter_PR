using UnityEngine;
using UnityEngine.AI;

public class RagdollAgent : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private Rigidbody[] ragdollBodies;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();

        DisableRagdoll();
    }

    void DisableRagdoll()
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = true;
        }
    }

    public void EnableRagdoll()
    {
        animator.enabled = false;
        agent.enabled = false;

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
        }

        Destroy(gameObject, 5);
    }
}
