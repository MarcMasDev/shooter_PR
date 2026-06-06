using UnityEngine;
using UnityEngine.Audio;

public class ItemAmmo : PurchasableInteractable
{
    [SerializeField] private int clips = 1;
    [SerializeField] private AudioSource audioSource;

    protected override bool ExecuteInteraction(GameObject user)
    {
        WeaponInventory inventory = user.GetComponent<WeaponInventory>();
        if (inventory != null)
        {
            Weapon currentWeapon = inventory.GetActiveWeapon();

            if (currentWeapon.GetCurrentAmmo().x == -1) return false; //el arma no usa balas

            currentWeapon.AddAmmo(currentWeapon.GetClipSize() * clips);

            if (audioSource != null) audioSource.Play();
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    public override string GetInteractionText()
    {
        string clipText = "clips";
        if (clips == 1) clipText = "clip";

        string extraInfo = $"{clips} {clipText} to the weapon";
        if (Cost > 0)
        {
            return $"{BaseInteractString} {extraInfo}: {Cost} points";
        }
        return $"{BaseInteractString} {extraInfo}";
    }
}
