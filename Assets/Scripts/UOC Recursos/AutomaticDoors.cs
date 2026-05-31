using UnityEngine;
using UnityEngine.AI;

public class AutomaticDoors : PurchasableInteractable
{
    [Header("Door Identification")]
    public string doorID;

    [Header("Door Moving Parts")]
    public Transform leftDoor;
    public Transform rightDoor;
    public Transform leftClosedLocation;
    public Transform rightClosedLocation;
    public Transform leftOpenLocation;
    public Transform rightOpenLocation;
    public float speed = 1.0f;

    private bool hasKey = false;
    private bool isOpening = false;
    private bool hasBeenPurchased = false;
    private Vector3 distance;

    private void OnEnable() => Key.OnKeyCollected += HandleKeyCollected;
    private void OnDisable() => Key.OnKeyCollected -= HandleKeyCollected;

    private void HandleKeyCollected(string id)
    {
        if (id == doorID) hasKey = true;
    }

    protected override bool PreConditionMet(GameObject user)
    {
        if (hasBeenPurchased) return false;
        return hasKey;
    }

    protected override bool ExecuteInteraction(GameObject user)
    {
        isOpening = true;
        hasBeenPurchased = true;

        //Disable NavMesh obstacles so AI can walk through
        if (leftDoor.TryGetComponent(out NavMeshObstacle leftObstacle)) leftObstacle.enabled = false;
        if (rightDoor.TryGetComponent(out NavMeshObstacle rightObstacle)) rightObstacle.enabled = false;

        //Disable this interaction collider entirely so it can't be triggered again
        if (TryGetComponent(out Collider col)) col.enabled = false;

        return true;
    }

    //Dynamic UI Prompt management
    public override string GetInteractionText()
    {
        if (hasBeenPurchased) return "";

        if (!hasKey) return $"Locked: Requires {doorID} Key and has a cost of {Cost}";
        return $"{BaseInteractString} [Cost: {Cost}]";
    }

    void Update()
    {
        if (!isOpening) return;

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
}
