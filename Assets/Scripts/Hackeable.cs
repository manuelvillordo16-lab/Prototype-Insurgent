using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hackeable : MonoBehaviour
{
    public int nivelRequerido = 1;
    private Renderer objectRenderer;
    private Color originalColor;
    private EnemyAI enemyAI;

    private SurveillanceCamera surveillanceCamera;

    [SerializeField] private DoorMovement doorMovement;

    void Start()
    {
        objectRenderer = GetComponentInChildren<Renderer>();
       

        if (objectRenderer != null)
            originalColor = objectRenderer.material.color;

        enemyAI = GetComponent<EnemyAI>();
        surveillanceCamera = GetComponent<SurveillanceCamera>();
    }

   public void Hack(float duracion, int nivelPuertas, bool puedeStun, bool puedeHackearCamara)
    {
        bool abrePuerta = doorMovement != null && nivelPuertas >= nivelRequerido;
        bool aturde = enemyAI != null && puedeStun;
        bool desactivaCamara = surveillanceCamera != null && puedeHackearCamara;

        Debug.Log("EnemyAI encontrado: " + (enemyAI != null));
        Debug.Log("Stun desbloqueado: " + puedeStun);

        if (!abrePuerta && !aturde && !desactivaCamara)
        {
            Debug.Log("No tenes el nivel para hackear: " + gameObject.name);
            return;
        }

        Debug.Log("Hack activado en: " + gameObject.name + "duracion: " + duracion);

        if (abrePuerta)
        {
            Debug.Log("Abriendo puerta. ");
            doorMovement.OpenDoor();
        }

        if (desactivaCamara)
        {
            Debug.Log("Desactivando camara mediante hackeo");
            surveillanceCamera.Hack(duracion);
        }

        StartCoroutine(HackEffect(duracion, abrePuerta, aturde));

    }
    private IEnumerator HackEffect(float duracion, bool abrePuerta, bool aturde)
    {
        if (objectRenderer != null)
            objectRenderer.material.color = Color.green;

        if (aturde)
        {
            Debug.Log("Aplicando stun. ");
            enemyAI.Stun(duracion);
        }

        yield return new WaitForSeconds(duracion);

        if (objectRenderer != null)
            objectRenderer.material.color = originalColor;

        if (abrePuerta)
        {
            Debug.Log("Hack terminado. Cerrando puerta. ");
            doorMovement.CloseDoor();
        }


    }
}
