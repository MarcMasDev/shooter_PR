using UnityEngine;
public interface IDamageable
{
    void TakeDamage(float amount);
}
public class EntityHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private CharacterBlackboard m_StateBlackboard;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxShield = 100f;
    [SerializeField] private float startShield = 100f;
    [SerializeField] [Range(0,1)] private float shieldProtection = 0.75f;
    private float currentHealth;
   private float currentShield;

    private void Start()
    {
        startShield = Mathf.Min(maxShield, startShield);

        Heal(maxHealth, startShield); //Updates UI and sets init values
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

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
            Die();
        }
    }

    public void Heal(float healthAmount, float shieldAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healthAmount, maxHealth);
        currentShield = Mathf.Min(currentShield + shieldAmount, maxShield);

        if (m_StateBlackboard != null) m_StateBlackboard.TriggerHeal(new Vector2(currentHealth, maxHealth), new Vector2(currentShield, maxShield));
    }
    private void Die()
    {
        m_StateBlackboard.TriggerDeath();
    }
    public Vector2 GetCurrentAndMaxHealth() => new Vector2(currentHealth, maxHealth);
    public Vector2 GetCurrentAndMaxShield() => new Vector2(currentShield, maxShield);
}
