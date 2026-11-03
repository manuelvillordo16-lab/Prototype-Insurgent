using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class GlitchUI : MonoBehaviour
{
    private Material mat;

    void Awake()
    {
        RawImage img = GetComponent<RawImage>();
        mat = Instantiate(img.material);   
        img.material = mat;
    }

    void Update()
    {
        mat.SetFloat("_T", Time.unscaledTime);
    }

    void OnDestroy()
    {
        if (mat != null) Destroy(mat);
    }
}
