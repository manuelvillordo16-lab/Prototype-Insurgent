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
    bool hackeoDesbloqueado = false;

    void Start()
    {
        hacker.enabled = false;
        hacker.duracionHack = 3f;
        hacker.nivelPuertas = 0;
        hacker.stunDesbloqueado = false;
        playerMovement.dobleSaltoDesbloqueado = false;
        playerMovement.tripleSaltoDesbloqueado = false;
      
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
        if (poder == Poder.PuertasNivel1)
        {
            hackeoDesbloqueado = true;
            hacker.nivelPuertas = 1;
        }

        if (poder == Poder.DobleSalto)
        {
            playerMovement.dobleSaltoDesbloqueado = true;
        }

        if (poder == Poder.Stun)
        {
            hacker.stunDesbloqueado = true;
            hackeoDesbloqueado = true;
        }

        if (poder == Poder.PuertasNivel2)
        {
            hacker.nivelPuertas = 2;
        }
        
        if (poder == Poder.StunMejorado)
        {
            hacker.duracionHack = 6f;
        }

        if (poder == Poder.TripleSalto)
        {
            playerMovement.tripleSaltoDesbloqueado = true;
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
        menuAbierto = true;
        panelSkills.SetActive(true);
        hacker.enabled = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    void CerrarMenu()
    {
        menuAbierto = false;
        panelSkills.SetActive(false);
        hacker.enabled = hackeoDesbloqueado;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}