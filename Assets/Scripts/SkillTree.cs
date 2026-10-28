using TMPro;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public int exp = 100;
    public TextMeshProUGUI textoExp;
    public TextMeshProUGUI textoNivel;
    public NodoHabilidad[] nodosJunior;
    public NodoHabilidad[] nodosSenior;
    public GameObject candadoSenior;
    public GameObject panelSkills;
    public KeyCode teclaAbrir = KeyCode.K;
    bool menuAbierto = false; 
    public Hacker hacker;

    public PlayerMovement playerMovement;
    bool hackeoDesbloqueado = false;

    void Start()
    {
        hacker.enabled = false;
        playerMovement.dobleSaltoDesbloqueado = false;
        playerMovement.tripleSaltoDesbloqueado = false;
        Hackeable.duracionHack = 3f;
        for (int i = 0; i < nodosJunior.Length; i++) nodosJunior[i].Iniciar(this);
        for (int i = 0; i < nodosSenior.Length; i++) nodosSenior[i].Iniciar(this);

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
        switch (poder)
        {
            case Poder.Hackeo:
                hackeoDesbloqueado = true;
                break;

        }
    }

    void ActualizarPantalla()
    {
        textoExp.text = "EXP " + exp;
        textoNivel.text = "Nivel " + (1 + CuadrosCompradosCantidad());
        
        for (int i = 0; i < cuadrosComprados.Length; i++)
        {
            PintarTarjeta(fondosCuadros[i], botonesCuadros[i], cuadrosComprados[i], exp >= costosCuadros[i]);
        }

        for (int i = 0; i < seniorComprados.Length; i++)
        {
          PintarTarjeta(fondosSenior[i], botonesSenior[i], seniorComprados[i], JuniorCompleto() && exp >= costosSenior[i]);

        }

        
        if (JuniorCompleto()) fondoJunior.color = Color.yellow;

        else fondoJunior.color= Color.green;

        botonJunior.interactable = true;


        if (!JuniorCompleto())
        {
            fondoSenior.color = Color.gray;
            botonSenior.interactable = false;
        }
        else if (SeniorCompleto())
        {
            fondoSenior.color = Color.yellow;
            botonSenior.interactable = true;
        }
        else
        {
            fondoSenior.color = Color.green;
            botonSenior.interactable = true;
        }

        

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
        hacker.enabled = cuadrosComprados[0];
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}