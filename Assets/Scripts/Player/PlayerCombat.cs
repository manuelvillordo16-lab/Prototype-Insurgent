using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float criticalMultiplier = 2f;

    [Header("Audio")]
    [SerializeField] private PlayerAudioController audioController;
    [Header("Ataque Especial - Tornado Kick")]
    [SerializeField] private float specialDamage = 9999f;        
    [SerializeField] private float sequenceTimeout = 1.2f;    
    [SerializeField] private float specialCooldown = 1f;
    [SerializeField] private LegDamage legDamage;

    private float lastLightAttackTime;
    private float lastHeavyAttackTime;
    private float lastSpecialTime;
    private List<string> currentSequence = new List<string>();
    private float lastInputTime;
    private bool specialJustActivated = false;
    private float CalculateDamage(bool isHeavy)
    {
        float baseDamage = isHeavy ? heavyAttackDamage : lightAttackDamage;

        // Crit chance
        if (Random.value <= criticalChance)
        {
            baseDamage *= criticalMultiplier;
            Debug.Log("¡Golpe crítico!");
        }

        return baseDamage;
    }

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
            RegisterInput("Attack");

            if (specialJustActivated)
            {
                specialJustActivated = false;
                return;
            }
            TryLightAttack();
        }
    }

    // Heavy Attack
    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RegisterInput("Attack");
            TryHeavyAttack();
        }
    }

    private void TryLightAttack()
    {
        if (Time.time - lastLightAttackTime < lightAttackCooldown) return;

        lastLightAttackTime = Time.time;
        animator.SetTrigger("Attack");
        StartCoroutine(DamageWindow(lightDamageStart, lightDamageEnd, false));
        Debug.Log("Ataque Rápido");

        if (audioController != null)
            audioController.PlayPlayerAttack();
    }

    private void TryHeavyAttack()
    {
        if (Time.time - lastHeavyAttackTime < heavyAttackCooldown) return;

        lastHeavyAttackTime = Time.time;
        animator.SetTrigger("HeavyAttack");
        StartCoroutine(DamageWindow(heavyDamageStart, heavyDamageEnd, true));
        Debug.Log("Ataque Pesado");

        if (audioController != null)
            audioController.PlayPlayerHeavyAttack();
    }

   
    private System.Collections.IEnumerator DamageWindow(float start, float end, bool isHeavy)
    {
        yield return new WaitForSeconds(start);

        float finalDamage = CalculateDamage(isHeavy);
        swordDamage.EnableDamage(finalDamage);

        yield return new WaitForSeconds(end - start);

        swordDamage.DisableDamage();
    }
    private void RegisterInput(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        if (Time.time - lastInputTime > sequenceTimeout)
            currentSequence.Clear();

        lastInputTime = Time.time;

        // Avoids same reptead input often
        if (currentSequence.Count > 0 && currentSequence[^1] == input)
            return;

        currentSequence.Add(input);

        if (currentSequence.Count > 4)
            currentSequence.RemoveAt(0);

        Debug.Log("Secuencia actual: " + string.Join(" → ", currentSequence)); // Important for debug

        CheckSpecialSequence();
    }

    private void CheckSpecialSequence()
    {
        if (currentSequence.Count < 4) return;

        // Sequence: S → SA → A → Attack
        if (currentSequence[0] == "S" &&
            currentSequence[1] == "SA" &&
            currentSequence[2] == "A" &&
            currentSequence[3] == "Attack")
        {
            TryTornadoKick();
            currentSequence.Clear();
        }
    }

    private void TryTornadoKick()
    {
        if (Time.time - lastSpecialTime < specialCooldown) return;

        lastSpecialTime = Time.time;
        specialJustActivated = true;

        animator.SetTrigger("TornadoKick");
        Debug.Log("¡TORNADO KICK ACTIVADO!");

        if (audioController != null)
            audioController.PlayPlayerTornadoKick();

        StartCoroutine(SpecialDamageWindow(0.15f, 0.75f));
    }
    private IEnumerator SpecialDamageWindow(float start, float end)
    {
        yield return new WaitForSeconds(start);

        if (legDamage != null)
            legDamage.EnableDamage(specialDamage);   // Activates damage that It's Over 9000!!!

        yield return new WaitForSeconds(end - start);

        if (legDamage != null)
            legDamage.DisableDamage();
    }
    public void RegisterDirectionalInput(string dir)
    {
        RegisterInput(dir);
    }
}