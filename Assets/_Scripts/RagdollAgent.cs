using UnityEngine;
using UnityEngine.AI;

public class RagdollAgent : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private Rigidbody[] ragdollBodies;

    [Header("Ragdoll Settings")]
    [Tooltip("Use if the ragdoll needs special collisions, if set to 'Nothing', the layer won't change")]
    [SerializeField] private LayerMask ragdollLayer;

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

        bool shouldChangeLayer = ragdollLayer.value > 0;
        int targetLayer = Mathf.RoundToInt(Mathf.Log(ragdollLayer.value, 2));

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;

            if (shouldChangeLayer) rb.gameObject.layer = targetLayer;
        }

        Destroy(gameObject, 5);
    }
}
