using UnityEngine;
using UnityEngine.AI;

public class Enemy2 : MonoBehaviour
{
    public enum State { Guarding, Chase, Attack }
    public State currentState = State.Guarding;

    [Header("Patrol")]
    public Transform guardPoint;
    public float guardRadius = 10f;
    public float guardSpeed = 3.5f;   
    public float chaseSpeed = 6.0f;   

    [Header("Combat and Ranges")]
    public Transform player;
    public float detectionRange = 15f;
    public float meleeAttackRange = 2f;

    [Header("Firing System")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireCooldown = 4f;
    private float fireTimer = 0f;

    [Header("Attack")]
    public float meleeCooldown = 1.5f;
    private float meleeTimer = 0f;
    public float meleeDamage = 10f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
                agent.speed = chaseSpeed;
                agent.SetDestination(player.position);


                TryShootProjectile();


                if (distToPlayer <= meleeAttackRange)
                {
                    currentState = State.Attack;
                }
                else if (distToPlayer > detectionRange * 1.5f)
                {

                    currentState = State.Guarding;
                }
                break;

            case State.Attack:
                agent.speed = 0f;
                transform.LookAt(player);


                if (meleeTimer <= 0f)
                {
                    PerformMeleeAttack();
                    meleeTimer = meleeCooldown;
                }


                TryShootProjectile();


                if (distToPlayer > meleeAttackRange)
                {
                    currentState = State.Chase;
                }
                break;
        }
    }

    void GuardingBehavior()
    {
        agent.speed = guardSpeed;
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
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Debug.Log("¡Enemy2 fired a projectile!");
        }
        else
        {
            Debug.LogWarning("The Projectile or FirePoint still needs to be assigned on Enemy2");
        }
    }

    void PerformMeleeAttack()
    {
        Debug.Log("¡Enemy2 delivered a blow in close-quarters combat!");

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(meleeDamage);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, detectionRange);

       
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);

        Vector3 centerPosition = (guardPoint != null) ? guardPoint.position : transform.position;
        Gizmos.DrawWireSphere(centerPosition, guardRadius);

    }
}