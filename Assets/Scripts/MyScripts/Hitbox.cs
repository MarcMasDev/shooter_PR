using UnityEngine;

public enum HitboxType
{
    Body,
    Head,
    Limb
}
public static class HitboxDamage
{
    public static float GetMultiplier(HitboxType hitboxType)
    {
        switch (hitboxType)
        {
            case HitboxType.Head:
                return 2f;
            case HitboxType.Limb:
                return 0.75f;
            default:
                return 1f;
        }
    }
}

public class Hitbox : MonoBehaviour
{
    public HitboxType hitboxType = HitboxType.Body;
}
