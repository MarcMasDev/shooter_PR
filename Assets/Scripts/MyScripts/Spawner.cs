using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[System.Serializable]
public struct EnemyInfo
{
    public GameObject prefab;

    [Header("Probabilidades")]
    [Range(0f, 100f), Tooltip("Probabilidad de salir al inicio de la partida (0 Kills)")]
    public float earlyChance;
    [Range(0f, 100f), Tooltip("Probabilidad de salir al llegar al máximo de dificultad")]
    public float lateChance;
}

public class Spawner : MonoBehaviour
{
    [SerializeField] private EnemyInfo[] zombies;
    [SerializeField] private Transform player;
    [SerializeField] private int initialSpawn = 20;

    [Header("Dificultad")]
    [SerializeField] private float initialSpawnDelay = 5f;
    [SerializeField] private float minimumSpawnDelay = 0.5f;
    [SerializeField] private int killsToReachMaxDifficulty = 100;
    [SerializeField] private float gameTimer = 0f;

    [Tooltip("Velocidades")]
    [SerializeField] private float zombieWanderSpeed = 1.5f;
    [SerializeField] private float walkerChaseSpeed = 3.5f;
    [SerializeField] private float runnerChaseSpeed = 6.5f;
    [SerializeField] private Vector2 randomSpeedRange = new Vector2(-0.5f, 0.5f);

    [Range(0f, 100f), Tooltip("Probabilidad inicial de que salga un Runner (0 Kills)")]
    [SerializeField] private float initialRunnerChance = 5f;
    [Range(0f, 100f), Tooltip("Probabilidad máxima de que salga un Runner (Al llegar al límite de Kills)")]
    [SerializeField] private float maxRunnerChance = 95f;

    [Header("Reglas de Spawn")]
    [SerializeField] private float minSpawnDistanceFromPlayer = 15f;
    [SerializeField] private float maxSpawnDistanceFromPlayer = 30f;
    [SerializeField] private int maxActiveZombies = 30;


    private int activeZombiesCount = 0;

    private void Start()
    {
        for (int i = 0; i < initialSpawn; i++) SpawnZombie();

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (activeZombiesCount < maxActiveZombies) SpawnZombie();

            //Calculamos cuánto esperar para el SIGUIENTE zombie basado en las kills actuales
            float currentDelay = Mathf.Lerp(initialSpawnDelay, minimumSpawnDelay, GetCurrentDificulty);

            yield return new WaitForSeconds(currentDelay);
        }
    }
    private void SpawnZombie()
    {
        if (zombies == null || zombies.Length == 0) return;

        //Comprobar si es un punto válido en el NavMesh
        if (NavMesh.SamplePosition(GetSpawnPos(), out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            GameObject newZombie = InstantiateEnemy(hit.position);
            if (newZombie == null) return;

            activeZombiesCount++;

            SetZombieSpeed(newZombie);

            SetZombieDeath(newZombie.GetComponent<CharacterBlackboard>());

        }
    }

    //Posición aleatoria
    private Vector3 GetSpawnPos()
    {
        Vector3 randomDir = Random.onUnitSphere;
        randomDir.y = 0;
        randomDir.Normalize();

        float randomDistance = Random.Range(minSpawnDistanceFromPlayer, maxSpawnDistanceFromPlayer);

        return player.position + (randomDir * randomDistance);
    }

    //Spawn en base a los chances dinámicos (Early vs Late)
    private GameObject InstantiateEnemy(Vector3 spawnPos)
    {
        //Calcular el peso o probabilidad total actual del pool de enemigos
        float totalWeight = 0f;
        float[] currentWeights = new float[zombies.Length];

        for (int i = 0; i < zombies.Length; i++)
        {
            currentWeights[i] = Mathf.Lerp(zombies[i].earlyChance, zombies[i].lateChance, GetCurrentDificulty);
            totalWeight += currentWeights[i];
        }
        
        //Tirar dado
        float randomRoll = Random.Range(0f, totalWeight);
        float processedWeight = 0f;

        //Seleccionar el zombie correspondiente
        for (int i = 0; i < zombies.Length; i++)
        {
            processedWeight += currentWeights[i];
            if (randomRoll <= processedWeight) return Instantiate(zombies[i].prefab, spawnPos, Quaternion.identity);
        }

        return Instantiate(zombies[0].prefab, spawnPos, Quaternion.identity);   //Por si acaso, devolvemos el primero
    }

    //Set chase speed en base a las kills (dificultad)
    private void SetZombieSpeed(GameObject agent)
    {
        //Calcular la probabilidad ACTUAL de que sea un Runner basado en las kills
        float currentRunnerChance = Mathf.Lerp(initialRunnerChance, maxRunnerChance, GetCurrentDificulty);

        //Tirar el dado (Número aleatorio entre 0 y 100)
        float randomRoll = Random.Range(0f, 100f);
        float randomSpeed = Random.Range(randomSpeedRange.x, randomSpeedRange.y);
        float finalChaseSpeed = zombieWanderSpeed;

        if (randomRoll <= currentRunnerChance) finalChaseSpeed = runnerChaseSpeed;

        //Aplicar las velocidades a su FSM
        EnemyFSM fsm = agent.GetComponent<EnemyFSM>();
        if (fsm != null) fsm.SetupSpeeds(zombieWanderSpeed + randomSpeed, finalChaseSpeed + randomSpeed);
    }

    //Suscribirse a su muerte
    private void SetZombieDeath(CharacterBlackboard blackboard)
    {
        if (blackboard != null)
        {
            Action deathAction = null;

            deathAction = () => //Al morir...
            {
                activeZombiesCount--;
                ScoreManager.Instance.AddKill();
                blackboard.OnDeath -= deathAction;
            };

            blackboard.OnDeath += deathAction;
        }
    }
    private void OnDrawGizmos()
    {
        if (player == null) return; //Si el player no está asignado, no dibujamos nada para evitar errores

        Matrix4x4 oldMatrix = Gizmos.matrix;

        //Creamos una matriz basada en la posición del jugador pero totalmente horizontal en el suelo
        Vector3 playerFloorPosition = new Vector3(player.position.x, player.position.y + 0.1f, player.position.z);

        //Dibujar el círculo de distancia MÍNIMA
        Gizmos.color = Color.yellow;
        DrawGizmoCircle(playerFloorPosition, minSpawnDistanceFromPlayer);

        //Dibujar el círculo de distancia MÁXIMA
        Gizmos.color = Color.red;
        DrawGizmoCircle(playerFloorPosition, maxSpawnDistanceFromPlayer);

        //Connexión entre ambos rangos para ver el grosor del anillo de spawn
        Gizmos.color = Color.orange;
        Gizmos.DrawLine(playerFloorPosition + Vector3.forward * minSpawnDistanceFromPlayer, playerFloorPosition + Vector3.forward * maxSpawnDistanceFromPlayer);
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
    private float GetCurrentDificulty => Mathf.Clamp01((float)ScoreManager.Instance.GetCurrentKills() / killsToReachMaxDifficulty);
}
