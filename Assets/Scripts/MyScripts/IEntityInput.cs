using UnityEngine;

public interface IEntityInput
{
    Vector2 MoveInput { get; }
    Vector3 LookDirection { get; }
    bool IsJumping { get; }
    bool IsCrouching { get; }
    bool IsAiming { get; }
    bool IsShooting { get; }
    bool IsReloading { get; }
    bool IsFlashlight { get; }
    bool IsInteracting { get; }
    int WeaponSwapIndex { get; }
    float MouseScrollDelta { get; }
}
