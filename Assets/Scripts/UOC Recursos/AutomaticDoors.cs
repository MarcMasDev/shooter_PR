using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class AutomaticDoors : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public Transform leftClosedLocation;
    public Transform rightClosedLocation;
    public Transform leftOpenLocation;
    public Transform rightOpenLocation;

    public float speed = 1.0f;
    public string doorID;
    public GameObject keyNeeded;

    bool isOpening = false;
    bool isClosing = false;
    bool enableNavmesh = false;
    bool hasBeenUnlocked = false;
    Vector3 distance;

    private void OnEnable()
    {
        Key.OnKeyCollected += HandleKeyCollected;
    }

    private void OnDisable()
    {
        Key.OnKeyCollected -= HandleKeyCollected;
    }
    private void HandleKeyCollected(string id)
    {
        if (id == doorID) hasBeenUnlocked = true;
    }

    void Update ()
    {
        if (isOpening)
        {
            distance = leftDoor.localPosition - leftOpenLocation.localPosition;
            if (distance.magnitude < 0.001f)
            {
                isOpening = false;
                leftDoor.localPosition = leftOpenLocation.localPosition;
                rightDoor.localPosition = rightOpenLocation.localPosition;
            }
            else
            {
                leftDoor.localPosition = Vector3.Lerp(leftDoor.localPosition, leftOpenLocation.localPosition, Time.deltaTime * speed);
                rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, rightOpenLocation.localPosition, Time.deltaTime * speed);
            }
        }
        else if (isClosing)
        {
            distance = leftDoor.localPosition - leftClosedLocation.localPosition;
            if (distance.magnitude < 0.001f)
            {
                isClosing = false;
                leftDoor.localPosition = leftClosedLocation.localPosition;
                rightDoor.localPosition = rightClosedLocation.localPosition;
            }
            else
            {
                leftDoor.localPosition = Vector3.Lerp(leftDoor.localPosition, leftClosedLocation.localPosition, Time.deltaTime * speed);
                rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, rightClosedLocation.localPosition, Time.deltaTime * speed);
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (!hasBeenUnlocked)
        {
            if (keyNeeded != null && col.CompareTag("Player")) keyNeeded.SetActive(true);
            return;
        }

        isOpening = true;
        isClosing = false;
    }

    void OnTriggerStay(Collider col)
    {
        if (!hasBeenUnlocked) return;

        isOpening = true;
        isClosing = false;
    }

    void OnTriggerExit(Collider col)
    {
        if (!hasBeenUnlocked && col.CompareTag("Player"))
        {
            if(keyNeeded != null) keyNeeded.SetActive(false);
            return;
        }
        if (!enableNavmesh)
        {
            enableNavmesh = true;
            leftDoor.GetComponent<NavMeshObstacle>().enabled = false;
            rightDoor.GetComponent<NavMeshObstacle>().enabled = false;
        }

        isClosing = true;
        isOpening = false;
    }
}
