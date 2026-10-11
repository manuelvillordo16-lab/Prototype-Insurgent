using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;

    [Header("Ataques")]
    [SerializeField] private float lightAttackCooldown = 0.6f;
    [SerializeField] private float heavyAttackCooldown = 1.2f;

    private float lastLightAttackTime;
    private float lastHeavyAttackTime;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    // --- Ataque Rápido (Click Izquierdo) ---
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryLightAttack();
        }
    }

    // --- Ataque Pesado (Click Derecho) ---
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
        animator.SetTrigger("Attack");          // Trigger del ataque rápido
        Debug.Log("Ataque Rápido");
    }

    private void TryHeavyAttack()
    {
        if (Time.time - lastHeavyAttackTime < heavyAttackCooldown) return;

        lastHeavyAttackTime = Time.time;
        animator.SetTrigger("HeavyAttack");     // Trigger del ataque pesado
        Debug.Log("Ataque Pesado");
    }
}