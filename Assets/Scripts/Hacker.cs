using UnityEngine;

public class Hacker : MonoBehaviour
{
    public Camera playerCamera;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Detecte: " + hit.collider.name);
                Hackeable hackeable = hit.collider.GetComponent<Hackeable>();

                if (hackeable != null)
                {
                    hackeable.Hack();
                }

            }
        }
    }
}
