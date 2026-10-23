using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    public int exp = 100;
    public int[] costosCuadros = {50,100,150};
    public int[] costosSenior = {200,250,300};
    public TextMeshProUGUI textoExp;
    public TextMeshProUGUI textoNivel;
    public Button botonJunior;
    public Button botonSenior;
    public Image fondoJunior;
    public Image fondoSenior;

    public GameObject panelJunior;
    public Button[] botonesCuadros;
    public Image[] fondosCuadros;
    public GameObject panelSenior;
    public Button[] botonesSenior;
    public Image[] fondosSenior;

    public GameObject panelSkills;
    public KeyCode teclaAbrir = KeyCode.K;
    bool menuAbierto = false;
    bool[] cuadrosComprados = new bool[3];
    bool[] seniorComprados = new bool[3]; 
    public Hacker hacker;

    void Start()
    {
        hacker.enabled = false;
        ActualizarPantalla();
        panelJunior.SetActive(false);
        panelSenior.SetActive(false);
        panelSkills.SetActive(false);
    }

    public void AbrirJunior()
    {
        panelJunior.SetActive(true);
    }

    public void CerrarJunior()
    {
        panelJunior.SetActive(false);
    }

    public void AbrirSenior()
    {
        if (JuniorCompleto()) panelSenior.SetActive(true);
    }

    public void CerrarSenior()
    {
        panelSenior.SetActive(false);
    }
    public void ComprarCuadro(int numero)
    {
        if (!cuadrosComprados[numero] && exp >= costosCuadros[numero])
        {
            exp = exp - costosCuadros[numero];
            cuadrosComprados[numero] = true;
            ActualizarPantalla();
        }
    }

    public void ComprarCuadroSenior(int numero)
    {
        if (JuniorCompleto() && !seniorComprados[numero] && exp >= costosSenior[numero])
        {
            exp = exp - costosSenior[numero];
            seniorComprados[numero] = true;
            ActualizarPantalla();
        }

    }
     
    bool JuniorCompleto()
    {
        for (int i = 0; i < cuadrosComprados.Length; i++)
        {
            if (!cuadrosComprados[i]) return false;
        }
        return true;

    }

    bool SeniorCompleto()
    {
        for (int i = 0; i < seniorComprados.Length; i++)
        {
            if (!seniorComprados[i]) return false;
        }
        return true;

    }

    int CuadrosCompradosCantidad()
    {
        int cantidad = 0;
        for (int i= 0; i < cuadrosComprados.Length; i++)
        {
            if (cuadrosComprados[i]) cantidad++;
        }
        for (int i= 0; i < seniorComprados.Length; i++)
        {
            if (seniorComprados[i]) cantidad++;
        }
        return cantidad;


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

    void PintarTarjeta(Image fondo, Button boton, bool comprada, bool disponible)
    {
        if (comprada)
        {
            fondo.color = Color.yellow;
            boton.interactable = false;
        }
        else if (disponible)
        {
            fondo.color = Color.green;
            boton.interactable = true;

        }
        else
        {
            fondo.color = Color.gray;
            boton.interactable = false;
        }

    }
    void Update()
    {
        if (Input.GetKeyDown(teclaAbrir))
        {
            if (menuAbierto) CerrarMenu();
            else AbrirMenu();
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