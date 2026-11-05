
using UnityEngine;

public class RoomAudioTrigger : MonoBehaviour
{
    public AudioSource audioOffice;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioOffice.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioOffice.Stop();
        }
    }
}

