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

    [Header("Rendimiento")]
    [SerializeField] private float checkInterval = 0.2f;   // revisa 5 veces por segundo en vez de cada frame

    private bool alarmTriggered = false;
    private float nextCheckTime;
    private readonly Collider[] hitsBuffer = new Collider[8];   // se reutiliza, no genera basura

    public static event Action<Vector3> OnAlarmRaised;

    void Update()
    {
        if (alarmTriggered) return;

        if (Time.time < nextCheckTime) return;
        nextCheckTime = Time.time + checkInterval;

        DetectPlayerIn3DCone();
    }

    void DetectPlayerIn3DCone()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, viewRange, hitsBuffer, playerMask);

        for (int i = 0; i < count; i++)
        {
            Collider hit = hitsBuffer[i];

           
            Vector3 toTarget = hit.bounds.center - transform.position;
            float dist = toTarget.magnitude;
            if (dist < 0.01f) continue;
            Vector3 dir = toTarget / dist;

            
            Vector3 local = transform.InverseTransformDirection(dir);
            if (local.z <= 0f) continue;  

            float h = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;   // angulo horizontal
            float v = Mathf.Atan2(local.y, Mathf.Sqrt(local.x * local.x + local.z * local.z)) * Mathf.Rad2Deg;   // angulo vertical

            if (Mathf.Abs(h) > horizontalAngle / 2f || Mathf.Abs(v) > verticalAngle / 2f) continue;

            
            if (!Physics.Raycast(transform.position, dir, dist, obstacleMask))
            {
                
                TriggerAlarm(hit.transform.position);
                return;
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

        
        Vector3 upLeft = transform.rotation * Quaternion.Euler(-verticalAngle / 2f, -horizontalAngle / 2f, 0) * Vector3.forward * viewRange;
        Vector3 upRight = transform.rotation * Quaternion.Euler(-verticalAngle / 2f, horizontalAngle / 2f, 0) * Vector3.forward * viewRange;
        Vector3 downLeft = transform.rotation * Quaternion.Euler(verticalAngle / 2f, -horizontalAngle / 2f, 0) * Vector3.forward * viewRange;
        Vector3 downRight = transform.rotation * Quaternion.Euler(verticalAngle / 2f, horizontalAngle / 2f, 0) * Vector3.forward * viewRange;

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