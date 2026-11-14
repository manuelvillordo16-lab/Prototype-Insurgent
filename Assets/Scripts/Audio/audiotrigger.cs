using UnityEngine;

public class audiotrigger : MonoBehaviour
{
    public MusicManager targetScript;
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            targetScript.ChangeClip();
            Debug.Log("cambió la pista de audio");
        }
    }
}
