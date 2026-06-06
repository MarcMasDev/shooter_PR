using UnityEngine;

[RequireComponent (typeof(Animator))]
public class CharacterAnimationManager : MonoBehaviour
{
    [SerializeField] private CharacterBlackboard m_StateBlackboard;
    private Animator m_Animator;
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        if (m_StateBlackboard != null)
        {
            m_StateBlackboard.OnAttack += OnAttack;
            m_StateBlackboard.OnReload += Reload;
            m_StateBlackboard.OnHide += Hide;
            m_StateBlackboard.OnHurt += Hurt;
            m_StateBlackboard.OnDeath += Death;
            m_StateBlackboard.OnJump += Jump;
            m_StateBlackboard.OnSearch += OnSearch;
        }
    }

    private void OnDisable()
    {
        if (m_StateBlackboard != null)
        {
            m_StateBlackboard.OnAttack -= OnAttack;
            m_StateBlackboard.OnReload -= Reload;
            m_StateBlackboard.OnHide -= Hide;
            m_StateBlackboard.OnHurt -= Hurt;
            m_StateBlackboard.OnDeath -= Death;
            m_StateBlackboard.OnJump -= Jump;
            m_StateBlackboard.OnSearch -= OnSearch;
        }
    }
    private void Update()
    {
        if (m_Animator == null || m_StateBlackboard == null) return;

        m_Animator.SetFloat("Speed", m_StateBlackboard.GetNormalizedSpeed(), 0.1f, Time.deltaTime);

        m_Animator.SetBool("IsAiming", m_StateBlackboard.GetAim()); 
    }

    private void OnAttack(bool isFiring, bool melee = false)
    {
        m_Animator.SetBool("Attack", isFiring);

        if (melee) m_Animator.SetTrigger("MeleeAttack");
    }
    private void Reload() => m_Animator.SetTrigger("Reload");
    private void Hide() => m_Animator.SetTrigger("Hide");
    private void Jump() => m_Animator.SetTrigger("Jump");
    private void Hurt(Vector2 health, Vector2 shield) => m_Animator.SetTrigger("Hurt");
    private void Death() => m_Animator.SetTrigger("Death");
    private void OnSearch(bool isSearching) => m_Animator.SetBool("Search", isSearching);
}
