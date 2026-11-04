using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleScreen : MonoBehaviour
{
    [Header("Fondo")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Paneles")]
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Opciones")]
    [SerializeField] private Slider volumeSlider;

    
    private const string VolumeKey = "MasterVolume";

    private void Start()
    {
        
        Time.timeScale = 0f;

        
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
          
            volumeSlider.SetValueWithoutNotify(savedVolume);
        }
    }

    
    public void OnStartPressed()
    {
        videoPlayer.Stop();
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    
    public void OnCreditsPressed()
    {
        creditsPanel.SetActive(true);
    }

    public void OnCreditsClosed()
    {
        creditsPanel.SetActive(false);
    }

    
    public void OnOptionsPressed()
    {
        optionsPanel.SetActive(true);
    }

    public void OnOptionsClosed()
    {
        
        PlayerPrefs.Save();
        optionsPanel.SetActive(false);
    }

    
    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;          
        PlayerPrefs.SetFloat(VolumeKey, value);
    }

    
    public void OnExitPressed()
    {
#if UNITY_EDITOR
        
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}