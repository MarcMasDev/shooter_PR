using UnityEngine;

public class RagdollExplosion : MonoBehaviour
{
    [SerializeField] private float blastRadius = 5f;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float upwardsModifier = 1f;
    [SerializeField] private LayerMask targetLayers;
    public void TriggerExplosion()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, blastRadius, targetLayers);

        foreach (Collider hit in colliders)
        {
            
            if (hit.TryGetComponent<Rigidbody>(out var rb))
            {
                Animator anim = hit.GetComponentInParent<Animator>();
                if (anim != null && anim.enabled) anim.enabled = false;

                rb.AddExplosionForce(explosionForce, explosionPos, blastRadius, upwardsModifier, ForceMode.Impulse);
            }
        }
    }
}
