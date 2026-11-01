using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))] 
public class DoorSliding : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private Vector3 direccionLocal = Vector3.left;
    [SerializeField] private float distancia = 2f;
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float tiempoAbierta = 2f;

    [Header("Sonidos")] 
    [SerializeField] private AudioClip sonidoAbrir;
    [SerializeField] private AudioClip sonidoCerrar;

    private Vector3 posCerrada;
    private Vector3 posAbierta;
    private bool enMovimiento;
    private AudioSource audioSource;
    public bool EnMovimiento => enMovimiento;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); 
        posCerrada = transform.localPosition;
        posAbierta = posCerrada + direccionLocal.normalized * distancia;
    }

  
    public bool Abrir()
    {
        if (enMovimiento) return false;
        StartCoroutine(Secuencia());
        return true;
    }

    private IEnumerator Secuencia()
    {
        enMovimiento = true;

        audioSource.PlayOneShot(sonidoAbrir);  
        yield return Mover(posAbierta);

        yield return new WaitForSeconds(tiempoAbierta);

        audioSource.PlayOneShot(sonidoCerrar);  
        yield return Mover(posCerrada);

        enMovimiento = false;
    }

    private IEnumerator Mover(Vector3 destino)
    {
        while ((transform.localPosition - destino).sqrMagnitude > 0.0001f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition, destino, velocidad * Time.deltaTime);
            yield return null;
        }
        transform.localPosition = destino;
    }
}