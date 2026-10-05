using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack }
    public State currentState = State.Patrol;

    public enum PatrolMode { Waypoints, RandomNavMesh }
    public PatrolMode patrolMode = PatrolMode.RandomNavMesh;

    public Spawner spawner;

    [Header("Patrol Mode")]
    public float randomPatrolRadius = 30f;

    [Header("References")]
    public Transform player;

    [Header("Movement Settings")]
    public float patrolSpeed = 20f;
    public float chaseSpeed = 25f;

    [Header("Attack Settings")]
    public float attackDistance = 5f;
    public float attackCooldown = 2f;

    [Header("Detection Settings")]
    public float chaseDistance = 40f;
    public float viewDistance = 50f;
    [Range(0, 360)]
    public float viewAngle = 120f;
    public LayerMask obstacleMask;

   
    private UnityEngine.AI.NavMeshAgent agent;
    private float attackTimer = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("¡No se encontró ningún objeto con la etiqueta 'Player' en la escena!");
            }
        }
    }


    void Start()
    {

        agent = GetComponent<NavMeshAgent>();

        agent.speed = patrolSpeed;

    }

    void OnEnable()
    {
        SurveillanceCamera.OnAlarmRaised += HandleAlarm;
    }


    void OnDisable()
    {
        SurveillanceCamera.OnAlarmRaised -= HandleAlarm;
    }


    void HandleAlarm(Vector3 targetPosition)
    {

        if (currentState == State.Patrol || currentState == State.Patrol)
        {
            agent.SetDestination(targetPosition);
            currentState = State.Chase;
            Debug.Log("Enemy its on it way towards the player");
        }
    }

    void Update()
    {
        if (player == null) return;

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase();
                break;

            case State.Attack:
                Attack();
                break;
        }

        attackTimer -= Time.deltaTime;
    }

    // PATROL 
    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && (!agent.hasPath || agent.remainingDistance < 0.3f))
        {
            Vector3 newPos = GetRandomNavMeshPosition();
            agent.SetDestination(newPos);
        }

        if (ShouldStartChasing())
        {
            float dist = Vector3.Distance(transform.position, player.position);


            if (dist <= attackDistance)
            {
                currentState = State.Attack;
            }
            else
            {
                currentState = State.Chase;
            }


            if (ShouldStartChasing())

                if (spawner != null)
                {
                    spawner.SpawnThreeEnemies();
                }

            currentState = State.Chase;
        }


    }


    //CHASE 
    void Chase()
    {
        agent.speed = chaseSpeed;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > chaseDistance)
        {
            currentState = State.Patrol;
            return;
        }

        agent.SetDestination(player.position);

        if (dist <= attackDistance)
        {
            currentState = State.Attack;
        }
    }

    //ATTACK
    void Attack()
    {
        agent.speed = 0f; // Stop moving while attacking

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > attackDistance || !PlayerInChaseRange())
        {
            currentState = State.Patrol;
            return;
        }

        if (!PlayerInViewRange())
        {
            currentState = State.Chase;
            return;
        }

        transform.LookAt(player);

        if (attackTimer <= 0f)
        {
            Debug.Log("Enemy attacked the player!");


            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {

                playerHealth.TakeDamage(10f);

            }
            else
            {

                Debug.LogError("¡The object assigned to the 'player' variable in EnemyAI does not have the PlayerHealth script!");

            }

            attackTimer = attackCooldown;
        }

    }

    //DETECTION 
    bool PlayerInChaseRange()
    {
        Vector3 dirToPlayer = player.position - transform.position;
        float dist = dirToPlayer.magnitude;

        if (dist > chaseDistance)
            return false;

        if (Physics.Raycast(transform.position, dirToPlayer.normalized, dist, obstacleMask))
            return false;

        return true;
    }

    bool PlayerInViewRange()
    {
        Vector3 dirToPlayer = player.position - transform.position;
        float dist = dirToPlayer.magnitude;

        if (dist > viewDistance)
            return false;

        dirToPlayer.Normalize();

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > viewAngle / 2f)
            return false;

        if (Physics.Raycast(transform.position, dirToPlayer, dist, obstacleMask))
            return false;

        return true;
    }

    bool ShouldStartChasing()
    {
        return PlayerInChaseRange() || PlayerInViewRange();
    }

    // NAVMESH PATROL
    Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * randomPatrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, randomPatrolRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    // RANGE
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }

    public void Stun(float duration)    
    {
        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        State previousState = currentState;
        agent.isStopped = true;
        yield return new WaitForSeconds(duration);
        agent.isStopped = false;
        currentState= previousState;



    }
}
