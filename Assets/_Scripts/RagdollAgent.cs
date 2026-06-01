using System;
using UnityEngine;
using UnityEngine.AI;

public class RagdollAgent : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private Rigidbody[] ragdollBodies;
    [SerializeField] private int ragdollDestroyTime = 5;
    public Action<Vector3> OnRagdollCleanup;
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

        Invoke(nameof(Cleanup), ragdollDestroyTime);
    }
    private void Cleanup()
    {
        //Saber dónde esta muriendo
        OnRagdollCleanup?.Invoke(transform.position);

        Destroy(gameObject);
    }
}
