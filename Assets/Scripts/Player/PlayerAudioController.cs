using UnityEngine;

public class PlayerAudioController : MonoBehaviour

{
    private AudioSource audioSource;

    [Header("Voices and Sounds of the Player")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip stepSound;
    [SerializeField] private AudioClip playerVoiceDamage;
    [SerializeField] private AudioClip playerAttackSound;
    [SerializeField] private AudioClip playerHeavyAttackSound;
    [SerializeField] private AudioClip playerDeathSound;
    [SerializeField] private AudioClip playerTornadoKickSound;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("The AudioSource component is missing from the Player");
        }
    }

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

    public void PlayPlayerHeavyAttack()
    {
        if (audioSource != null && playerHeavyAttackSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.0f);
            audioSource.PlayOneShot(playerHeavyAttackSound);
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
    public void PlayPlayerTornadoKick()
    {
        if (audioSource != null && playerTornadoKickSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(playerTornadoKickSound);
        }
    }
}