using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct CharacterInfo
{
    public GameObject prefab;

    [Header("Probabilidades")]
    [Range(0f, 100f), Tooltip("Probabilidad de salir al inicio de la partida (0 Kills)")]
    public float earlyChance;
    [Range(0f, 100f), Tooltip("Probabilidad de salir al llegar al máximo de dificultad")]
    public float lateChance;
}

public abstract class Spawner : MonoBehaviour
{
    [Header("Base Spawner Settings")]
    protected Transform player;
    [SerializeField] protected int initialSpawn = 15;
    [SerializeField] protected int maxActiveEntities = 30;
    [SerializeField] protected float initialSpawnDelay = 4f;
    [SerializeField] protected float minimumSpawnDelay = 0.6f;

    [Header("Position Rules")]
    [SerializeField] private float minSpawnDistanceFromPlayer = 15f;
    [SerializeField] private float maxSpawnDistanceFromPlayer = 30f;

    protected int activeEntitiesCount = 0;

    protected virtual void Start()
    {
        player = GameManager.Instance.GetPlayerTransform();

        for (int i = 0; i < initialSpawn; i++) SpawnEntity();
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (activeEntitiesCount < maxActiveEntities) SpawnEntity();
            yield return new WaitForSeconds(GetCalculatedDelay());
        }
    }
    protected void SpawnEntity()
    {
        if (NavMesh.SamplePosition(GetSpawnPos(), out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            GameObject newEntity = OnEntityInstantiated(hit.position);
            if (newEntity != null) activeEntitiesCount++;
        }
    }
    protected abstract GameObject OnEntityInstantiated(Vector3 position);
    protected abstract float GetCalculatedDelay();

    private Vector3 GetSpawnPos()
    {
        Vector3 randomDir = Random.onUnitSphere;
        randomDir.y = 0;
        randomDir.Normalize();
        float randomDistance = Random.Range(minSpawnDistanceFromPlayer, maxSpawnDistanceFromPlayer);
        return player.position + (randomDir * randomDistance);
    }
    protected virtual void OnDrawGizmos()
    {
        if (player == null) return;
        Vector3 playerFloorPosition = new Vector3(player.position.x, player.position.y + 0.1f, player.position.z);

        Gizmos.color = Color.yellow;
        DrawGizmoCircle(playerFloorPosition, minSpawnDistanceFromPlayer);
        Gizmos.color = Color.red;
        DrawGizmoCircle(playerFloorPosition, maxSpawnDistanceFromPlayer);
    }

    //Draw circle función helper para los gizmos
    private void DrawGizmoCircle(Vector3 center, float radius)
    {
        int segments = 32;
        float angleStep = 360f / segments;
        Vector3 lastPoint = center + new Vector3(Mathf.Sin(0) * radius, 0, Mathf.Cos(0) * radius);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Sin(angle) * radius, 0, Mathf.Cos(angle) * radius);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }
}
