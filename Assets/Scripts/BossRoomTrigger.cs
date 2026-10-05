using UnityEngine;
using System;
public class BossRoomTrigger : MonoBehaviour
{
 
    public static event Action OnBossRoomTriggered;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log(" ¡Player is on the Boss Room!");

            OnBossRoomTriggered?.Invoke();

            gameObject.SetActive(false);
        }
    }
}

