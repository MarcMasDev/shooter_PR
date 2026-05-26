using NaughtyAttributes;
using UnityEngine;

public interface IReloadable
{
    void Reload(bool useClip = true);
    void StopReloading();
}

/// <summary>
/// Esta clase agrupa ambas clases: armas melee y a rango.
/// Esta es su clase padre que se encarga de manejar toda la tecnología que comparten
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected CharacterBlackboard m_StateBlackboard;
    [SerializeField] protected WeaponClass weaponInfo;
    [SerializeField] protected AudioItem[] audioItems;
    [SerializeField] protected GameObject impact;

    [SerializeField] protected GameObject bloodEffect;
    [SerializeField] private LayerMask bloodLayers;

    [SerializeField] protected float vfxDestroyTime = 10f;
    [SerializeField] protected LayerMask impactLayer;
    [HideIf("isPlayer")][SerializeField] protected Transform origin;

    protected float nextFireTime;
    protected bool isFiring;
    private bool isPlayer => transform.root.CompareTag("Player");
    public virtual void TryShoot(bool initCooldown = false)
    {
        if (weaponInfo == null || m_StateBlackboard.m_IsPerformingAction) return;
        if (initCooldown) nextFireTime = Time.time + (1f / weaponInfo.fireRate);

        if (Time.time >= nextFireTime)
        {
            ExecuteAttack();
        }
    }

    protected abstract void ExecuteAttack();

    public virtual void StopShooting() => isFiring = false;

    protected void TriggerFeedback()
    {
        m_StateBlackboard.TriggerAttack(true);
        if (audioItems != null) AudioManager.instance.PlayOneShootFromArray(audioItems, SoundType.shoot);
    }

    public void EnableInput()
    {
        m_StateBlackboard.m_IsPerformingAction = false;
    }
    public void DisableInput() => m_StateBlackboard.m_IsPerformingAction = true;
    public void HideWeapon() => m_StateBlackboard.TriggerSwap();

    protected void ApplyDamage(RaycastHit hit)
    {
        float damage = weaponInfo.damage;
        HitboxType hitbox = HitboxType.Body;

        if (hit.collider.TryGetComponent(out Hitbox hitboxComponent))
        {
            hitbox = hitboxComponent.hitboxType;
            damage *= HitboxDamage.GetMultiplier(hitbox);
        }

        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            ImpactResult result = damageable.TakeDamage(damage, transform.root.gameObject);

            if (isPlayer && result != ImpactResult.alreadyDeath) ScoreManager.Instance.AddPoints(hitbox, result == ImpactResult.death);
        }
    }

    protected void ImpactVFX(RaycastHit hit)
    {
        GameObject prefabToSpawn = impact;
        bool isBlood = IsBloodAgent(hit.collider);
        if (isBlood) prefabToSpawn = bloodEffect;

        if (prefabToSpawn != null)
        {
            // Evita el z-figthing
            Vector3 spawnPos = hit.point + (hit.normal * 0.01f);
            Quaternion spawnRot = Quaternion.LookRotation(hit.normal);

            //Crea el efecto
            GameObject vfxInstance = Instantiate(prefabToSpawn, spawnPos, spawnRot);

            //Lo ponemos como hijo, asi se mueve con el padre.
            if (!isBlood)  vfxInstance.transform.SetParent(hit.transform);

            Destroy(vfxInstance, vfxDestroyTime);
        }
    }


    public virtual Vector2Int GetCurrentAmmo()
    {
        return new Vector2Int(-1, -1);
    }

    public string GetWeaponName()
    {
        return weaponInfo.name;
    }
    public int GetClipSize()
    {
        return weaponInfo.clipSize;
    }

    private bool IsBloodAgent(Collider hit) => (bloodLayers.value & (1 << hit.gameObject.layer)) != 0;
    protected Ray GetRayOrigin()
    {
        if (isPlayer) return Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        else return new Ray(origin.position, origin.forward); ;
    }

    public virtual void AddAmmo(int amount) { }
}
