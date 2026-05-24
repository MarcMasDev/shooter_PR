using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

//He adaptado el IENtityInput para que ahora funcione con el PlayerInput (new input system component)
[RequireComponent(typeof(PlayerInput))]
public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private CharacterBlackboard m_StateBlackboard;
    [SerializeField] private Weapon[] weapons;

    private int selectedWeaponIndex = 0;

    //New Input System
    private PlayerInput m_PlayerInput;
    private InputAction m_ShootAction;
    private InputAction m_ReloadAction;
    private InputAction m_ScrollAction;
    private InputAction[] m_WeaponNumberActions;

    private void Awake()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        if (m_StateBlackboard != null) m_StateBlackboard.OnSwapTick += SwapWeapon;
    }

    private void OnDisable()
    {
        if (m_StateBlackboard != null) m_StateBlackboard.OnSwapTick -= SwapWeapon;
    }

    private void Start()
    {
        //Cache the input actions by name
        m_ShootAction = m_PlayerInput.actions["Shoot"];
        m_ReloadAction = m_PlayerInput.actions["Reload"];
        m_ScrollAction = m_PlayerInput.actions["Scroll"];

        m_WeaponNumberActions = new InputAction[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            m_WeaponNumberActions[i] = m_PlayerInput.actions[$"Weapon{i + 1}"];
        }

        SwapWeapon();
    }

    private void Update()
    {
        HandleWeaponSwitching();
        HandleCombatInput();
    }

    private void HandleWeaponSwitching()
    {
        int previousSelectedWeapon = selectedWeaponIndex;

        // Input: rueda. The new Input System reads Scroll as a Vector2. We want the Y axis.
        float scroll = 0f;
        if (m_ScrollAction != null)
        {
            scroll = m_ScrollAction.ReadValue<Vector2>().y;
        }

        if (scroll > 0f) selectedWeaponIndex = (selectedWeaponIndex + 1) % weapons.Length;
        else if (scroll < 0f) selectedWeaponIndex = (selectedWeaponIndex - 1 + weapons.Length) % weapons.Length;

        // Input: números. Si se pulsa un número superior a la cantidad de armas no hace nada
        for (int i = 0; i < m_WeaponNumberActions.Length; i++)
        {
            if (m_WeaponNumberActions[i] != null && m_WeaponNumberActions[i].triggered)
            {
                selectedWeaponIndex = i;
                break; //Stop checking once we find the pressed key
            }
        }

        if (previousSelectedWeapon != selectedWeaponIndex) HideWeapon();
    }

    private void HandleCombatInput()
    {
        Weapon active = GetActiveWeapon();
        if (active == null) return;

        // Shooting
        if (m_ShootAction != null && m_ShootAction.IsPressed()) active.TryShoot();
        else active.StopShooting();

        // Reloading - Solo si el arma se puede recargar
        if (m_ReloadAction != null && m_ReloadAction.triggered && active is IReloadable reloadableWeapon)
        {
            reloadableWeapon.Reload();
        }
    }

    private void HideWeapon()
    {
        m_StateBlackboard.TriggerHide();
        foreach (var weapon in weapons)
        {
            if (weapon.gameObject.activeSelf)
            {
                weapon.StopShooting();
                weapon.DisableInput();
            }
        }
    }

    private void SwapWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == selectedWeaponIndex);
        }
    }

    public Weapon GetActiveWeapon()
    {
        if (selectedWeaponIndex >= 0 && selectedWeaponIndex < weapons.Length)
        {
            return weapons[selectedWeaponIndex];
        }
        return null;
    }

    public void StopReloadFromActiveWeapon()
    {
        WeaponRanged weaponRanged = GetActiveWeapon() as WeaponRanged;

        if (weaponRanged != null) weaponRanged.StopReloading();
    }
}