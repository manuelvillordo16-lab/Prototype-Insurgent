using TMPro;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public int exp = 100;
    public TextMeshProUGUI textoExp;
    public TextMeshProUGUI textoNivel;
    public NodoHabilidad[] nodos;
    public GameObject panelSkills;
    public KeyCode teclaAbrir = KeyCode.K;
    bool menuAbierto = false; 
    public Hacker hacker;

    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;
    public PlayerHealth playerHealth;
    bool regeneracionActiva = false;
    public float curacionPorSegundo = 10f;

    public MonoBehaviour CameraCollision; 

    void Start()
    {
        hacker.enabled = true;
        hacker.duracionHack = 3f;
        hacker.nivelPuertas = 0;
        hacker.stunDesbloqueado = false;
        playerMovement.dobleSaltoDesbloqueado = false;
        playerCombat.ataquePesadoDesbloqueado = false;
        playerCombat.criticoDesbloqueado = false;
      
        for (int i = 0; i < nodos.Length; i++) nodos[i].Iniciar(this);
        

        ActualizarPantalla();
        panelSkills.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaAbrir))
        {
            if (menuAbierto) CerrarMenu();
            else AbrirMenu();
        }

        if (regeneracionActiva)
        {
            playerHealth.Heal(curacionPorSegundo * Time.deltaTime);
        }
    }

    public void Comprar(NodoHabilidad nodo)
    {
        if (!nodo.disponible) return;
        if (exp < nodo.costo) return;

        exp = exp - nodo.costo;
        nodo.comprado = true;
        AplicarPoder(nodo.poder);
        ActualizarPantalla();
    }

    void AplicarPoder(Poder poder)
    {
       if (poder == Poder.Stun)
       {
            hacker.stunDesbloqueado = true;
       }
       if (poder == Poder.PuertasNivel2)
       {
            hacker.nivelPuertas = 2;
       }

       if (poder == Poder.Regeneracion)
       {
            regeneracionActiva = true;
       }

       if (poder == Poder.DobleSalto)
       {
            playerMovement.dobleSaltoDesbloqueado = true;
       }

       if (poder == Poder.AtaquePesado)
       {
            playerCombat.ataquePesadoDesbloqueado = true;
       }

       if (poder == Poder.Critico)
       {
            playerCombat.criticoDesbloqueado = true;
       }


        
    }

    int CantidadComprados()
    {
        int cantidad = 0;
        for (int i = 0; i < nodos.Length; i++)
        {
            if (nodos[i].comprado) cantidad++;
        }

        return cantidad;
    }

    void ActualizarPantalla()
    {
        textoExp.text = "EXP " + exp;
        textoNivel.text = "Nivel " + (1 + CantidadComprados());

        for (int i = 0; i < nodos.Length; i++) nodos[i].Actualizar(true, exp);
      
    }

    void AbrirMenu()
    {
        playerCombat.enabled = false; // nuevo
        menuAbierto = true;
        panelSkills.SetActive(true);
        hacker.enabled = false;
        CameraCollision.enabled = false;   // nuevo
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    void CerrarMenu()
    {
        playerCombat.enabled = true;
        menuAbierto = false;
        panelSkills.SetActive(false);
        hacker.enabled = true;
        CameraCollision.enabled = true;    // nuevo
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ExpDrop"))
        {
            exp++;
            textoExp.text = exp.ToString();
            Destroy(other.gameObject);
        }
    }


}



