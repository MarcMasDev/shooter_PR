using System.Collections;
using UnityEngine;

public class PedestrianSpawner : Spawner
{
    [Header("Pedestrian Pools")]
    [SerializeField] private CharacterInfo[] pedestrians;

    [Header("Infection Config")]
    [SerializeField] private ZombieSpawner zombieSpawner;
    [SerializeField] private float infectionRadius = 5f;
    [SerializeField] private LayerMask zombieMask;
    protected override float GetCalculatedDelay() => Random.Range(initialSpawnDelay, initialSpawnDelay * 1.5f);

    protected override GameObject OnEntityInstantiated(Vector3 position)
    {
        if (pedestrians == null || pedestrians.Length == 0) return null;

        int randomIndex = Random.Range(0, pedestrians.Length);
        GameObject newPedestrian = Instantiate(pedestrians[randomIndex].prefab, position, Quaternion.identity);

        if (newPedestrian != null) SetPedestrianDeath(newPedestrian);
        return newPedestrian;
    }

    private void SetPedestrianDeath(GameObject pedestrianInstance)
    {
        if (pedestrianInstance.TryGetComponent<RagdollAgent>(out var ragdoll))
        {
            ragdoll.OnRagdollCleanup += (finalPosition) => ExecuteImmediateConversion(finalPosition);
        }
    }
    private void ExecuteImmediateConversion(Vector3 finalPosition)
    {
        if (zombieSpawner == null) return;

        //Check if a zombie is currently eating the body at its final resting place
        bool zombieNearby = Physics.CheckSphere(finalPosition, infectionRadius, zombieMask);

        if (zombieNearby) zombieSpawner.SpawnZombieAtPosition(finalPosition, true);
    }
}
