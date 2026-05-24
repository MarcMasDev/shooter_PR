using UnityEngine;

public class EnemySensors : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewRadius = 15f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask targetMask; // Capa del jugador
    public LayerMask obstacleMask; // Capa de paredes/suelo

    [Header("Hearing Settings")]
    public float hearingRadius = 8f;

    private Transform player;
    [SerializeField] private Transform eyes;

    private void Start()
    {
        // Asumimos que el jugador tiene el tag "Player"
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    /// <summary>
    /// Comprueba si el jugador está dentro del cono de visión y no hay paredes en medio.
    /// </summary>
    public bool CanSeePlayer()
    {
        if (player == null) return false;

        // Apunta al pecho del jugador (no a sus pies)
        Vector3 targetPos = player.position + Vector3.up * 1.5f;

        Vector3 dirToPlayer = (targetPos - eyes.position).normalized;
        float distanceToPlayer = Vector3.Distance(eyes.position, targetPos);

        if (distanceToPlayer < viewRadius)
        {
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f)
            {
                // DEBUG: Dibuja una línea en la ventana de escena para ver el rayo
                Debug.DrawRay(eyes.position, dirToPlayer * distanceToPlayer, Color.green);

                // 3. El rayo ahora sale desde eyePosition
                if (!Physics.Raycast(eyes.position, dirToPlayer, distanceToPlayer, obstacleMask))
                {
                    return true;
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
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= hearingRadius;
    }

    public Transform GetPlayerTransform() => player;
}
