using UnityEngine;

public class EnemyAudioController : MonoBehaviour

{

    private AudioSource audioSource;

    [Header("Configuration")]
    [Tooltip("Mark if this object is the (FinalBoss)")]
    [SerializeField] private bool isBoss = false;

    [Header("Voices and Attacks")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound; 
    [SerializeField] private AudioClip specialBossVoice;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("The AudioSource component is missing in " + gameObject.name);
        }
    }


    public void PlayAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            
            audioSource.pitch = isBoss ? Random.Range(0.8f, 0.95f) : Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void PlayDamageSound()
    {
        if (audioSource != null && damageSound != null)
        {
            audioSource.pitch = isBoss ? 0.85f : 1.0f;
            audioSource.PlayOneShot(damageSound);
        }
    }

    public void PlayDeathSound()
    {
        if (audioSource != null && deathSound != null)
        {
         
            audioSource.pitch = isBoss ? Random.Range(0.75f, 0.85f) : Random.Range(0.85f, 0.95f);
            audioSource.PlayOneShot(deathSound);
        }
    }


    public void PlayBossIntroVoice()
    {
        if (isBoss && audioSource != null && specialBossVoice != null)
        {
            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(specialBossVoice);
        }
    }
}