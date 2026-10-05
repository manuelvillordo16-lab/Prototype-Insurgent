using UnityEngine;
using System;

public class SurveillanceCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float viewRange = 15f;
    [Range(1, 180)]
    public float horizontalAngle = 90f;
    [Range(1, 180)]
    public float verticalAngle = 60f;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    private bool alarmTriggered = false;

   
    public static event Action<Vector3> OnAlarmRaised;

    void Update()
    {
        if (alarmTriggered) return;

        DetectPlayerIn3DCone();
    }

    void DetectPlayerIn3DCone()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, viewRange, playerMask);
        foreach (Collider hit in hits)
        {
            Transform player = hit.transform;
            Vector3 dirToPlayer = (player.position - transform.position);
            float distToPlayer = dirToPlayer.magnitude;
            dirToPlayer.Normalize();

            Vector3 horizontalDirToPlayer = new Vector3(dirToPlayer.x, 0, dirToPlayer.z);
            Vector3 horizontalForward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            float horizontalAngleToPlayer = Vector3.Angle(horizontalForward, horizontalDirToPlayer);

            float verticalAngleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);

            if (horizontalAngleToPlayer <= horizontalAngle / 2f && verticalAngleToPlayer <= verticalAngle / 2f)
            {
                
                if (!Physics.Raycast(transform.position, dirToPlayer, distToPlayer, obstacleMask))
                {
                    TriggerAlarm(player.position);
                    break;
                }
            }
        }
    }

    void TriggerAlarm(Vector3 playerPosition)
    {
        if (alarmTriggered) return;
        alarmTriggered = true;
        Debug.Log("Player detected");

        
        OnAlarmRaised?.Invoke(playerPosition);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1.0f, 1.0f, 0.3f); 

       
        Vector3 upLeft = Quaternion.Euler(-verticalAngle / 2f, -horizontalAngle / 2f, 0) * transform.forward * viewRange;
        Vector3 upRight = Quaternion.Euler(-verticalAngle / 2f, horizontalAngle / 2f, 0) * transform.forward * viewRange;
        Vector3 downLeft = Quaternion.Euler(verticalAngle / 2f, -horizontalAngle / 2f, 0) * transform.forward * viewRange;
        Vector3 downRight = Quaternion.Euler(verticalAngle / 2f, horizontalAngle / 2f, 0) * transform.forward * viewRange;

        
        Gizmos.DrawLine(transform.position, transform.position + upLeft);
        Gizmos.DrawLine(transform.position, transform.position + upRight);
        Gizmos.DrawLine(transform.position, transform.position + downLeft);
        Gizmos.DrawLine(transform.position, transform.position + downRight);

       
        Gizmos.DrawLine(transform.position + upLeft, transform.position + upRight);
        Gizmos.DrawLine(transform.position + upRight, transform.position + downRight);
        Gizmos.DrawLine(transform.position + downRight, transform.position + downLeft);
        Gizmos.DrawLine(transform.position + downLeft, transform.position + upLeft);
    }
}