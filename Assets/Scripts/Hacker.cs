using UnityEngine;

public class Hacker : MonoBehaviour
{
    public Camera playerCamera;
    public float duracionHack = 6f;
    public int nivelPuertas = 0;
    public bool stunDesbloqueado = false;
    public bool camHackDesbloqueado = false;

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
                    Debug.Log("Hackeable encontrado: " + hackeable.gameObject.name);
                    hackeable.Hack(duracionHack, nivelPuertas, stunDesbloqueado, camHackDesbloqueado);
                }
                else
                {
                    Debug.Log("El objeto detectado no tiene Hackeable en sus padres");
                }

            }
        }
    }
}
