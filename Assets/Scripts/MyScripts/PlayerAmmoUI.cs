using TMPro;
using UnityEngine;

public class PlayerAmmoUI : MonoBehaviour
{
    [SerializeField] private WeaponInventory m_WeaponInventory; 
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text clipsText;
    [SerializeField] private TMP_Text weaponName;

    private void Update()
    {
        Weapon weapon = m_WeaponInventory.GetActiveWeapon();

        //Fallback (por ejemplo cuando estamos cambiando de arma)
        if (weapon == null)
        {
            weaponName.text = "";
            ammoText.text = "";
            clipsText.text = "";
            return;
        }

        //Name
        weaponName.text = weapon.GetWeaponName();

        //Ammo
        Vector2 currentAmmo = weapon.GetCurrentAmmo();

        if (currentAmmo.x == -1)
        {
            ammoText.text = "";
            clipsText.text = "";
        }
        else
        {
            ammoText.text = currentAmmo.x.ToString();
            clipsText.text = "// " + currentAmmo.y.ToString();
        }
    }
}
