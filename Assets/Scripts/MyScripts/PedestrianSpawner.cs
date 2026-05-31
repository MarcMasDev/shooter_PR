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

        if (newPedestrian != null)
        {
            SetPedestrianDeath(newPedestrian.GetComponent<CharacterBlackboard>(), newPedestrian);
        }

        return newPedestrian;
    }

    private void SetPedestrianDeath(CharacterBlackboard blackboard, GameObject pedestrianInstance)
    {
        if (blackboard == null) return;

        System.Action deathAction = null;
        deathAction = () =>
        {
            activeEntitiesCount--;

            if (pedestrianInstance != null && zombieSpawner != null)
            {
                StartCoroutine(DelayedZombieConversion(pedestrianInstance));//spawn a zombie, solo cuando lo mata un zombie
            }

            blackboard.OnDeath -= deathAction;
        };
        blackboard.OnDeath += deathAction;
    }

    private IEnumerator DelayedZombieConversion(GameObject pedestrianInstance)
    {
        int duration = pedestrianInstance.GetComponent<RagdollAgent>().ragdollDestroyTime;

        //Wait for the ragdoll physics to finish playing out
        yield return new WaitForSeconds(duration - 0.1f);

        //Double check the object wasn't cleaned up by something else in the meantime
        if (pedestrianInstance != null)
        {
            //Grabs the final resting place of the ragdoll
            Vector3 conversionPosition = pedestrianInstance.transform.position;

            bool zombieNearby = Physics.CheckSphere(conversionPosition, infectionRadius, zombieMask);

            //Spawn the zombie at the dead body's feet only if a zombie is near (eating the body)
            if (zombieNearby) zombieSpawner.SpawnZombieAtPosition(conversionPosition);
        }
    }
}
