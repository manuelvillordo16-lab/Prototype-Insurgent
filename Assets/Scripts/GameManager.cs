using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Playing,
        Paused,
        GameOver
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;

    [Header("Scenes")]
    [SerializeField] private string gameOverSceneName = "GameOver";
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    
    [Header("Victoria")]
    [SerializeField] private AudioClip[] victoryDialogueClips;
    [SerializeField] private string victorySceneName = "Victory";

    [SerializeField] private float delayBeforeVictoryScreen = 4f;

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton simple
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
       
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        Instance = this;
       
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                break;

            case GameState.GameOver:
                Time.timeScale = 1f;
                break;
        }
    }

    public void PlayerDied()
    {
        SetState(GameState.GameOver);

        Invoke(nameof(LoadGameOverScene), 2.5f);
    }

    public void LoadGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
   
    public void Victory()
    {
        SetState(GameState.GameOver);
        StartCoroutine(PlayVictorySequence());
    }
    private IEnumerator PlayVictorySequence()
    {

        if (victoryDialogueClips != null)
        {
            foreach (AudioClip clip in victoryDialogueClips)
            {
                if (clip == null) continue;

                audioSource.clip = clip;
                audioSource.Play();
                yield return new WaitForSeconds(clip.length + 0.15f);
            }
        }

        yield return new WaitForSeconds(delayBeforeVictoryScreen);
        SceneManager.LoadScene(victorySceneName);
    }
}