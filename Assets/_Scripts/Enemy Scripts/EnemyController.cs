using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Patrol, Suspect, Chase }

    [Header("State Monitoring")]
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float waitTime = 2f;
    public float moveSpeed = 3.5f;

    [Header("Detection Settings")]
    public float viewRadius = 15f;
    public float immediateChaseRadius = 5f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public LayerMask obstacleMask;
    public float suspectToChaseLapse = 1.5f;

    [Header("Dynamic Detection Settings")]
    public float sprintChaseRadiusMultiplier = 1.5f;  
    public float sneakChaseRadiusMultiplier = 0.5f;  
    private float baseImmediateChaseRadius;          
    private SimplePlayerController playerController; 

    [Header("Suspect Settings")]
    public float suspectSpeed = 4.5f;
    public float investigateTime = 4f;

    [Header("Chase Settings")]
    public float chaseSpeed = 6f;
    public float loseSightTime = 3f;

    [Header("Attack Settings")]
    public float attackDistance = 2f;
    public float attackInterval = 1.5f;
    public int attackDamage = 34;

    [Header("Debug Settings")]
    public bool showDebugGizmos = true;

    private NavMeshAgent agent;
    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private int currentPointIndex = 0;
    private float waitTimer;
    private float investigateTimer;
    private float spotTimer = 0f;
    private float attackTimer = 0f;
    private float loseSightTimer = 0f;
    private Vector3 lastKnownPlayerPos;
    private bool hasReachedSuspectPos = false;
    private Quaternion lookLeft;
    private Quaternion lookRight;
    private bool isLookingLeft = true;
    private float lookTimer = 0f;
    private EnemyState previousState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        previousState = currentState;
        attackTimer = attackInterval;
        Debug.Log("Enemy starts in state: " + currentState);

        baseImmediateChaseRadius = immediateChaseRadius;
     
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();

            playerController = playerObj.GetComponent<SimplePlayerController>();
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[0].position;
        }
    }

    void Update()
    {
        if (currentState != previousState)
        {
            Debug.Log("Enemy state changed to: " + currentState);
            previousState = currentState;
        }

        if (attackTimer < attackInterval)
        {
            attackTimer += Time.deltaTime;
        }

        UpdateRadiusAndListen();

        switch (currentState)
        {
            case EnemyState.Patrol:
                LookForPlayer();
                PatrolLogic();
                break;

            case EnemyState.Suspect:
                LookForPlayer();
                SuspectLogic();
                break;

            case EnemyState.Chase:
                ChaseLogic();
                break;
        }
    }

    void UpdateRadiusAndListen()
    {
        if (playerController == null || playerTransform == null) return;

        // change immediateChaseRadius by player statement
        switch (playerController.CurrentState)
        {
            case SimplePlayerController.PlayerState.Sprinting:
                immediateChaseRadius = baseImmediateChaseRadius * sprintChaseRadiusMultiplier;
                //If the enemy has not yet entered pursuit mode and the player runs within the expanded area
                //it will directly trigger the suspect to proceed with reconnaissance
                if (currentState != EnemyState.Chase)
                {
                    float dist = Vector3.Distance(transform.position, playerTransform.position);
                    if (dist <= immediateChaseRadius)
                    {
                        // in suspect
                        StartSuspect(playerTransform.position);
                    }
                }
                break;

            case SimplePlayerController.PlayerState.Sneaking:
                immediateChaseRadius = baseImmediateChaseRadius * sneakChaseRadiusMultiplier;
                break;

            default: 
                immediateChaseRadius = baseImmediateChaseRadius;
                break;
        }
    }

    void PatrolLogic()
    {
        if (!agent.isOnNavMesh) return;

        agent.isStopped = false;
        agent.speed = moveSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                GoToNextPoint();
                waitTimer = 0f;
            }
        }
    }

    void SuspectLogic()
    {
        agent.isStopped = false;
        agent.speed = suspectSpeed;

        if (!hasReachedSuspectPos)
        {
            agent.destination = lastKnownPlayerPos;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                hasReachedSuspectPos = true;
                investigateTimer = 0f;
                lookTimer = 0f;

                lookLeft = transform.rotation * Quaternion.Euler(0, -45, 0);
                lookRight = transform.rotation * Quaternion.Euler(0, 45, 0);
                isLookingLeft = true;
            }
        }
        else
        {
            investigateTimer += Time.deltaTime;
            lookTimer += Time.deltaTime;

            if (lookTimer >= 1f)
            {
                isLookingLeft = !isLookingLeft;
                lookTimer = 0f;
            }

            Quaternion targetRotation = isLookingLeft ? lookLeft : lookRight;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);

            if (investigateTimer >= investigateTime)
            {
                currentState = EnemyState.Patrol;
                GoToNextPoint();
            }
        }
    }

    void ChaseLogic()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (CanSeePlayer())
        {
            loseSightTimer = 0f;
            lastKnownPlayerPos = playerTransform.position;
        }
        else
        {
            loseSightTimer += Time.deltaTime;
        }

        if (loseSightTimer >= loseSightTime)
        {
            agent.isStopped = false;
            currentState = EnemyState.Patrol;
            GoToNextPoint();
            loseSightTimer = 0f;
            return;
        }

        if (distanceToPlayer <= attackDistance)
        {
            agent.isStopped = true;

            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
            dirToPlayer.y = 0;
            if (dirToPlayer != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirToPlayer), Time.deltaTime * 5f);
            }

            if (attackTimer >= attackInterval)
            {
                Debug.Log("Under attack,-1 health");
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
                attackTimer = 0f;
            }
        }
        else
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            agent.destination = lastKnownPlayerPos;
        }
    }

    bool CanSeePlayer()
    {
        if (playerTransform == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > viewRadius) return false;

        Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
        {
            if (!Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distanceToPlayer, obstacleMask))
            {
                return true;
            }
        }
        return false;
    }

    void LookForPlayer()
    {
        if (playerTransform == null) return;

        bool canSeePlayer = CanSeePlayer();
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (canSeePlayer)
        {
            if (distanceToPlayer < immediateChaseRadius)
            {
                StartChase();
            }
            else
            {
                if (currentState == EnemyState.Patrol)
                {
                    StartSuspect(playerTransform.position);
                }
                else if (currentState == EnemyState.Suspect)
                {
                    lastKnownPlayerPos = playerTransform.position;
                    hasReachedSuspectPos = false;

                    spotTimer += Time.deltaTime;
                    if (spotTimer >= suspectToChaseLapse)
                    {
                        StartChase();
                        spotTimer = 0f;
                    }
                }
            }
        }
        else
        {
            spotTimer = 0f;
        }
    }

    void StartSuspect(Vector3 triggerPos)
    {
        currentState = EnemyState.Suspect;
        lastKnownPlayerPos = triggerPos;
        hasReachedSuspectPos = false;
        spotTimer = 0f;
    }

    void StartChase()
    {
        currentState = EnemyState.Chase;
        loseSightTimer = 0f;
    }

    void GoToNextPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        agent.destination = patrolPoints[currentPointIndex].position;
    }

    void OnDrawGizmosSelected()
    {
        if (this == null || transform == null) return;
        if (!showDebugGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, immediateChaseRadius);

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void ResetState()
    {
        transform.position = patrolPoints[0].position;
        currentState = EnemyState.Patrol;
        agent.ResetPath();
    }
}