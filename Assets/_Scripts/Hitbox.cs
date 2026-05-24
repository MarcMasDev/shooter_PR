using UnityEngine;

public enum HitboxType
{
    Body,
    Head,
    Limb,
    Fire
}

public class Hitbox : MonoBehaviour
{
    public HitboxType hitboxType = HitboxType.Body;
}
