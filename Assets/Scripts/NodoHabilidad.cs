using UnityEngine;
using UnityEngine.UI;

public enum Poder { Ninguno, PuertasNivel1, DobleSalto, Stun, PuertasNivel2, TripleSalto, StunMejorado}
public class NodoHabilidad : MonoBehaviour
{
    public int costo = 50;
    public Poder poder;
    public NodoHabilidad nodoPrevio;
    public Image linea;
    public bool comprado = false;
    public bool disponible = false;

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

        if (comprado) fondo.color = Color.yellow;
        else if (disponible) fondo.color = Color.white;
        else fondo.color = Color.gray;

        boton.interactable = disponible && exp >= costo;

        if (marco != null) marco.enabled = disponible;
        if (linea != null)
        {
            if (previoComprado) linea.color = Color.yellow;
            else linea.color = Color.gray;
        }
    }
    
  






}
