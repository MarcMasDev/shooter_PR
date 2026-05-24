using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private float damage = 1000;
    [SerializeField] private float timeBetweenDamage = 1;
    [SerializeField] private bool damageOnEntry = true;
    [SerializeField] private bool damageOnlyPlayer = false;
    private float time = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            time = timeBetweenDamage;

            if ((damageOnlyPlayer && !other.CompareTag("Player")) || !damageOnEntry) return;

            damageable.TakeDamage(damage);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            time = timeBetweenDamage; 
            if (other.TryGetComponent(out IDamageable damageable))
            {
                if (damageOnlyPlayer && other.CompareTag("Player")) damageable.TakeDamage(damage);
                else if (!damageOnlyPlayer) damageable.TakeDamage(damage);
            }
        }

    }
}
