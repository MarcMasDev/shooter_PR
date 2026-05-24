using UnityEngine;
using UnityEngine.Audio;

public class ItemHealth : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactString = "Restore: ";
    [SerializeField] private float healAmount = 25f;
    [SerializeField] private float shieldAmount = 25f;
    [SerializeField] private AudioSource audioSource;
    public bool Interact(GameObject user)
    {
        EntityHealth health = user.GetComponent<EntityHealth>();
        if (health == null) return false;

        float startHealth = healAmount;
        float startShield = shieldAmount;

        if (healAmount > 0) ApplyHealth(health);
        if (shieldAmount > 0) ApplyShield(health);

        if (startHealth != healAmount ||  startShield != shieldAmount) if (audioSource != null) audioSource.Play();

        bool consumed = healAmount <= 0 && shieldAmount <= 0;
        if (consumed) Destroy(gameObject);

        return consumed;
    }

    private void ApplyHealth(EntityHealth health)
    {
        Vector2 healthData = health.GetCurrentAndMaxHealth();
        float currentHealth = healthData.x;
        float maxHealth = healthData.y;

        // Cuanto necesitamos? max - current
        float neededHealth = maxHealth - currentHealth;

        if (neededHealth > 0)
        {
            // Pilla el valor más bajo: lo que tiene el item vs lo que necesita el jugador
            int amountToGive = Mathf.RoundToInt(Mathf.Min(healAmount, neededHealth));

            health.Heal(amountToGive, 0); // Aplicar al jugador
            healAmount -= amountToGive;   // Restar del item
        }
    }

    private void ApplyShield(EntityHealth health)
    {
        Vector2 shieldData = health.GetCurrentAndMaxShield();
        float currentShield = shieldData.x;
        float maxShield = shieldData.y;

        // Cuanto necesitamos? max - current (Corregido: antes tenías x - y)
        float neededShield = maxShield - currentShield;

        if (neededShield > 0)
        {
            // Pilla el valor más bajo: lo que tiene el item vs lo que necesita el jugador
            int amountToGive = Mathf.RoundToInt(Mathf.Min(shieldAmount, neededShield));

            health.Heal(0, amountToGive); // Aplicar al jugador
            shieldAmount -= amountToGive; // Restar del item
        }
    }

    public string GetInteractionText()
    {
        string text = interactString;
        if (healAmount > 0)
        {
            text += healAmount + " health ";
        }
        if (shieldAmount > 0)
        {
            text += shieldAmount + " shield ";
        }
        return text;
    }
}
