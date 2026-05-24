using UnityEngine;

/// <summary>
/// Clase para obtener todo el input y no incluirlo en otras scripts.
/// Esto permite cambiar los controles facilmente y reaprovechar los demás scripts para otros agentes
/// </summary>
public class PlayerCustomInput : MonoBehaviour, IEntityInput
{
    public Vector2 MoveInput => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    public Vector3 LookDirection => Camera.main.transform.forward;
    public bool IsJumping => Input.GetButtonDown("Jump");
    public bool IsCrouching => Input.GetKey(KeyCode.LeftControl);
    public bool IsRunning => Input.GetKey(KeyCode.LeftShift);
    public bool IsAiming => Input.GetMouseButton(1);
    public bool IsShooting => Input.GetMouseButton(0);
    public bool IsReloading => Input.GetKeyDown(KeyCode.R);
    public bool IsFlashlight => Input.GetKey(KeyCode.F);
    public bool IsInteracting => Input.GetKey(KeyCode.E);
    public float MouseScrollDelta => Input.GetAxis("Mouse ScrollWheel");

    public int WeaponSwapIndex
    {
        get
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) return 0;
            if (Input.GetKeyDown(KeyCode.Alpha2)) return 1;
            if (Input.GetKeyDown(KeyCode.Alpha3)) return 2;
            if (Input.GetKeyDown(KeyCode.Alpha4)) return 3;
            if (Input.GetKeyDown(KeyCode.Alpha5)) return 4;
            if (Input.GetKeyDown(KeyCode.Alpha6)) return 5;
            if (Input.GetKeyDown(KeyCode.Alpha7)) return 6;
            if (Input.GetKeyDown(KeyCode.Alpha8)) return 7;
            if (Input.GetKeyDown(KeyCode.Alpha9)) return 8;
            return -1;
        }
    }
}
