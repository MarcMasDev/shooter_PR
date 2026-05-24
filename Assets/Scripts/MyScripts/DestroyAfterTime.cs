using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float time = 5;
    private void Start()
    {
        Destroy(gameObject, time);
    }
}
