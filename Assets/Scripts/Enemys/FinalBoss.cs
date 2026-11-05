using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FinalBoss : MonoBehaviour, IDamageable
{
    public enum BossState { Sleeping, Active }
    public BossState currentState = BossState.Sleeping;

    [Header("Two Bars Health")]
    [SerializeField] private float maxHealthBar1 = 100f;
    [SerializeField] private float maxHealthBar2 = 100f;
    [SerializeField] private float currentHealthBar1;
    [SerializeField] private float currentHealthBar2;
    private bool isSecondBarActive = false;

    [Header("Shield System")]
    public GameObject shieldVisual;
    private bool isShieldActive = false;
    private float nextShieldTriggerBar1;
    private float nextShieldTriggerBar2;
    private Coroutine shieldRoutine; 

    [Header("Spawns")]
    public Transform player;
    public GameObject enemy2Prefab;
    private bool hasSpawnedReinforcements = false;

    [Header("Attacks and Movement")]
    public float attackRange = 10f; 
    public float attackCooldown = 2f;
    private float attackTimer = 0f;
    private NavMeshAgent agent;

    [Header("Animation")]
    private Animator bossAnim;

    void Start()
    {
        bossAnim = GetComponentInChildren<Animator>();
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealthBar1 = maxHealthBar1;
        currentHealthBar2 = maxHealthBar2;

        nextShieldTriggerBar1 = maxHealthBar1 - 25f;
        nextShieldTriggerBar2 = maxHealthBar2 - 25f;

        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (agent != null) agent.enabled = false;
        if (shieldVisual != null) shieldVisual.SetActive(false);
    }

    void OnEnable()
    {
        BossRoomTrigger.OnBossRoomTriggered += WakeUpBoss;
    }

    void OnDisable()
    {
        BossRoomTrigger.OnBossRoomTriggered -= WakeUpBoss;
    }

    void WakeUpBoss()
    {
        currentState = BossState.Active;

        if (agent != null) agent.enabled = true;

        Debug.Log("¡The Final Boss is Awake!");

      
        ActivateShield(10f);
    }

    void Update()
    {
        if (currentState == BossState.Sleeping || agent == null || !agent.enabled || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        agent.SetDestination(player.position);

        if (bossAnim != null) bossAnim.SetBool("walking", true);

        if (dist <= attackRange)
        {
            agent.speed = 0f;
            transform.LookAt(player);

            if (attackTimer <= 0f)
            {
                PerformBossAttacks();

                
                ActivateShield(1f);

                attackTimer = attackCooldown;
            }
        }
        else
        {
            agent.speed = 3.5f;
        }

        attackTimer -= Time.deltaTime;
    }

    
    void ActivateShield(float duration)
    {
        if (shieldRoutine != null) StopCoroutine(shieldRoutine);
        shieldRoutine = StartCoroutine(ActivateShieldForSeconds(duration));
    }

    public void TakeDamage(float amount)
    {
        if (isShieldActive)
        {
            Debug.Log("¡The attack was blocked by the boss's shield!");
            return;
        }

        if (!isSecondBarActive)
        {
            currentHealthBar1 -= amount;
            Debug.Log("Boss Health Bar 1: " + currentHealthBar1);

            if (currentHealthBar1 <= nextShieldTriggerBar1 && currentHealthBar1 > 0f)
            {
                TriggerShieldByHealthLoss();
                nextShieldTriggerBar1 -= 25f;
            }

            if (currentHealthBar1 <= 0f)
            {
                isSecondBarActive = true;
                nextShieldTriggerBar2 = maxHealthBar2 - 25f;
                Debug.Log("¡The Boss has entered it's SECOND HEALTH BAR!");

                ActivateShield(5f);
            }
        }
        else
        {
            currentHealthBar2 -= amount;
            Debug.Log("Boss Health Bar 2: " + currentHealthBar2);

            if (currentHealthBar2 <= nextShieldTriggerBar2 && currentHealthBar2 > 0f)
            {
                TriggerShieldByHealthLoss();
                nextShieldTriggerBar2 -= 25f;
            }

            if (!hasSpawnedReinforcements && currentHealthBar2 <= (maxHealthBar2 - 25f))
            {
                SpawnBossMinions();
                hasSpawnedReinforcements = true;
            }

            if (currentHealthBar2 <= 0f)
            {
                BossDie();
            }
        }
    }

    void TriggerShieldByHealthLoss()
    {
        Debug.Log("¡Shield activated by health loss!");
        ActivateShield(5f);
    }

    IEnumerator ActivateShieldForSeconds(float duration)
    {
        isShieldActive = true;
        if (shieldVisual != null) shieldVisual.SetActive(true);

        Debug.Log("¡Shield activated!");

        yield return new WaitForSeconds(duration);

        isShieldActive = false;
        if (shieldVisual != null) shieldVisual.SetActive(false);
        shieldRoutine = null;

        Debug.Log("Shield deactivated");
    }

    void PerformBossAttacks()
    {
        int attackChoice = Random.Range(0, 2);
        if (attackChoice == 0)
        {
            Debug.Log("The boss is attacking the player");
            if (bossAnim != null) bossAnim.SetBool("attack", true);
        }
        else
        {
            Debug.Log("The boss is shooting");
            if (bossAnim != null) bossAnim.SetBool("attack", false);
        }
    }

    void SpawnBossMinions()
    {
        if (enemy2Prefab != null)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 offset = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
                Instantiate(enemy2Prefab, transform.position + offset, Quaternion.identity);
            }
            Debug.Log("¡The boss spawned three enemies!");
        }
    }

    void BossDie()
    {
        Debug.Log("¡The Final Boss has been defeated!");
        Destroy(gameObject);
    }
}
