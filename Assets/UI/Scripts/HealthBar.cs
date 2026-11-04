using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerHealth playerHealth;      
    [SerializeField] private Image fillImage;                

    [Header("Estilo")]
    [SerializeField] private float fillSpeed = 2f;           
    [SerializeField] private Color colorFull = Color.green;
    [SerializeField] private Color colorLow = Color.red;

    private int lastShownHealth = -1;

    private void Start()
    {
        if (playerHealth == null || fillImage == null)
        {
            Debug.LogError("HealthBar: faltan referencias en el Inspector (PlayerHealth o Fill Image).");
            enabled = false;
            return;
        }

        
        fillImage.fillAmount = GetPercent();
        fillImage.color = Color.Lerp(colorLow, colorFull, fillImage.fillAmount);
        
    }

    private void Update()
    {
        float target = GetPercent();

        
        if (!Mathf.Approximately(fillImage.fillAmount, target))
        {
            
            fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, target, fillSpeed * Time.unscaledDeltaTime);
            fillImage.color = Color.Lerp(colorLow, colorFull, fillImage.fillAmount);
        }

        
    }

    private float GetPercent()
    {
        if (playerHealth.MaxHealth <= 0f) return 0f;
        return Mathf.Clamp01(playerHealth.CurrentHealth / playerHealth.MaxHealth);
    }

   
}