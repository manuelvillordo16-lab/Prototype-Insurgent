using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    public int exp = 100;
    public int[] costosCuadros = {50,100,150};
    public int costoSenior = 150;
    public TextMeshProUGUI textoExp;
    public Button botonJunior;
    public Button botonSenior;
    public Image fondoJunior;
    public Image fondoSenior;

    public GameObject panelJunior;
    public Button[] botonesCuadros;
    public Image[] fondosCuadros;

    public GameObject panelSkills;
    public KeyCode teclaAbrir = KeyCode.K;
    bool menuAbierto = false;
    bool[] cuadrosComprados = new bool[3];
    bool seniorComprada = false;
    public Hacker hacker;

    void Start()
    {
        hacker.enabled = false;
        ActualizarPantalla();
        panelJunior.SetActive(false);
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

    public void ComprarCuadro(int numero)
    {
        if (!cuadrosComprados[numero] && exp >= costosCuadros[numero])
        {
            exp = exp - costosCuadros[numero];
            cuadrosComprados[numero] = true;
            ActualizarPantalla();
        }
    }

    public void ComprarSenior()
    {
        if (JuniorCompleto() && !seniorComprada && exp >= costoSenior)
        {
            exp = exp - costoSenior;
            seniorComprada = true;
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

    void ActualizarPantalla()
    {
        textoExp.text = "EXP " + exp;
        
        for (int i = 0; i < cuadrosComprados.Length; i++)
        {
            PintarTarjeta(fondosCuadros[i], botonesCuadros[i], cuadrosComprados[i], exp >= costosCuadros[i]);
        }

        if (JuniorCompleto()) fondoJunior.color = Color.yellow;
        else fondoJunior.color = Color.green;
        botonJunior.interactable = true;

        PintarTarjeta(fondoSenior, botonSenior, seniorComprada, JuniorCompleto() && exp >= costoSenior);

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