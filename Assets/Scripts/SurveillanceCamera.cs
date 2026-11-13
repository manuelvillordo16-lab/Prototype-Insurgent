using UnityEngine;
using System;
using System.Collections;

// Recomendado: este script (y Hackeable) en el objeto RAIZ del prefab "Surveillance Camera".
// Si quedan en un hijo (ej. "Camera"), asignar la raiz en el campo Body Root.
public class SurveillanceCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float viewRange = 15f;
    [Range(1, 180)]
    public float horizontalAngle = 90f;
    [Range(1, 180)]
    public float verticalAngle = 60f;

    [Tooltip("Layer(s) donde esta el collider del jugador")]
    public LayerMask playerMask;
    [Tooltip("Paredes, puertas, etc. NO incluir la layer del jugador")]
    public LayerMask obstacleMask;

    [Header("Alarma")]
    [Tooltip("Los enemigos que esten a esta distancia de la camara acuden a la alarma")]
    public float alertRadius = 30f;
    [Tooltip("Segundos que tienen que pasar para que la camara pueda volver a disparar la alarma")]
    public float alarmCooldown = 5f;

    [Tooltip("Raiz del prefab de la camara (Surveillance Camera). Sus colliders no bloquean la vision. Si queda vacio usa este mismo objeto.")]
    [SerializeField] private Transform bodyRoot;

    [Header("Rendimiento")]
    [SerializeField] private float checkInterval = 0.2f;   // revisa 5 veces por segundo en vez de cada frame

    private float nextCheckTime;
    private float nextAlarmTime;
    private readonly Collider[] hitsBuffer = new Collider[8];   // se reutiliza, no genera basura
    private readonly RaycastHit[] rayBuffer = new RaycastHit[16];
    private bool isHacked = false;
    private Coroutine hackRoutine;

    // Parametros: (posicion de la camara, posicion del jugador, radio de alerta)
    public static event Action<Vector3, Vector3, float> OnAlarmRaised;

    void Awake()
    {
        if (bodyRoot == null) bodyRoot = transform;
    }

    void Update()
    {
        if (isHacked) return;
        if (Time.time < nextAlarmTime) return;   // todavia en cooldown despues de una alarma

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
            if (local.z <= 0f) continue;   // esta detras de la camara

            float h = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;   // angulo horizontal
            float v = Mathf.Atan2(local.y, Mathf.Sqrt(local.x * local.x + local.z * local.z)) * Mathf.Rad2Deg;   // angulo vertical

            if (Mathf.Abs(h) > horizontalAngle / 2f || Mathf.Abs(v) > verticalAngle / 2f) continue;

            if (HasLineOfSight(dir, dist, hit))
            {
                TriggerAlarm(hit.transform.position);
                return;
            }
        }
    }

    // Devuelve true si NO hay una pared entre la camara y el jugador.
    // Ignora los colliders de la propia camara (Base, Camera...) porque si no
    // se "bloquea" sola y nunca ve al jugador.
    bool HasLineOfSight(Vector3 dir, float dist, Collider target)
    {
        int n = Physics.RaycastNonAlloc(transform.position, dir, rayBuffer, dist, obstacleMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < n; i++)
        {
            if (rayBuffer[i].transform.IsChildOf(bodyRoot)) continue;    // parte de la propia camara
            if (rayBuffer[i].collider == target) continue;               // el propio jugador
            return false;                                                // choco con una pared
        }

        return true;
    }

    void TriggerAlarm(Vector3 playerPosition)
    {
        nextAlarmTime = Time.time + alarmCooldown;
        Debug.Log("Player detected");

        OnAlarmRaised?.Invoke(transform.position, playerPosition, alertRadius);
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

        // Radio en el que los enemigos escuchan la alarma
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, alertRadius);
    }

    public void Hack(float duration)
    {
        // Si ya estaba hackeada, reinicia el tiempo en vez de superponer dos corrutinas
        if (hackRoutine != null) StopCoroutine(hackRoutine);
        hackRoutine = StartCoroutine(HackCoroutine(duration));
    }

    private IEnumerator HackCoroutine(float duration)
    {
        isHacked = true;

        Debug.Log("Camara hackeada, Deteccion desactivada");

        yield return new WaitForSeconds(duration);

        isHacked = false;
        hackRoutine = null;

        Debug.Log("Camara recuperada, Deteccion activada");
    }
}