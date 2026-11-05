using UnityEngine;
using UnityEngine.AI;

public class Enemy2 : MonoBehaviour
{
    public enum State { Guarding, Chase, Attack }
    public State currentState = State.Guarding;

    [Header("Patrol Settings")]
    public Transform guardPoint;
    public float guardRadius = 10f;
    public float guardSpeed = 20f;
    public float chaseSpeed = 30f;

    [Header("Combat and Ranges (Configurables)")]
    public Transform player;
    public float detectionRange = 15f;
    public float fireRange = 7f;
    public float keepDistance = 4f;
    public float meleeAttackRange = 2f;  

    [Header("Firing System")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float aimHeight = 1.2f;
    public float fireCooldown = 4f;
    private float fireTimer = 0f;
   
    [Header("Melee Attack")]
    public float meleeCooldown = 1.5f;
    private float meleeTimer = 0f;
    public float meleeDamage = 10f;

    [Header("Optimization")]
    [SerializeField] private float repathInterval = 0.25f;
    private float repathTimer = 0f;

    private NavMeshAgent agent;
    private Animator robotAnim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        robotAnim = GetComponent<Animator>();
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        fireTimer -= Time.deltaTime;
        meleeTimer -= Time.deltaTime;
        repathTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Guarding:
                GuardingBehavior();

                if (distToPlayer <= detectionRange)
                {
                    currentState = State.Chase;
                }
                break;

            case State.Chase:
                agent.updateRotation = false;
                agent.speed = chaseSpeed;
                agent.stoppingDistance = keepDistance;
                robotAnim.SetBool("aiming", true);
                 
                FacePlayer();

                if (distToPlayer > keepDistance)
                {
                    agent.isStopped = false;
                    if (repathTimer <= 0f)
                    {
                        agent.SetDestination(player.position);
                        repathTimer = repathInterval;
                    }
                }
                else
                {
                    agent.isStopped = true;
                }
                if(distToPlayer <= fireRange)
                {
                    TryShootProjectile();
                }

                if (distToPlayer <= meleeAttackRange)
                {
                    currentState = State.Attack;
                }
                else if (distToPlayer > detectionRange * 1.5f)
                {
                    robotAnim.SetBool("aiming", false);
                    currentState = State.Guarding;
                }
                 break;

            case State.Attack:
                agent.speed = 0f;
                agent.stoppingDistance = 0f;
                transform.LookAt(player);

                
                TryShootProjectile();

                
                if (meleeTimer <= 0f)
                {
                    PerformMeleeAttack();
                    meleeTimer = meleeCooldown;
                }

                
                if (distToPlayer > meleeAttackRange)
                {
                    currentState = State.Chase;
                }
                break;
        }
    }

    void FacePlayer()
    {
        Vector3 flat = player.position - transform.position;
        flat.y = 0f;
        if (flat.sqrMagnitude < 0.001f) return;

        Quaternion target = Quaternion.LookRotation(flat);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, 10f * Time.deltaTime);
    }

    void GuardingBehavior()
    {
        agent.speed = guardSpeed;
        agent.stoppingDistance = 0f;
        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            Vector3 randomPoint = guardPoint != null ? guardPoint.position + Random.insideUnitSphere * guardRadius : transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, guardRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    void TryShootProjectile()
    {
        if (fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireCooldown;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null && firePoint == null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Debug.Log("¡Enemy2 fired a projectile!");
            robotAnim.SetTrigger("shoot");
        }
        else
        {
            Debug.LogWarning("The Projectile or FirePoint still needs to be assigned on Enemy2");
        }
    }

    void PerformMeleeAttack()
    {
        Debug.Log("¡Enemy2 delivered a blow in close quarters combat!");

        EnemyAudioController enemyAudio = GetComponent<EnemyAudioController>();
        if (enemyAudio != null)
        {
            enemyAudio.PlayAttackSound();
        }

        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                robotAnim.SetTrigger("kick");
                playerHealth.TakeDamage(meleeDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, fireRange);

       
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);

        
        Vector3 centerPosition = (guardPoint != null) ? guardPoint.position : transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(centerPosition, guardRadius);
    }
}