using UnityEngine;

public class WeaponMelee : Weapon
{

    // Cuando el jugador aprieta el botón:
    protected override void ExecuteAttack()
    {        
        // No hacemos damage, esperamos a la animación...
        m_StateBlackboard.TriggerAttack();
        print("ATTACK!");

        if (audioItems != null) AudioManager.instance.PlayOneShootFromArray(audioItems, SoundType.shoot);

        DisableInput();
    }

   // Hacemos daño
    public void HitEvent()
    {
        PerformMeleeCast();
    }

    private void PerformMeleeCast()
    {
        RaycastHit hit;
 
        if (Physics.Raycast(GetRayOrigin(), out hit, weaponInfo.meleeRange, impactLayer))
        {
            ApplyDamage(hit);
            ImpactVFX(hit);
        }
    }

    // Al acabar el ataque
    public void ResetAttack() => EnableInput();

    //Animator si no no se entera de que los puede usar
    public new void HideWeapon() => base.HideWeapon();
    public new void EnableInput() => base.EnableInput();
    public new void DisableInput() => base.DisableInput();
}
