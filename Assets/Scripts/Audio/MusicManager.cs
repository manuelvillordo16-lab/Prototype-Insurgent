using UnityEngine;
using UnityEditor;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource aS1, aS2;
    private float defaultVolume = 0.4f;
    private float transitionTime = 1.25F;
    
    public void ChangeClip()
    {
        AudioSource nowPlaying = aS1;
        AudioSource target = aS2;
        if (nowPlaying.isPlaying == false)
        {
           nowPlaying = aS2;
           target = aS1;
        }
     StartCoroutine(MixSources(nowPlaying, target));
    
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
