using UnityEngine;
using UnityEngine.SceneManagement;

public class Ontrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("victory"))
        {
            SceneManager.LoadScene("VictoryScene");
        }
    }

}
