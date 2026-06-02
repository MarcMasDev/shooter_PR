using System;
using UnityEngine;
using UnityEngine.AI;

public class RagdollAgent : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    [SerializeField] private int ragdollDestroyTime = 5;

    [Header("Death Layer Settings")]
    [Tooltip("If true, the parent and all children will change to the specified layer on death.")]
    [SerializeField] private bool changeLayerOnDeath = false;
    [SerializeField] private LayerMask deathLayer;

    public Action<Vector3> OnRagdollCleanup;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

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

        foreach (var rb in ragdollBodies) rb.isKinematic = false;

        if (changeLayerOnDeath)
        {
            int layerIndex = GetLayerIndexFromMask(deathLayer);

            foreach (var col in ragdollColliders) col.gameObject.layer = layerIndex;

            gameObject.layer = layerIndex;
        }

        Invoke(nameof(Cleanup), ragdollDestroyTime);
    }
    private void Cleanup()
    {
        //Saber dónde esta muriendo
        OnRagdollCleanup?.Invoke(transform.position);

        Destroy(gameObject);
    }

    /// <summary>
    /// Extrae el índice de capa de una LayerMask de Unity.
    /// </summary>
    private int GetLayerIndexFromMask(LayerMask mask)
    {
        int bitmask = mask.value;
        if (bitmask == 0) return 0; //layer 0 if empty

        for (int i = 0; i < 32; i++)
        {
            if ((bitmask & (1 << i)) != 0)
            {
                return i;
            }
        }
        return 0;
    }
}
