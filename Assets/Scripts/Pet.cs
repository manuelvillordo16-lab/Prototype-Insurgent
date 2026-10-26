using UnityEngine;
using UnityEngine.AI; 
public class Pet: MonoBehaviour

{
    [Header("Components")]
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Movement")]
    [SerializeField] private float wanderRadius = 20f; 
    [SerializeField] private float waitTimeMin = 2f;    
    [SerializeField] private float waitTimeMax = 5f;    
    private float timer;
    private float currentWaitTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        currentWaitTime = Random.Range(waitTimeMin, waitTimeMax);
        timer = currentWaitTime;
    }

    void Update()
    {
    
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
               
                if (animator != null) animator.SetBool("IsWalking", false);

                timer += Time.deltaTime;

              
                if (timer >= currentWaitTime)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                    agent.SetDestination(newPos);

                    
                    if (animator != null) animator.SetBool("IsWalking", true);

                   
                    timer = 0;
                    currentWaitTime = Random.Range(waitTimeMin, waitTimeMax);
                }
            }
        }
    }

  
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}