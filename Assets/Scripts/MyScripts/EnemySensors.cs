using UnityEngine;

public class EnemySensors : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewRadius = 15f;
    [Range(0, 360)] public float viewAngle = 90f;

    [Tooltip("Debe incluir la capa del Player y la de los Peatones")]
    public LayerMask targetMask; // Capa del jugador / peatones
    public LayerMask obstacleMask; // Capa de paredes / suelo

    [Header("Hearing Settings")]
    public float hearingRadius = 8f;

    [SerializeField] private Transform eyes;
    private Transform currentTarget;

    private void Update()
    {
        FindClosestTarget();
    }

    private void FindClosestTarget()
    {
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        float minDistance = float.MaxValue;
        Transform closest = null;

        foreach (var t in targetsInRadius)
        {
            if (t.gameObject == gameObject) continue; // Evitar autodetectarse

            float distance = Vector3.Distance(transform.position, t.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = t.transform;
            }
        }
        currentTarget = closest;
    }

    /// <summary>
    /// Comprueba si el jugador está dentro del cono de visión y no hay paredes en medio.
    /// </summary>
    public bool CanSeePlayer()
    {
        if (currentTarget == null) return false;

        // Apunta al pecho del target (no a sus pies)
        Vector3 targetPos = currentTarget.position + Vector3.up * 1.5f;
        Vector3 dirToTarget = (targetPos - eyes.position).normalized;
        float distanceToTarget = Vector3.Distance(eyes.position, targetPos);

        if (distanceToTarget < viewRadius)
        {
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2f)
            {
                Vector3 flatDir = new Vector3(dirToTarget.x, 0, dirToTarget.z).normalized;
                if (Vector3.Angle(transform.forward, flatDir) < viewAngle / 2f)
                {
                    if (!Physics.Raycast(eyes.position, dirToTarget, distanceToTarget, obstacleMask))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Comprueba si el jugador hace ruido cerca.
    /// </summary>
    public bool CanHearPlayer()
    {
        if (currentTarget == null) return false;
        return Vector3.Distance(transform.position, currentTarget.position) <= hearingRadius;
    }

    public Transform GetCurrentTarget() => currentTarget;
}
