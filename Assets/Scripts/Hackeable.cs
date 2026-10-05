using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hackeable : MonoBehaviour
{
    private Renderer objectRenderer;
    private Color originalColor;
    private EnemyAI enemyAI;

    [SerializeField] private DoorMovement doorMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();  
        originalColor = objectRenderer.material.color;
        enemyAI= GetComponent<EnemyAI>();
    }

   public void Hack()
    {
        Debug.Log("Hack activado en: " + gameObject.name);

        if (doorMovement != null)
        {
            Debug.Log("Puerta Encontrada. Abriendo puerta. ");
            doorMovement.OpenDoor();
        }

        StartCoroutine(HackEffect());

    }
    private IEnumerator HackEffect()
    {
        objectRenderer.material.color = Color.green;
        if (enemyAI != null)
        {
            Debug.Log("EnemyAI encontrado. Aplicando stun. ");
            enemyAI.Stun(3f);
        }
       
        yield return new WaitForSeconds(3f);

        objectRenderer.material.color = originalColor;

        if (doorMovement != null)
        {
            Debug.Log("Hack terminado. Cerrando puerta. ");
            doorMovement.CloseDoor();
        }
    }
}
