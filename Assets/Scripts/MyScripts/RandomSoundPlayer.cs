using UnityEngine;

public class RandomSoundPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clips;
    public Vector2 delayRange = new Vector2(2f, 10f);

    float timer;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        if (clips.Length == 0) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            audioSource.PlayOneShot(clip);
            ResetTimer();
        }
    }

    void ResetTimer()
    {
        timer = Random.Range(delayRange.x, delayRange.y);
    }
}
