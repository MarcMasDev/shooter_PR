using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private CharacterBlackboard m_StateBlackboard;


    [SerializeField] private Animator m_hurtAnim;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider shieldSlider;
    [SerializeField] private TMP_Text shieldText;

    private void OnEnable()
    {
        if (m_StateBlackboard != null)
        {
            m_StateBlackboard.OnHurt += OnHurt;
            m_StateBlackboard.OnHeal += UpdateHealthUI;
            m_StateBlackboard.OnDeath += Death;
        }
    }

    private void OnDisable()
    {
        if (m_StateBlackboard != null)
        {
            m_StateBlackboard.OnHurt -= OnHurt;
            m_StateBlackboard.OnHeal -= UpdateHealthUI;
            m_StateBlackboard.OnDeath -= Death;
        }
    }
    private void OnHurt(Vector2 health, Vector2 shield)
    {
        m_hurtAnim.SetTrigger("Hurt");
        UpdateHealthUI(health, shield);
    }
    private void UpdateHealthUI(Vector2 health, Vector2 shield)
    {
        //Health
        UpdateValues(healthSlider, healthText, health.x, health.y);

        //Shield
        UpdateValues(shieldSlider, shieldText, shield.x, shield.y);
    }
    private void UpdateValues(Slider s, TMP_Text t, float current, float max)
    {
        s.value = current / max;
        t.text = Mathf.CeilToInt(current).ToString();
    }
    private void Death() {}
}
