using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 1f;

    private Rigidbody rb;
    private Transform targetPoint;
    private float waitTimer;
    private Vector3 platformDelta;
    private Vector3 previousPosition;

    // We use a list to handle multiple objects on the platform (like NPCs or items)
    private List<CharacterController> passengers = new List<CharacterController>();

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetPoint = pointB;
        previousPosition = rb.position;
    }

    private void FixedUpdate()
    {
        // 1. Calculate movement
        Vector3 newPos = Vector3.MoveTowards(rb.position, targetPoint.position, speed * Time.fixedDeltaTime);

        // 2. Determine how much we moved this frame
        platformDelta = newPos - previousPosition;

        // 3. Move the platform physically
        rb.MovePosition(newPos);
        previousPosition = newPos;

        // 4. Move all passengers manually to stay in sync
        foreach (CharacterController cc in passengers)
        {
            if (cc != null && cc.enabled)
            {
                cc.Move(platformDelta);
            }
        }

        // 5. Handle Point-to-Point logic
        HandleWaypoints();
    }

    private void HandleWaypoints()
    {
        if (Vector3.Distance(rb.position, targetPoint.position) < 0.01f)
        {
            if (waitTimer <= 0)
            {
                targetPoint = (targetPoint == pointA) ? pointB : pointA;
                waitTimer = waitTime;
            }
            else
            {
                waitTimer -= Time.fixedDeltaTime;
            }
        }
    }

    // Manage the list of passengers via triggers
    private void OnTriggerEnter(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null && !passengers.Contains(cc))
        {
            passengers.Add(cc);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null && passengers.Contains(cc))
        {
            passengers.Remove(cc);
        }
    }
}
