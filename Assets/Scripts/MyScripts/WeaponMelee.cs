using UnityEngine;

public class WeaponMelee : Weapon
{

    // Cuando el jugador aprieta el botón:
    protected override void ExecuteAttack()
    {        
        // No hacemos damage, esperamos a la animación...
        m_StateBlackboard.TriggerAttack(true);

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
        Ray ray = GetRayOrigin();
        float range = weaponInfo != null ? weaponInfo.meleeRange : 3f;

        if (Physics.Raycast(ray, out RaycastHit hit, range, impactLayer))
        {
            ApplyDamage(hit);
            ImpactVFX(hit);
            return;
        }
    }

    // Al acabar el ataque
    public void ResetAttack() => EnableInput();

    //Animator si no no se entera de que los puede usar
    public new void HideWeapon() => base.HideWeapon();
    public new void EnableInput() => base.EnableInput();
    public new void DisableInput() => base.DisableInput();
}
