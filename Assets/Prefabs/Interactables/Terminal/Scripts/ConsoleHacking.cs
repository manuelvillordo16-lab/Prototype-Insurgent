using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class ConsoleHacking : MonoBehaviour
{
    [SerializeField] private GameObject canvasHackear;
    [SerializeField] private DoorSliding puerta;
    [SerializeField] private AudioClip sonidoHackeo;
    [SerializeField] private float demoraApertura = 2f; 

    private bool jugadorCerca;
    private bool hackeando; 
    private AudioSource audioSource;
    private WaitForSeconds espera; 

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        espera = new WaitForSeconds(demoraApertura); 
    }

    private void Start()
    {
        canvasHackear.SetActive(false);
    }

    private void Update()
    {
        if (jugadorCerca && !hackeando && !puerta.EnMovimiento
            && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(Hackear());
        }
    }

   
    private IEnumerator Hackear()
    {
        hackeando = true;

        audioSource.PlayOneShot(sonidoHackeo);
        yield return espera;
        puerta.Abrir();

        hackeando = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        jugadorCerca = true;
        canvasHackear.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        jugadorCerca = false;
        canvasHackear.SetActive(false);
    }
}