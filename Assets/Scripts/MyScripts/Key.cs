using UnityEngine;
using UnityEngine.Audio;
using System;

public class Key : PurchasableInteractable
{
    [Header("Key Settings")]
    [SerializeField] private GameObject keyVisuals;
    [SerializeField] private string keyID;
    [SerializeField] private AudioSource audioSource;

    public static event Action<string> OnKeyCollected;

    protected override bool ExecuteInteraction(GameObject user)
    {
        OnKeyCollected?.Invoke(keyID);

        GetComponent<Collider>().enabled = false;
        if (keyVisuals != null) keyVisuals.SetActive(false);
        if (audioSource != null) audioSource.Play();

        return true; // Return true because the key is fully consumed
    }
}