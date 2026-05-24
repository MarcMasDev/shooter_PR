using UnityEngine;

/// <summary>
/// Actúa como un mando virtual para el enemigo. La FSM modifica estos valores
/// y el WeaponInventory/AnimatorManager los consumen igual que si fuera el jugador.
/// </summary>
public class EnemyInput : MonoBehaviour, IEntityInput
{
    public Vector2 MoveInput { get; set; }
    public Vector3 LookDirection { get; set; }
    public bool IsJumping { get; set; }
    public bool IsCrouching { get; set; }
    public bool IsAiming { get; set; }
    public bool IsShooting { get; set; }
    public bool IsReloading { get; set; }
    public bool IsFlashlight { get; set; }
    public float MouseScrollDelta { get; set; }
    public int WeaponSwapIndex { get; set; } = -1;

    public bool IsInteracting { get; set; }
}