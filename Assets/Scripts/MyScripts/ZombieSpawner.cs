using UnityEngine;

public class ZombieSpawner : Spawner
{
    [Header("Zombie Pools")]
    [SerializeField] private CharacterInfo[] zombies;

    [Header("Difficulty Modifiers")]
    [SerializeField] private int killsToReachMaxDifficulty = 100;

    [Header("Speeds")]
    [SerializeField] private float zombieWanderSpeed = 1.5f;
    [SerializeField] private float walkerChaseSpeed = 3.5f;
    [SerializeField] private float runnerChaseSpeed = 6.5f;
    [SerializeField] private Vector2 randomSpeedRange = new Vector2(-0.5f, 0.5f);

    [Header("Runner Chances")]
    [Range(0f, 100f)][SerializeField] private float initialRunnerChance = 5f;
    [Range(0f, 100f)][SerializeField] private float maxRunnerChance = 95f;

    private float GetCurrentDifficulty => Mathf.Clamp01((float)ScoreManager.Instance.GetCurrentKills() / killsToReachMaxDifficulty);

    protected override float GetCalculatedDelay() => Mathf.Lerp(initialSpawnDelay, minimumSpawnDelay, GetCurrentDifficulty);

    protected override GameObject OnEntityInstantiated(Vector3 position)
    {
        if (zombies == null || zombies.Length == 0) return null;

        GameObject newZombie = GetRandomZombieByWeight(position);
        if (newZombie == null) return null;

        SetZombieSpeed(newZombie);
        SetZombieDeath(newZombie.GetComponent<CharacterBlackboard>());

        return newZombie;
    }

    //Metodo para spawnear un zombie cuando hay un pederastian que es comido
    public void SpawnZombieAtPosition(Vector3 position)
    {
        if (activeEntitiesCount < maxActiveEntities)
        {
            //OnEntityInstantiated(position);
            activeEntitiesCount++;
        }
    }

    //Para aumentar la dificultad, el sistema incluye una selección de tipos dinámica que aumenta junto con la dificultad del juego cada vez que el jugador mata a un enemigo
    private GameObject GetRandomZombieByWeight(Vector3 spawnPos)
    {
        float totalWeight = 0f;
        float[] currentWeights = new float[zombies.Length];

        for (int i = 0; i < zombies.Length; i++)
        {
            currentWeights[i] = Mathf.Lerp(zombies[i].earlyChance, zombies[i].lateChance, GetCurrentDifficulty);
            totalWeight += currentWeights[i];
        }

        float randomRoll = Random.Range(0f, totalWeight);
        float processedWeight = 0f;

        for (int i = 0; i < zombies.Length; i++)
        {
            processedWeight += currentWeights[i];
            if (randomRoll <= processedWeight) return Instantiate(zombies[i].prefab, spawnPos, Quaternion.identity);
        }

        return Instantiate(zombies[0].prefab, spawnPos, Quaternion.identity);
    }

    //Para aumentar la dificultad, el sistema incluye speed dinámica que aumenta junto con la dificultad del juego cada vez que el jugador mata a un enemigo
    private void SetZombieSpeed(GameObject agent)
    {
        float currentRunnerChance = Mathf.Lerp(initialRunnerChance, maxRunnerChance, GetCurrentDifficulty);
        float randomRoll = Random.Range(0f, 100f);
        float randomSpeed = Random.Range(randomSpeedRange.x, randomSpeedRange.y);
        float finalChaseSpeed = zombieWanderSpeed;

        if (randomRoll <= currentRunnerChance) finalChaseSpeed = runnerChaseSpeed;

        EnemyFSM fsm = agent.GetComponent<EnemyFSM>();
        if (fsm != null) fsm.SetupSpeeds(zombieWanderSpeed + randomSpeed, finalChaseSpeed + randomSpeed);
    }

    private void SetZombieDeath(CharacterBlackboard blackboard)
    {
        if (blackboard == null) return;

        System.Action deathAction = null;
        deathAction = () =>
        {
            activeEntitiesCount--;
            ScoreManager.Instance.AddKill();
            blackboard.OnDeath -= deathAction;
        };
        blackboard.OnDeath += deathAction;
    }
}
