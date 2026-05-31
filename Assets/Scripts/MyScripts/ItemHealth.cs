using UnityEngine;
using UnityEngine.Audio;

public class ItemHealth : PurchasableInteractable
{
    [Header("Health Settings")]
    [SerializeField] private float healAmount = 25f;
    [SerializeField] private float shieldAmount = 25f;
    [SerializeField] private AudioSource audioSource;

    protected override bool ExecuteInteraction(GameObject user)
    {
        EntityHealth health = user.GetComponent<EntityHealth>();
        if (health == null) return false;

        float startHealth = healAmount;
        float startShield = shieldAmount;

        if (healAmount > 0) ApplyHealth(health);
        if (shieldAmount > 0) ApplyShield(health);

        if (startHealth != healAmount || startShield != shieldAmount)
        {
            if (audioSource != null) audioSource.Play();
        }

        bool consumed = healAmount <= 0 && shieldAmount <= 0;
        if (consumed) Destroy(gameObject);

        return consumed;
    }

    private void ApplyHealth(EntityHealth health)
    {
        Vector2 healthData = health.GetCurrentAndMaxHealth();
        float neededHealth = healthData.y - healthData.x;

        if (neededHealth > 0)
        {
            int amountToGive = Mathf.RoundToInt(Mathf.Min(healAmount, neededHealth));
            health.Heal(amountToGive, 0);
            healAmount -= amountToGive;
        }
    }

    private void ApplyShield(EntityHealth health)
    {
        Vector2 shieldData = health.GetCurrentAndMaxShield();
        float neededShield = shieldData.y - shieldData.x;

        if (neededShield > 0)
        {
            int amountToGive = Mathf.RoundToInt(Mathf.Min(shieldAmount, neededShield));
            health.Heal(0, amountToGive);
            shieldAmount -= amountToGive;
        }
    }

    public override string GetInteractionText()
    {
        string extraInfo = "";
        if (healAmount > 0) extraInfo += $"{healAmount} HP ";
        if (shieldAmount > 0) extraInfo += $"{shieldAmount} Shield";

        if (Cost > 0)
        {
            return $"{BaseInteractString} {extraInfo} [Cost: {Cost}]";
        }
        return $"{BaseInteractString} {extraInfo}";
    }
}
