using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private SwordDamage swordDamage;

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
}