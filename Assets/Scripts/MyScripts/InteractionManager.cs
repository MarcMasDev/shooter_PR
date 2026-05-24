using UnityEngine;
public interface IInteractable
{
    bool Interact(GameObject user);
    string GetInteractionText();

}
public class InteractionManager : MonoBehaviour
{
    private IEntityInput input;
    private IInteractable currentInteractable;
    [SerializeField] private InteractUI interactUI;

    private void Awake()
    {
        input = GetComponent<IEntityInput>();
    }

    private void Update()
    {
        if (currentInteractable != null)
        {
            interactUI.Show(currentInteractable.GetInteractionText());

            if (input.IsInteracting)
            {
                //Intentamos interactuar y guardamos si funciona
                bool success = currentInteractable.Interact(gameObject);

                if (success)
                {
                    interactUI.Show(""); // Ocultar solo si funciona
                    currentInteractable = null;
                }
                else interactUI.ShowTooltip("Not available");
            }
        }
        else interactUI.Show(""); //hide

    }

    private void OnTriggerEnter(Collider other)
    {
        currentInteractable = other.GetComponent<IInteractable>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IInteractable>() == currentInteractable)
        {
            currentInteractable = null;
        }
    }
}
