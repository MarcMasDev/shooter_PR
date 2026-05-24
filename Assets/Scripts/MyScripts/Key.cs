using UnityEngine;
using UnityEngine.Audio;
using System;

public class Key : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject keyVisuals;
    public string uiKeyText;
    public string keyID;
    [SerializeField] private AudioSource audioSource;
    public static event Action<string> OnKeyCollected;
    public bool Interact(GameObject user)
    {
        OnKeyCollected?.Invoke(keyID); //Envia la ID a todas las listening doors
        GetComponent<Collider>().enabled = false;
        keyVisuals.SetActive(false);
        if (audioSource != null) audioSource.Play();
        return true;
    }

    public string GetInteractionText() => uiKeyText;
}