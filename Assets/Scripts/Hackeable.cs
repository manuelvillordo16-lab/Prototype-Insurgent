using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hackeable : MonoBehaviour
{
    public int nivelRequerido = 1;
    private Renderer objectRenderer;
    private Color originalColor;
    private EnemyAI enemyAI;

    [SerializeField] private DoorMovement doorMovement;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();  
        originalColor = objectRenderer.material.color;
        enemyAI= GetComponent<EnemyAI>();
    }

   public void Hack(float duracion, int nivelPuertas, bool puedeStun)
    {
        bool abrePuerta = doorMovement != null && nivelPuertas >= nivelRequerido;
        bool aturde = enemyAI != null && puedeStun;

        if (!abrePuerta && !aturde)
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

        StartCoroutine(HackEffect(duracion, abrePuerta, aturde));

    }
    private IEnumerator HackEffect(float duracion, bool abrePuerta, bool aturde)
    {
        objectRenderer.material.color = Color.green;
        if (aturde)
        {
            Debug.Log("Aplicando stun. ");
            enemyAI.Stun(duracion);
        }
       
        yield return new WaitForSeconds(duracion);

        objectRenderer.material.color = originalColor;

        if (abrePuerta)
        {
            Debug.Log("Hack terminado. Cerrando puerta. ");
            doorMovement.CloseDoor();
        }
    }
}
