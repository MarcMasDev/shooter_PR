using UnityEngine;

[System.Serializable]
public class LootItem
{
    public GameObject prefab;
    [Range(0, 100)] public float weight; //Más alto = más común 
}
public class LootDrop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterBlackboard m_Blackboard;

    [Header("Settings")]
    [SerializeField] private LootItem[] m_Loot;
    [SerializeField, Range(0, 1)] private float m_DropChance = 100f; // 1.0 = 100% chance to drop SOMETHING

    private void OnEnable()
    {
        if (m_Blackboard != null) m_Blackboard.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (m_Blackboard != null) m_Blackboard.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        //Comrpueba si tiene que dropear algo
        if (Random.value > m_DropChance) return;

        //Pilla un item aleatorio en base a sus probabilidades
        GameObject itemToDrop = GetWeightedRandomItem();

        if (itemToDrop != null)
        {
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
        }
    }

    private GameObject GetWeightedRandomItem()
    {
        if (m_Loot == null || m_Loot.Length == 0) return null;

        float totalWeight = 0f;
        foreach (var item in m_Loot)
        {
            totalWeight += item.weight;
        }

        float pivot = Random.Range(0, totalWeight);
        float s = 0;

        //Weighted random system
        foreach (var item in m_Loot)
        {
            s += item.weight;
            if (pivot <= s)
            {
                return item.prefab;
            }
        }

        return null;
    }
}
