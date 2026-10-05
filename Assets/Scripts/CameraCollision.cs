using UnityEngine;
using Unity.Cinemachine;

public class CameraCollision : MonoBehaviour
{
    [Header("References")]
    public Transform target;                          
    public CinemachineThirdPersonFollow thirdPersonFollow;

    [Header("Collision")]
    public LayerMask collisionLayers;                 
    public float collisionRadius = 0.35f;
    public float minDistance = 1.2f;
    public float normalDistance = 3.5f;
    public float smoothTime = 8f;

    private float currentDistance;

    void Start()
    {
        if (thirdPersonFollow == null)
            thirdPersonFollow = GetComponent<CinemachineThirdPersonFollow>();

        currentDistance = normalDistance;
    }

    void LateUpdate()
    {
        if (target == null || thirdPersonFollow == null) return;

        // Character's direction to camera
        Vector3 direction = (transform.position - target.position).normalized;
        float desiredDistance = normalDistance;

        // SphereCast for wall detection
        if (Physics.SphereCast(target.position, collisionRadius, direction, out RaycastHit hit, normalDistance, collisionLayers))
        {
            desiredDistance = Mathf.Clamp(hit.distance - 0.15f, minDistance, normalDistance);
        }

        // Smoother distance changes
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * smoothTime);

        // Forces thrd person follow
        thirdPersonFollow.CameraDistance = currentDistance;
    }
}