using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatPanel : MonoBehaviour
{
    
    public void OnRestartPressed()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}