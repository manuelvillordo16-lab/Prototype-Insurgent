using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class EnemyAI : MonoBehaviour
{
    // Alert va al final para no cambiar los valores numericos de los estados que ya existen
    public enum State { Patrol, Chase, Attack, Alert }
    public State currentState = State.Patrol;

    public enum PatrolMode { Waypoints, RandomNavMesh }
    public PatrolMode patrolMode = PatrolMode.RandomNavMesh;

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

    [Header("Alert Settings")]
    public float alertSpeed = 25f;          // velocidad al ir hacia la alarma
    public float alertStopDistance = 2f;    // a que distancia del punto de alarma se considera que llego
    public float alertTimeout = 15f;        // si no llega en este tiempo, vuelve a patrullar

    [Header("Detection Settings")]
    public float chaseDistance = 40f;
    public float viewDistance = 50f;
    [Range(0, 360)]
    public float viewAngle = 120f;
    public LayerMask obstacleMask;


    private UnityEngine.AI.NavMeshAgent agent;
    private float attackTimer = 0f;
    private bool isStunned = false;
    private float alertTimer = 0f;

    [Header("Animation")]
    private Animator guardAnim;

    void Awake()
    {
        guardAnim = GetComponent<Animator>();
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


    // Se ejecuta cuando CUALQUIER camara dispara la alarma.
    // Cada enemigo decide solo si le corresponde acudir.
    void HandleAlarm(Vector3 cameraPosition, Vector3 alarmPosition, float radius)
    {
        if (isStunned) return;

        // Si ya esta persiguiendo o atacando, no lo distraemos
        if (currentState == State.Chase || currentState == State.Attack) return;

        // Solo acuden los enemigos cercanos a la camara
        if (Vector3.Distance(transform.position, cameraPosition) > radius) return;

        if (agent == null || !agent.isOnNavMesh) return;

        // El punto de alarma puede no estar justo sobre el NavMesh, buscamos el mas cercano
        NavMeshHit navHit;
        if (!NavMesh.SamplePosition(alarmPosition, out navHit, 5f, NavMesh.AllAreas)) return;

        agent.isStopped = false;
        agent.SetDestination(navHit.position);

        alertTimer = alertTimeout;
        currentState = State.Alert;

        Debug.Log(name + " acude a la alarma");
    }


    void Update()
    {
        if (player == null) return;

        if (isStunned) return;

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

            case State.Alert:
                Alert();
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
            guardAnim.SetBool("Chase", false);
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


        }


    }


    //CHASE 
    void Chase()
    {
        agent.speed = chaseSpeed;

        if (agent != null && agent.isOnNavMesh && player != null)
        {
            agent.isStopped = false;

            agent.SetDestination(player.position);
        }

        float dist = Vector3.Distance(transform.position, player.position);
        guardAnim.SetBool("Chase", true);

        if (dist > chaseDistance)
        {
            currentState = State.Patrol;
            return;
        }

        if (dist <= attackDistance)
        {
            currentState = State.Attack;
        }
    }

    //ALERT
    // Va hacia el punto donde la camara vio al jugador.
    // A diferencia de Chase, NO se cancela por chaseDistance: justamente
    // los enemigos que estan lejos son los que necesitan acudir.
    void Alert()
    {
        agent.speed = alertSpeed;

        if (guardAnim != null)
            guardAnim.SetBool("Chase", true);   // reutiliza la animacion de correr

        alertTimer -= Time.deltaTime;

        // Si en el camino ve al jugador, pasa a la persecucion normal
        if (PlayerInChaseRange())
        {
            currentState = State.Chase;
            return;
        }

        bool arrived = !agent.pathPending && agent.remainingDistance <= alertStopDistance;

        if (arrived || alertTimer <= 0f)
        {
            // Llego y no hay nadie: vuelve a patrullar
            currentState = State.Patrol;

            if (guardAnim != null)
                guardAnim.SetBool("Chase", false);
        }
    }

    //ATTACK
    void Attack()
    {
        agent.speed = 0f; // Stop moving while attacking

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > attackDistance || !PlayerInChaseRange())
        {
            if (PlayerInChaseRange())
            {
                currentState = State.Chase;
            }
            else
            {
                currentState = State.Patrol;
            }

            guardAnim.SetBool("Attack", false);
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


            PlayerHealth playerHealth = player.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {

                playerHealth.TakeDamage(10f);
                guardAnim.SetBool("Chase", false);
                guardAnim.SetBool("Attack", true);

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
        if (!isStunned)
        {
            StartCoroutine(StunCoroutine(duration));
        }

    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        if (guardAnim != null)
        {
            guardAnim.SetBool("Chase", false);
            guardAnim.SetBool("Attack", false);
        }
        Debug.Log("Enemigo aturdido ");

        yield return new WaitForSeconds(duration);

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }


        isStunned = false;

        if (PlayerInChaseRange())
        {
            currentState = State.Chase;

            if (guardAnim != null)
                guardAnim.SetBool("Chase", true);
        }
        else
        {
            currentState = State.Patrol;

            if (guardAnim != null)
                guardAnim.SetBool("Chase", false);
        }

        Debug.Log("Stun terminado, Enemigo recuperado ");



    }
}