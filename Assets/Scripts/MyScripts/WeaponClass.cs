using NaughtyAttributes;
using UnityEngine;
public enum FireMode
{
    SemiAuto,
    FullAuto,
    Burst
}

[CreateAssetMenu(fileName = "WeaponClass", menuName = "Scriptable Objects/WeaponClass")]
public class WeaponClass : ScriptableObject
{
    public string weaponName = "";
    public bool isMelee = false;

    // MELEE

    [ShowIf("isMelee")]
    [BoxGroup("Melee Settings")]
    public float meleeRange = 2.5f;


    // RANGED

    [HideIf("isMelee")]
    [BoxGroup("Ranged Settings")]
    public FireMode fireMode = FireMode.SemiAuto;

    [HideIf("isMelee")]
    [BoxGroup("Ranged Settings")]
    public float fireRate = 10f;

    [HideIf("isMelee")]
    [EnableIf("IsBurstMode")]
    [BoxGroup("Ranged Settings")]
    public int burstCount = 3;

    [HideIf("isMelee")]
    [BoxGroup("Ammo")]
    public int clipSize = 30;

    [HideIf("isMelee")]
    [BoxGroup("Ammo")]
    public int maxClips = 5;

    [Header("Shared Settings")]
    public float damage = 25f;

    // Helper para el display en el inspector del burst
    private bool IsBurstMode => fireMode == FireMode.Burst;
}
