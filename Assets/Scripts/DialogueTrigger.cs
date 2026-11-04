using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Diálogo")]
    [SerializeField] private AudioClip dialogueClip;
    [SerializeField] private bool playOnlyOnce = true;
    [SerializeField] private float volume = 1f;

    [Header("Opcional")]
    [SerializeField] private bool disableColliderAfterPlay = true;

    private AudioSource audioSource;
    private bool hasPlayed = false;

    private void Awake()
    {

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (playOnlyOnce && hasPlayed) return;
        if (dialogueClip == null) return;

        PlayDialogue();
    }

    public void PlayDialogue()
    {
        if (dialogueClip == null) return;

        audioSource.clip = dialogueClip;
        audioSource.volume = volume;
        audioSource.Play();

        hasPlayed = true;

        if (disableColliderAfterPlay)
        {
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }
}