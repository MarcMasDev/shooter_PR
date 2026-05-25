using System;
using UnityEngine;

public class CharacterBlackboard : MonoBehaviour
{
    private float m_NormalizedSpeed;
    private bool m_IsAiming;
    [HideInInspector] public bool m_IsPerformingAction = false;

    public event Action OnFootstep;
    public event Action OnJump;
    public event Action OnLand;
    public event Action<Vector2, Vector2> OnHurt;
    public event Action<Vector2, Vector2> OnHeal;
    public event Action OnDeath;
    public event Action<bool> OnAttack;
    public event Action OnReload;
    public event Action OnHide;
    public event Action OnSwapTick;
    public event Action<bool> OnSearch;

    //private PlayerInput m_PlayerInput;
    //public PlayerInput GetPlayerInput()
    //{
    //    if (m_PlayerInput == null) m_PlayerInput = GetComponent<PlayerInput>();
    //    return m_PlayerInput;
    //} 

    // Setter
    public void SetNormalizedSpeed(float currentSpeed, float maxSpeed)
    {
        m_NormalizedSpeed = Mathf.Min(currentSpeed, maxSpeed);
    }

    public float GetNormalizedSpeed() => m_NormalizedSpeed;

    public void SetAim(bool isAiming) => m_IsAiming = isAiming;
    public bool GetAim() => !m_IsPerformingAction && m_IsAiming;

    // Event Triggers
    public void TriggerJump() => OnJump?.Invoke();
    public void TriggerLand() => OnLand?.Invoke();
    public void TriggerFootstep() => OnFootstep?.Invoke();
    public void TriggerHurt(Vector2 health, Vector2 shield) => OnHurt?.Invoke(health, shield);
    public void TriggerHeal(Vector2 health, Vector2 shield) => OnHeal?.Invoke(health, shield);
    public void TriggerDeath() => OnDeath?.Invoke();
    public void TriggerAttack(bool isFiring) => OnAttack?.Invoke(isFiring);
    public void TriggerReload() => OnReload?.Invoke();
    public void TriggerHide() => OnHide?.Invoke();
    public void TriggerSwap() => OnSwapTick?.Invoke();
    public void TriggerSearch(bool isInvestigating) => OnSearch?.Invoke(isInvestigating);
}
