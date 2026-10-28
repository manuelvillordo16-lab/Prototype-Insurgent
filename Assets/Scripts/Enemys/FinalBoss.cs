using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class FinalBoss : MonoBehaviour
{
    public enum BossState { Sleeping, Active }
    public BossState currentState = BossState.Sleeping;

    [Header("Two Bars Health")]
    public float maxHealthBar1 = 100f;
    public float maxHealthBar2 = 100f;
    private float currentHealthBar1;
    private float currentHealthBar2;
    private bool isSecondBarActive = false;

    [Header("Shield System")]
    public GameObject shieldVisual; 
    private bool isShieldActive = false;
    private float nextShieldHealthTrigger; 

    [Header("Spawns")]
    public Transform player;
    public GameObject enemy2Prefab; 
    private bool hasSpawnedReinforcements = false;

    [Header("Attacks and Movement")]
    public float attackRange = 50f;
    public float attackCooldown = 2f;
    private float attackTimer = 0f;
    private NavMeshAgent agent;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealthBar1 = maxHealthBar1;
        currentHealthBar2 = maxHealthBar2;

        nextShieldHealthTrigger = maxHealthBar1 - 25f;

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

        
        StartCoroutine(ActivateShieldForSeconds(10f));
    }
    void Update()
    {
        if (currentState == BossState.Sleeping || agent == null || !agent.enabled || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        agent.SetDestination(player.position);

        if (dist <= attackRange)
        {
            agent.speed = 0f;
            transform.LookAt(player);

            if (attackTimer <= 0f)
            {
                PerformBossAttacks();

               
                StartCoroutine(ActivateShieldForSeconds(3f)); 

                attackTimer = attackCooldown;
            }
        }
        else
        {
            agent.speed = 3.5f;
        }

        attackTimer -= Time.deltaTime;
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

            
            if (currentHealthBar1 <= nextShieldHealthTrigger)
            {
                TriggerShieldByHealthLoss();
            }

            if (currentHealthBar1 <= 0f)
            {
                isSecondBarActive = true;
                nextShieldHealthTrigger = maxHealthBar2 - 25f; 
                Debug.Log("¡The Boss has entered its  SECOND HEALTH BAR!");

                
                StartCoroutine(ActivateShieldForSeconds(5f));
            }
        }
        else
        {
            currentHealthBar2 -= amount;
            Debug.Log("Boss Health Bar 2: " + currentHealthBar2);

            
            if (currentHealthBar2 <= nextShieldHealthTrigger)
            {
                TriggerShieldByHealthLoss();
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
        Debug.Log("¡Shield activated!");
        StartCoroutine(ActivateShieldForSeconds(5f));
        nextShieldHealthTrigger -= 25f; 
    }

    IEnumerator ActivateShieldForSeconds(float duration)
    {
        isShieldActive = true;
        if (shieldVisual != null) shieldVisual.SetActive(true);

        Debug.Log("¡Shield activated!");

        yield return new WaitForSeconds(duration);

        isShieldActive = false;
        if (shieldVisual != null) shieldVisual.SetActive(false);

        Debug.Log("Shield deactivated");
    }

    void PerformBossAttacks()
    {
        int attackChoice = Random.Range(0, 2);
        if (attackChoice == 0)
        {
            Debug.Log("The boss is attacking the player");
        }
        else
        {
            Debug.Log("The boss is shooting");
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
