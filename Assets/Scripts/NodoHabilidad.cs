using UnityEngine;
using UnityEngine.UI;

public enum Poder { Ninguno, Stun, PuertasNivel2, Regeneracion, DobleSalto, AtaquePesado, Critico}
public class NodoHabilidad : MonoBehaviour
{
    public int costo = 50;
    public Poder poder;
    public NodoHabilidad nodoPrevio;
    public Image linea;
    public bool comprado = false;
    public bool disponible = false;

    [Header("Colores (se cambian desde el Inspector)")]
    public Color colorBloqueado = Color.red;
    public Color colorDisponible = Color.yellow;
    public Color colorComprado = Color.green;

    Button boton;
    Image fondo;
    Outline marco;
    SkillTree arbol;

    public void Iniciar(SkillTree a)
    {
        arbol = a;
        boton = GetComponent<Button>();
        fondo = GetComponent<Image>();
        marco = GetComponent<Outline>();
        
    }

    public void Click()
    {
        arbol.Comprar(this);
    }

    public void Actualizar(bool arbolAbierto, int exp)
    {
        bool previoComprado = false;
        if (nodoPrevio == null) previoComprado = true;
        else if (nodoPrevio.comprado) previoComprado = true;

        disponible = arbolAbierto && previoComprado && !comprado;

        if (comprado) fondo.color = colorComprado;
        else if (disponible) fondo.color = colorDisponible;
        else fondo.color = colorBloqueado;

        boton.interactable = disponible && exp >= costo;

        if (marco != null) marco.enabled = disponible;
        if (linea != null)
        {
            if (previoComprado) linea.color = colorComprado;
            else if (disponible) linea.color = colorDisponible;
            else linea.color = colorDisponible;
        }
    }
    
  






}
