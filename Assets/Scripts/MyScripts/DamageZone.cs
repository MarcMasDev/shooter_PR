using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private float damage = 1000;
    [SerializeField] private float timeBetweenDamage = 1;
    [SerializeField] private bool damageOnEntry = true;
    [SerializeField] private bool addPoints = false;
    [SerializeField] private float damageOnSpeed = 0;
    [SerializeField] private GameObject damageVFX;

    [SerializeField] private LayerMask targetLayers;

    private Rigidbody rb;

    private float time = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (!IsInLayerMask(other.gameObject.layer, targetLayers)) return;
        if (!PassesSpeedCheck(other)) return;
        if (!damageOnEntry) return;
        DealDamage(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!IsInLayerMask(other.gameObject.layer, targetLayers)) return;
        if (!PassesSpeedCheck(other)) return;

        time -= Time.deltaTime;
        if (time < 0) DealDamage(other);
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    private bool PassesSpeedCheck(Collider other)
    {
        if (damageOnSpeed <= 0) return true;
        if (rb == null) rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            float currentSpeed = rb.linearVelocity.magnitude;
            return currentSpeed >= damageOnSpeed;
        }
        return false;
    }

    private void DealDamage(Collider other)
    {
        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            time = timeBetweenDamage;
            ImpactResult result = damageable.TakeDamage(damage, damageVFX);

            if (addPoints && result != ImpactResult.alreadyDeath) ScoreManager.Instance.AddPoints(HitboxType.Body, result == ImpactResult.death);
        }
    }
}
