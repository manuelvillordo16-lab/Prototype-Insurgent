using UnityEngine;
using UnityEditor;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource menu, background, bossBattle, victory, defeat;
    private float defaultVolume = 0.4f;
    private float transitionTime = 1.25F;
    private bool bossMusicInPlay  = false;

    public void StartGame()
    {
        AudioSource nowPlaying = menu;
        AudioSource target = background;
        
        StartCoroutine(MixSources(nowPlaying, target));
    
    }
  
    public void ChangeClip()
    {
        AudioSource nowPlaying = background;
        AudioSource target = bossBattle;

        StartCoroutine(MixSources(nowPlaying, target));
    }
  
    public void Victory()
    {
        AudioSource nowPlaying = bossBattle;
        AudioSource target = victory;

        StartCoroutine(MixSources(nowPlaying, target));
    }

    public void Defeat()
    {
       if (bossMusicInPlay ==false) 
       {
            AudioSource nowPlaying = background;
            AudioSource target = defeat;
            StartCoroutine(MixSources(nowPlaying, target));
       }
        else
        {
            AudioSource nowPlaying = bossBattle;
            AudioSource target = defeat;
            StartCoroutine(MixSources(nowPlaying, target));
        }
       
    }
   
    IEnumerator MixSources(AudioSource nowPlaying, AudioSource target)
    {
        float percentage = 0;
        while (nowPlaying.volume > 0)
        {
            nowPlaying.volume = Mathf.Lerp (defaultVolume,0,percentage);
            percentage += Time.deltaTime / transitionTime;
            yield return null;
        }

        nowPlaying.Pause();
        if (target.isPlaying == false)
            target.Play();
        target.UnPause();
        percentage = 0;

        while(target.volume < defaultVolume)
        {
            target.volume = Mathf.Lerp (0,defaultVolume,percentage);
            percentage += Time.deltaTime / transitionTime;
            yield return null;
        }
    }
}
