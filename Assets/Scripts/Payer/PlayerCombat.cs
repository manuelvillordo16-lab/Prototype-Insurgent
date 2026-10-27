using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private SwordDamage swordDamage;

    [Header("Tiempos de la ventana de daño")]
    [SerializeField] private float lightDamageStart = 0.15f;   //When starts dealing damage
    [SerializeField] private float lightDamageEnd = 0.4f;     //When finishes dealing damage
    [SerializeField] private float heavyDamageStart = 0.25f;
    [SerializeField] private float heavyDamageEnd = 0.6f;

    [Header("Ataques")]
    [SerializeField] private float lightAttackCooldown = 0.6f;
    [SerializeField] private float heavyAttackCooldown = 1.2f;

    [Header("Daño")]
    [SerializeField] private float lightAttackDamage = 15f;
    [SerializeField] private float heavyAttackDamage = 35f;
    [SerializeField] private float criticalChance = 0.15f;
    [SerializeField] private float citicalMultiplier = 2f;

    private float lastLightAttackTime;
    private float lastHeavyAttackTime;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    // Light Attack
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryLightAttack();
        }
    }

    // Heavy Attack
    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryHeavyAttack();
        }
    }

    private void TryLightAttack()
    {
        if (Time.time - lastLightAttackTime < lightAttackCooldown) return;

        lastLightAttackTime = Time.time;
        animator.SetTrigger("Attack");          // Light Attack Trigger
        Debug.Log("Ataque Rápido");
    }

    private void TryHeavyAttack()
    {
        if (Time.time - lastHeavyAttackTime < heavyAttackCooldown) return;

        lastHeavyAttackTime = Time.time;
        animator.SetTrigger("HeavyAttack");     // Heavy Attack Trigger
        Debug.Log("Ataque Pesado");
    }

    public void EnableSwordDamage()
    {
        if (swordDamage != null)
            swordDamage.EnableDamage(false);
    }

    public void EnableSwordHeavyDamage()
    {
        if (swordDamage != null)
            swordDamage.EnableDamage(true);
    }

    public void DisableSwordDamage()
    {
        if (swordDamage != null)
            swordDamage.DisableDamage();
    }

    private void LightAttack()
    {
        if (swordDamage == null) return;
        animator.SetTrigger("Attack");
        StartCoroutine(DamageWindow(lightDamageStart, lightDamageEnd, false));
    }

    private void HeavyAttack()
    {
        if (swordDamage == null) return;
        animator.SetTrigger("HeavyAttack");
        StartCoroutine(DamageWindow(heavyDamageStart, heavyDamageEnd, true));
    }

    private System.Collections.IEnumerator DamageWindow(float start, float end, bool isHeavy)
    {
        yield return new WaitForSeconds(start);

        swordDamage.EnableDamage(isHeavy);

        yield return new WaitForSeconds(end - start);

        swordDamage.DisableDamage();
    }
}