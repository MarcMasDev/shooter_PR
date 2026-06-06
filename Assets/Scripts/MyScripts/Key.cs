using UnityEngine;
using UnityEngine.Audio;
using System;

public class Key : PurchasableInteractable
{
    [Header("Key Settings")]
    [SerializeField] private string keyID;
    [SerializeField] private AudioSource audioSource;

    public static event Action<string> OnKeyCollected;

    protected override bool ExecuteInteraction(GameObject user)
    {
        OnKeyCollected?.Invoke(keyID);
        if (audioSource != null) audioSource.Play();

        Destroy(gameObject);
        return true; // Return true because the key is fully consumed
    }
}