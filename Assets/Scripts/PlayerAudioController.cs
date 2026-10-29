using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AudioSource audioSource;

    [Header("Voices and Sounds of the Player")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip stepSound;
    [SerializeField] private AudioClip playerVoiceDamage;
    [SerializeField] private AudioClip playerAttackSound;
    [SerializeField] private AudioClip playerDeathSound;

    public void PlayJumpSound()
    {
        if (audioSource != null && jumpSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(jumpSound);
        }
    }

    public void PlayStepSound()
    {
        if (audioSource != null && stepSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(stepSound, 0.5f);
        }
    }

    public void PlayPlayerDamageVoice()
    {
        if (audioSource != null && playerVoiceDamage != null)
        {
            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(playerVoiceDamage);
        }
    }

    public void PlayPlayerAttack()
    {
        if (audioSource != null && playerAttackSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(playerAttackSound);
        }
    }

    public void PlayPlayerDeath()
    {
        if (audioSource != null && playerDeathSound != null)
        {
            audioSource.pitch = Random.Range(0.85f, 0.95f); 
            audioSource.PlayOneShot(playerDeathSound);
        }

    }
}