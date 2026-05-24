using System;
using System.Drawing;
using UnityEngine;
using UnityStandardAssets.Utility;
public class StuckDetector : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private CanvasGroup restartUI;

    [SerializeField] private float speedThreshold = 1f;
    [SerializeField] private float stuckTime = 3f;

    [SerializeField] private bool isPlayer = false;
    private WaypointProgressTracker tracker;

    private float timer = 0f;

    private Vector3 lastPosition;
    private Quaternion lastRotation;


    //Listener
    private void OnEnable()
    {
        Checkpoint.OnCheckpointReached += HandleCheckpoint;
        LapChecker.OnLapCompleted += NewLap;
    }

    private void OnDisable()
    {
        Checkpoint.OnCheckpointReached -= HandleCheckpoint;
        LapChecker.OnLapCompleted -= NewLap;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tracker = GetComponent<WaypointProgressTracker>();
    }
    private void Start()
    {
        if (restartUI != null) restartUI.alpha = 0;

        SetLastPos();
    }

    private void Update()
    {
        float speed = rb.linearVelocity.magnitude;

        if (speed < speedThreshold)
        {
            timer += Time.deltaTime;

            if (timer >= stuckTime)
            {
                if (restartUI != null) restartUI.alpha = 1;
                else if (!isPlayer) ResetToRoad();
            }
        }
        else
        {
            timer = 0f;
            if (restartUI != null) restartUI.alpha = 0;
        }

        if (Input.GetKeyDown(KeyCode.R) && isPlayer)
        {
            RestartCar();
        }
    }

    private void RestartCar()
    {
        ResetSpeeds();

        transform.position = new Vector3(lastPosition.x, lastPosition.y + 0.5f, lastPosition.z);
        transform.rotation = lastRotation;

        if (restartUI != null) restartUI.alpha = 0;
        timer = 0f;
    }

    private void NewLap(int lapNum)
    {
        if (!isPlayer) return;

        SetLastPos();
    }

    private void HandleCheckpoint(Checkpoint checkpoint)
    {
        if (!isPlayer) return;

        SetLastPos();
    }

    private void ResetToRoad()
    {
        if (tracker == null) return;
        ResetSpeeds();

        float progress = tracker.progressDistance;
        var point = tracker.circuit.GetRoutePoint(progress);

        transform.position = point.position + Vector3.up * 1f;
        transform.rotation = Quaternion.LookRotation(point.direction);

        transform.position = point.position + Vector3.up * 2f;
        SnapToGround(point.direction);

        rb.linearVelocity = transform.forward * 5f;
        timer = 0f;
    }
    private void SnapToGround(Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f))
        {
            transform.position = hit.point + Vector3.up * 0.5f;

            transform.rotation = Quaternion.LookRotation(
                Vector3.ProjectOnPlane(dir, hit.normal),
                hit.normal
            );
        }
    }

    private void ResetSpeeds()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void SetLastPos()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
}
