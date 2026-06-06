using System;
using UnityEngine;

public class ShowUIHealthbar : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;
    [SerializeField] private EntityHealth m_health;
    [SerializeField] private CharacterBlackboard m_blackboard;
    private void OnEnable()
    {
        m_blackboard.OnHurt += Hurt;
    }
    private void OnDisable()
    {
        m_blackboard.OnHurt -= Hurt;
    }
    private void Hurt(Vector2 vector1, Vector2 vector2) => EnableHealthbar(true);
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EnableHealthbar(true);
            m_health.TakeDamage(0);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) EnableHealthbar(false);
    }
    private void EnableHealthbar(bool enabled)
    {
        healthBar.SetActive(enabled);
    }
}
