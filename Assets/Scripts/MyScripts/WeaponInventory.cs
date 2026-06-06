using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

[System.Serializable]
public struct SavedWeapon
{
    public Weapon weapon;
    public Transform handPos;
}
public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private CharacterBlackboard m_StateBlackboard;
    [SerializeField] private SavedWeapon[] weapons;
    [SerializeField] private Transform helperHand;

    private int selectedWeaponIndex = 0;

    //New Input System
    private InputAction m_ShootAction;
    private InputAction m_ReloadAction;
    private InputAction m_ScrollAction;
    private InputAction[] m_WeaponNumberActions;

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
        m_ShootAction = GameManager.Instance.GetInput().actions["Shoot"];
        m_ReloadAction = GameManager.Instance.GetInput().actions["Reload"];
        m_ScrollAction = GameManager.Instance.GetInput().actions["Scroll"];

        m_WeaponNumberActions = new InputAction[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            m_WeaponNumberActions[i] = GameManager.Instance.GetInput().actions[$"Weapon{i + 1}"];
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

        if (previousSelectedWeapon != selectedWeaponIndex)
        {
            HideWeapon();
            SwapWeapon();
        }
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
            if (weapon.weapon.gameObject.activeSelf)
            {
                weapon.weapon.StopShooting();
                weapon.weapon.DisableInput();
            }
        }
    }

    private void SwapWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == selectedWeaponIndex)
            {
                weapons[i].weapon.gameObject.SetActive(true);
                weapons[i].weapon.EnableInput();
                helperHand.localPosition = weapons[i].handPos.localPosition;
                helperHand.localRotation = weapons[i].handPos.localRotation;
            }
            else weapons[i].weapon.gameObject.SetActive(false);
        }
    }

    public Weapon GetActiveWeapon()
    {
        if (selectedWeaponIndex >= 0 && selectedWeaponIndex < weapons.Length)
        {
            return weapons[selectedWeaponIndex].weapon;
        }
        return null;
    }

    public void StopReloadFromActiveWeapon()
    {
        WeaponRanged weaponRanged = GetActiveWeapon() as WeaponRanged;

        if (weaponRanged != null) weaponRanged.StopReloading();
    }

    public void SetActiveWeaponHitEvent()
    {
        WeaponMelee weaponMelee = GetActiveWeapon() as WeaponMelee;
        if (weaponMelee != null) weaponMelee.HitEvent();
    }
    public void EnableCurrentWeaponInput()
    {
        GetActiveWeapon().EnableInput();
    }
}