using System.Collections;
using UnityEngine;

public class WeaponRanged : Weapon, IReloadable
{
    [Header("Ranged Specifics")]
    [SerializeField] private ParticleSystem muzzleFlash;

    private int currentAmmo;
    private int currentClips;
    private Coroutine fireRoutine;

    private void Start()
    {
        currentClips = weaponInfo.maxClips;
        currentAmmo = weaponInfo.clipSize;
    }

    // El inventory lo activa cada vez que el jugador dispara
    public override void TryShoot(bool initCooldown = false)
    {
        if (m_StateBlackboard.m_IsPerformingAction || Time.time < nextFireTime || isFiring) return;

        if (initCooldown) nextFireTime = Time.time + (1f / weaponInfo.fireRate);

        switch (weaponInfo.fireMode)
        {
            case FireMode.SemiAuto:
                ExecuteAttack();
                nextFireTime = Time.time + (1f / weaponInfo.fireRate);
                break;

            case FireMode.FullAuto:
                if (fireRoutine == null) fireRoutine = StartCoroutine(FullAutoRoutine());
                break;

            case FireMode.Burst:
                if (fireRoutine == null) fireRoutine = StartCoroutine(BurstRoutine());
                break;
        }
    }

    // El inventory lo activa cada vez que el jugador para de disparar
    public override void StopShooting()
    {
        base.StopShooting();

        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
            fireRoutine = null;
        }

        m_StateBlackboard.TriggerFullAutoAttack(false);
    }

    protected override void ExecuteAttack()
    {
        if (currentAmmo <= 0)
        {
            AudioManager.instance.PlayOneShootFromArray(audioItems, SoundType.emptyShoot);
            return;
        }

        currentAmmo--;
        PerformRaycast();

        TriggerFeedback();
        if (muzzleFlash != null) muzzleFlash.Play();
    }

    private void PerformRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(GetRayOrigin(), out hit, float.PositiveInfinity, impactLayer))
        {
            ApplyDamage(hit);
            ImpactVFX(hit);
        }
    }

    private IEnumerator FullAutoRoutine()
    {
        isFiring = true; 
        m_StateBlackboard.TriggerFullAutoAttack(isFiring);

        while (isFiring)
        {
            ExecuteAttack();
            yield return new WaitForSeconds(1f / weaponInfo.fireRate);
        }
        isFiring = false;
        fireRoutine = null;
        m_StateBlackboard.TriggerFullAutoAttack(false);
    }

    private IEnumerator BurstRoutine()
    {
        isFiring = true;
        m_StateBlackboard.TriggerFullAutoAttack(isFiring);
        for (int i = 0; i < weaponInfo.burstCount; i++)
        {
            ExecuteAttack();
            yield return new WaitForSeconds(1f / (weaponInfo.fireRate * 2f)); // Bursts are usually faster
        }
        nextFireTime = Time.time + (1f / weaponInfo.fireRate);
        isFiring = false;
        fireRoutine = null;
        m_StateBlackboard.TriggerFullAutoAttack(false);
    }

    public void Reload(bool useClip = true)
    {
        if (!useClip) currentClips++;
        if (currentClips <= 0 || currentAmmo >= weaponInfo.clipSize) return;
        StopShooting();
        DisableInput();



        m_StateBlackboard.TriggerReload();
    }
    public void ExecuteReloadAudio()
    {
        if (audioItems != null)
            AudioManager.instance.PlayOneShootFromArray(audioItems, SoundType.reload);
    }
    public void StopReloading()
    {
        int bulletDeficit = weaponInfo.clipSize - currentAmmo;
        int amountToTransfer = Mathf.Min(bulletDeficit, currentClips);

        currentAmmo += amountToTransfer;
        currentClips -= amountToTransfer;

        EnableInput();
    }


    //Animator si no no se entera de que los puede usar
    public new void HideWeapon() => base.HideWeapon();
    public new void EnableInput() => base.EnableInput();
    public new void DisableInput() => base.DisableInput();

    public override Vector2Int GetCurrentAmmo()
    {
        return new Vector2Int(currentAmmo, currentClips);
    }
    public override void AddAmmo(int amount)
    {
        currentClips += amount;
    }
}
