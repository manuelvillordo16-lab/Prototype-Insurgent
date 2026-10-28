using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Game Over")]
    [SerializeField] private string gameOverSceneName = "GameOver";
    [SerializeField] private float delayBeforeGameOver = 2.5f;

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
        IsDead = false;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        Debug.Log($"Player received {amount} of damage. Current Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    void Die()
    {
        if (IsDead) return;

        IsDead = true;
        Debug.Log("Player is dead. Loading game over scene...");

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (combat != null) combat.enabled = false;

        StartCoroutine(LoadGameOverAfterDelay());
    }
    private IEnumerator LoadGameOverAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeGameOver);
        SceneManager.LoadScene(gameOverSceneName);
    }


}
