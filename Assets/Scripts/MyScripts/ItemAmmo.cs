using UnityEngine;
using UnityEngine.Audio;

public class ItemAmmo : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactString = "Press 'E' to add ";
    [SerializeField] private int clips = 1;
    [SerializeField] private AudioSource audioSource;
    public bool Interact(GameObject user)
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
    public string GetInteractionText() => interactString + "+" + clips + " clip(s) to the weapon";
}
