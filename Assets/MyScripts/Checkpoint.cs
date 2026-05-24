using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    [HideInInspector] public bool validated = false;
    [HideInInspector] public List<float> lapsTimes = new List<float>();

    //Events
    public static event Action<Checkpoint> OnCheckpointReached;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !validated)
        {
            validated = true;
            OnCheckpointReached?.Invoke(this);
        }
    }
}
