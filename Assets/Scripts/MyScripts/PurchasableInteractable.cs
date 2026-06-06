using UnityEngine;

public abstract class PurchasableInteractable : MonoBehaviour, IInteractable
{
    [Header("Purchase Settings")]
    [SerializeField] private string interactString = "Interact";
    [SerializeField] private int cost = 0;

    public int Cost => cost;
    public string BaseInteractString => interactString;

    public bool Interact(GameObject user)
    {
        if (!PreConditionMet(user)) return false;

        if (cost > 0)
        {
            bool enoughPoints = ScoreManager.Instance.CheckPoints(cost);
            if (!enoughPoints) return false;

            bool interactionExecuted = ExecuteInteraction(user);
            if (!interactionExecuted) return false;

            ScoreManager.Instance.DecreasePoints(cost);
        }
        else ExecuteInteraction(user);

        return true;
    }
    protected virtual bool PreConditionMet(GameObject user) => true;

    public virtual string GetInteractionText()
    {
        if (cost > 0) return $"{interactString}: {cost} points";
        return interactString;
    }

    protected abstract bool ExecuteInteraction(GameObject user);
}
