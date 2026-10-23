using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    public int exp = 100;
    public int costoJunior = 50;
    public int costoMid = 100;
    public int costoSenior = 150;
    public TextMeshProUGUI textoExp;

    public Button botonJunior;
    public Button botonMid;
    public Button botonSenior;

    public Image fondoJunior;
    public Image fondoMid;
    public Image fondoSenior;

    public GameObject panelSkills;
    public KeyCode teclaAbrir = KeyCode.K;
    bool menuAbierto = false;

    bool juniorComprada;
    bool midComprada;
    bool seniorComprada;
    void Start()
    {
        ActualizarPantalla();
        panelSkills.SetActive(false);
    }

    public void ComprarJunior()
    {
        if (!juniorComprada && exp >= costoJunior)
        {
            exp = exp - costoJunior;
            juniorComprada = true;
            ActualizarPantalla();
        }
    }

    public void ComprarMid()
    {
        if (!midComprada && exp >= costoMid)
        {
            exp = exp - costoMid;
            midComprada = true;
            ActualizarPantalla();
        }
    }

    public void ComprarSenior()
    {
        if (!seniorComprada && exp >= costoSenior)
        {
            exp = exp - costoSenior;
            seniorComprada = true;
            ActualizarPantalla();
        }

    }

    void ActualizarPantalla()
    {
        textoExp.text = "EXP" + exp;
        PintarTarjeta(fondoJunior, botonJunior, juniorComprada, true);
        PintarTarjeta(fondoMid, botonMid, midComprada, juniorComprada);
        PintarTarjeta(fondoSenior,botonSenior,seniorComprada,midComprada);
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
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    void CerrarMenu()
    {
        menuAbierto = false;
        panelSkills.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}