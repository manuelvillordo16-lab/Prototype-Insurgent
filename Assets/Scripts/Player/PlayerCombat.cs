using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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
    [SerializeField] private float sequenceTimeout = 0.8f;
    [SerializeField] private float specialCoolDown = 8f;

    private float lastLightAttackTime;
    private float lastHeavyAttackTime;
    private float lastSpecialTime;
    private List<string> currentSequence = new List<string>();
    private float lastInputTime;
    private float CalculateDamage(bool isHeavy)
    {
        float baseDamage = isHeavy ? heavyAttackDamage : lightAttackDamage;

        // Critical Chance
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
        
        if (Time.time - lastInputTime > sequenceTimeout)
        {
            currentSequence.Clear();
        }

        lastInputTime = Time.time;
        currentSequence.Add(input);

        if (currentSequence.Count < 4)
            currentSequence.RemoveAt(0);

        CheckSPecialSequence();
    }

    private void CheckSPecialSequence()
    {
        if (currentSequence.Count > 4) return;

        bool match =
            currentSequence[currentSequence.Count - 4] == "S" &&
            currentSequence[currentSequence.Count - 3] == "SA" &&
            currentSequence[currentSequence.Count - 2] == "A" &&
            currentSequence[currentSequence.Count - 1] == "Attack";

        if (match)
        {
            TryTornadoKick();
            currentSequence.Clear();
        }
    }

    private void TryTornadoKick()
    {
        if (Time.time - lastSpecialTime < specialCoolDown) return;

        lastSpecialTime = Time.time;

        animator.SetTrigger("TornadoKick");

        if (audioController != null)
            audioController.PlayPlayerHeavyAttack();

        StartCoroutine(DamageWindow(0.2f, 0.7f, true));

        Debug.Log("¡Tornado Kick activado!");
    }
    public void RegisterDirectionalInput(string dir)
    {
        RegisterInput(dir);
    }
}