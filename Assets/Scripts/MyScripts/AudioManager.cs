using UnityEngine;
public enum SoundType
{
    none,
    Footsteps,
    Jump,
    Land,
    Hurt,
    shoot,
    reload,
    emptyShoot,
    Death
}

[System.Serializable]
public class AudioItem
{
    public SoundType soundType;
    public AudioClip[] clips;
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else instance.enabled = false;
    }
    public void PlayOneShootFromArray(AudioItem[] items, SoundType t)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].soundType == t) PlayOneShoot(items[i]);
        }
    }
    public void PlayOneShoot(AudioItem sound)
    {
        if (sound == null || sound.source == null || sound.clips.Length == 0) return;

        bool multipleClips = sound.clips.Length > 1;

        int n = 0;
        if (multipleClips) n = Random.Range(1, sound.clips.Length);

        AudioClip selectedClip = sound.clips[n];
        sound.source.PlayOneShot(selectedClip);

        if (multipleClips)
        {
            sound.clips[n] = sound.clips[0];
            sound.clips[0] = selectedClip;
        }
    }
}
