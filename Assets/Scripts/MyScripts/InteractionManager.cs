using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public interface IInteractable
{
    bool Interact(GameObject user);
    string GetInteractionText();

}
public class InteractionManager : MonoBehaviour
{
    private IInteractable currentInteractable;
    [SerializeField] private InteractUI interactUI;

    //New Input System
    private PlayerInput m_PlayerInput;
    private InputAction m_interact;

    //Seguimiento de todos los elementos interactivos que se encuentran dentro de la activación
    private List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private void Awake()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
        //Cache the input actions by name
        m_interact = m_PlayerInput.actions["Interact"];
    }
    private void Update()
    {
        UpdateClosestInteractable();

        if (m_interact.triggered && currentInteractable != null) HandleInteraction();

        if (currentInteractable != null) interactUI.Show(currentInteractable.GetInteractionText());
        else interactUI.Show("");
    }
    private void UpdateClosestInteractable()
    {
        if (nearbyInteractables.Count == 0)
        {
            currentInteractable = null;
            return;
        }

        IInteractable closest = null;
        float minDistance = float.MaxValue;
        Vector3 playerPosition = transform.position;

        for (int i = nearbyInteractables.Count - 1; i >= 0; i--)
        {
            MonoBehaviour interactableMono = nearbyInteractables[i] as MonoBehaviour;

            if (interactableMono == null)
            {
                nearbyInteractables.RemoveAt(i);
                continue;
            }

            float distanceSqr = (interactableMono.transform.position - playerPosition).sqrMagnitude;
            if (distanceSqr < minDistance)
            {
                minDistance = distanceSqr;
                closest = nearbyInteractables[i];
            }
        }

        currentInteractable = closest;
    }


    private void HandleInteraction()
    {
        print("interact");
        //Intentamos interactuar y guardamos si funciona
        bool success = currentInteractable.Interact(gameObject);

        if (success)
        {
            interactUI.Show(""); // Ocultar solo si funciona
            currentInteractable = null;
        }
        else interactUI.ShowTooltip("Not available");
    }
    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && !nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            nearbyInteractables.Remove(interactable);
        }
    }
}
