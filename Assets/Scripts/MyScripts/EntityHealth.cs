using UnityEngine;
public enum ImpactResult
{
    alreadyDeath,
    death,
    impact
}
public struct Impact
{
    public ImpactResult impactResult;
    public bool givePoints;

    public Impact(ImpactResult impactR, bool giveP) : this()
    {
        impactResult = impactR;
        givePoints = giveP;
    }
}
public interface IDamageable
{
    Impact TakeDamage(float amount, GameObject deathFX = null);
}
public class EntityHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private CharacterBlackboard m_StateBlackboard;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxShield = 100f;
    [SerializeField] private float startShield = 100f;
    [SerializeField] [Range(0,1)] private float shieldProtection = 0.75f;
    [SerializeField] private bool givePoints = false;


    private RagdollAgent ragdoll;
    private float currentHealth;
    private float currentShield;

    private void Start()
    {
        startShield = Mathf.Min(maxShield, startShield);
        ragdoll = GetComponent<RagdollAgent>();

        Heal(maxHealth, startShield); //Updates UI and sets init values
    }

    public Impact TakeDamage(float amount, GameObject deathFX = null)
    {
        if (currentHealth <= 0) return new Impact(ImpactResult.alreadyDeath, false);

        float damageToShield = amount * shieldProtection;
        float damageToHealth = amount - damageToShield;

        //SHIELD
        if (currentShield > 0)
        {
            if (currentShield >= damageToShield) currentShield -= damageToShield;
            else //overflow
            {
                float overflow = damageToShield - currentShield;
                currentShield = 0;
                damageToHealth += overflow;
            }
        } //---
        else damageToHealth = amount;

        currentHealth -= damageToHealth;

        if (m_StateBlackboard != null) m_StateBlackboard.TriggerHurt(new Vector2(currentHealth,maxHealth), new Vector2(currentShield, maxShield));

        if (currentHealth <= 0)
        {
            if (deathFX != null) Instantiate(deathFX, transform.position, Quaternion.identity);
            Die();
            return new Impact(ImpactResult.death, givePoints);
        }
        return new Impact(ImpactResult.impact, givePoints);
    }

    public void Heal(float healthAmount, float shieldAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healthAmount, maxHealth);
        currentShield = Mathf.Min(currentShield + shieldAmount, maxShield);

        if (m_StateBlackboard != null) m_StateBlackboard.TriggerHeal(new Vector2(currentHealth, maxHealth), new Vector2(currentShield, maxShield));
    }
    private void Die()
    {
        if (m_StateBlackboard != null) m_StateBlackboard.TriggerDeath();

        if (ragdoll != null) ragdoll.EnableRagdoll();
    }
    public Vector2 GetCurrentAndMaxHealth() => new Vector2(currentHealth, maxHealth);
    public Vector2 GetCurrentAndMaxShield() => new Vector2(currentShield, maxShield);
}
