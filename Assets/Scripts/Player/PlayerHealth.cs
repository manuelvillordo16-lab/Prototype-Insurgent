using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead { get; private set; }
    [SerializeField] float bulletDamage;
    [SerializeField] float bossBulletDamage;

    [Header("Panel de derrota")]
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private float delayBeforeDefeatPanel = 2.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            currentHealth -= bulletDamage;
            Destroy(other.gameObject);
            Debug.Log(currentHealth);
        }
        else
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                currentHealth -= bossBulletDamage;
                Destroy(other.gameObject);
            }
        }
    }

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
        Debug.Log("Player is dead");

        PlayerAudioController playerAudio = GetComponent<PlayerAudioController>();
        if (playerAudio != null) playerAudio.PlayPlayerDeath();

        if (animator != null) animator.SetTrigger("Death");

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (combat != null) combat.enabled = false;

        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();

        StartCoroutine(ShowDefeatPanelAfterDelay());
    }

    private IEnumerator ShowDefeatPanelAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeDefeatPanel);

        if (defeatPanel != null)
            defeatPanel.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }
    

}
