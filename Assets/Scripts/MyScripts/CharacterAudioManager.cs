using UnityEngine;

public class CharacterAudioManager : MonoBehaviour
{
    [SerializeField] private CharacterBlackboard m_StateBlackboard;

    [Header("Audio Clips")]
    [SerializeField] private AudioItem[] audioItems;

    private void OnEnable()
    {
        if (m_StateBlackboard != null)
        {
            m_StateBlackboard.OnFootstep += PlayFootstepSound;
            m_StateBlackboard.OnJump += PlayJumpSound;
            m_StateBlackboard.OnLand += PlayLandSound;
            m_StateBlackboard.OnHurt += PlayHurtSound;
            m_StateBlackboard.OnDeath += PlayDeathSound;
        }
    }

    private void OnDisable()
    {
        if (m_StateBlackboard != null)
        {
            m_StateBlackboard.OnFootstep -= PlayFootstepSound;
            m_StateBlackboard.OnJump -= PlayJumpSound;
            m_StateBlackboard.OnLand -= PlayLandSound;
            m_StateBlackboard.OnHurt -= PlayHurtSound;
            m_StateBlackboard.OnDeath -= PlayDeathSound;
        }
    }

    private void PlayFootstepSound() => AudioManager.instance.PlayOneShootFromArray(audioItems, SoundType.Footsteps);
    private void PlayJumpSound() => AudioManager.instance.PlayOneShootFromArray(audioItems, SoundType.Jump);
    private void PlayLandSound() => AudioManager.instance.PlayOneShootFromArray(audioItems, SoundType.Land);
    private void PlayHurtSound(Vector2 health, Vector2 shield) => AudioManager.instance.PlayOneShootFromArray(audioItems, SoundType.Hurt);
    private void PlayDeathSound() => AudioManager.instance.PlayOneShootFromArray(audioItems, SoundType.Death);
}
