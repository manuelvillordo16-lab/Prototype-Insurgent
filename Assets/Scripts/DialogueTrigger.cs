using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Diálogo")]
    [SerializeField] private AudioClip[] dialogueClips;
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
        if (dialogueClips == null || dialogueClips.Length == 0) return;

        
        transform.SetParent(other.transform);
       
        transform.localPosition = Vector3.zero;

        PlayDialogue();
    }

    public void PlayDialogue()
    {
        if (dialogueClips == null || dialogueClips.Length == 0) return;

        StartCoroutine(PlayAudioListRoutine());

        hasPlayed = true;

        if (disableColliderAfterPlay)
        {
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }

    private IEnumerator PlayAudioListRoutine()
    {
        audioSource.volume = volume;

        foreach (AudioClip clip in dialogueClips)
        {
            if (clip == null) continue;

            audioSource.clip = clip;
            audioSource.Play();

            yield return new WaitForSeconds(clip.length);
        }

         transform.SetParent(null);
         enabled = false; 
    }
}
