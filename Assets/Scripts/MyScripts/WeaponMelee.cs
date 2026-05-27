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
        RaycastHit hit;

        //Comprobación SphereCast: barre una esfera hacia delante.
        //Para asegurar que golpea enemigos que están justo dentro del pivote
        Vector3 backOrigin = ray.origin - (ray.direction * weaponInfo.meleeRadius);
        float adjustedDistance = weaponInfo.meleeRange + weaponInfo.meleeRadius;

        if (Physics.SphereCast(backOrigin, weaponInfo.meleeRadius, ray.direction, out hit, adjustedDistance, impactLayer))
        {
            ProcessMeleeHit(hit);
            return;
        }

        //Comprobación de seguridad a corta distancia (OverlapSphere):
        //Si el golpe falla debido a ángulos extremadamente cercanos,comprobamos una pequeña zona justo delante.
        Collider[] closeColliders = Physics.OverlapSphere(ray.origin + ray.direction * (weaponInfo.meleeRange * 0.5f), weaponInfo.meleeRange * 0.5f, impactLayer);

        if (closeColliders.Length > 0)
        {
            //Find the closest collider to the aim direction
            Collider bestTarget = null;
            float closestDistance = float.MaxValue;

            foreach (var col in closeColliders)
            {
                float dist = Vector3.Distance(ray.origin, col.bounds.ClosestPoint(ray.origin));
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    bestTarget = col;
                }
            }
            if (bestTarget != null)
            {
                //Simular un raycast directo hacia el centro de ese collider específico.
                Vector3 directionToTarget = (bestTarget.bounds.center - ray.origin).normalized;
                if (Physics.Raycast(ray.origin, directionToTarget, out hit, weaponInfo.meleeRange, impactLayer))
                {
                    ProcessMeleeHit(hit);
                    return;
                }
            }
        }
    }

    private void ProcessMeleeHit(RaycastHit hit)
    {
        // Execute the parent class logic safely
        ApplyDamage(hit);
        ImpactVFX(hit);
    }

    // Al acabar el ataque
    public void ResetAttack() => EnableInput();

    //Animator si no no se entera de que los puede usar
    public new void HideWeapon() => base.HideWeapon();
    public new void EnableInput() => base.EnableInput();
    public new void DisableInput() => base.DisableInput();
}
