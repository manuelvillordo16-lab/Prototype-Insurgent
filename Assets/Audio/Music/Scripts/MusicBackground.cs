using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicBackground : MonoBehaviour
{
    [SerializeField] private AudioClip musica;
    [SerializeField] private float volumenFinal = 0.4f;
    [SerializeField] private float duracionFade = 2f; 

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = musica;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    
    public void Iniciar()
    {
        if (audioSource.isPlaying) return; 
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        audioSource.volume = 0f;
        audioSource.Play();

        float tiempo = 0f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, volumenFinal, tiempo / duracionFade);
            yield return null;
        }
        audioSource.volume = volumenFinal;
    }
}
