using UnityEngine;
using UnityEngine.Video;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    private void Start()
    {
        Time.timeScale = 0f;              
       
    }

    public void OnStartPressed()
    {
        videoPlayer.Stop();               
        gameObject.SetActive(false);      
        Time.timeScale = 1f;             
        
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }
}