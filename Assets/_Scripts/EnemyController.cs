using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    // Define the states for the Finite State Machine
    public enum EnemyState { Patrol, Suspect, Chase }

    [Header("State Monitoring")]
    public EnemyState currentState = EnemyState.Patrol; // Current state

    [Header("Patrol Settings")]
    public Transform[] patrolPoints; // Array of patrol waypoints
    public float waitTime = 2f;      // Time to wait at each waypoint
    public float moveSpeed = 3.5f;   // Walking speed

    [Header("Detection Settings")]
    public float viewRadius = 15f;        // How far the enemy can see
    public float immediateChaseRadius = 5f; // How close before immediate chase
    [Range(0, 360)]
    public float viewAngle = 90f;         // Field of view angle
    public LayerMask obstacleMask;        // Layers that block vision
    public float suspectToChaseLapse = 1.5f; // Time interval to trigger chase after suspecting

    [Header("Suspect Settings")]
    public float suspectSpeed = 4.5f;     // Speed when investigating
    public float investigateTime = 4f;    // Time spent looking around

    [Header("Chase Settings")]
    public float chaseSpeed = 6f;         // Speed when chasing
    public float giveUpDistance = 20f;    // Distance to stop chasing

    [Header("Attack Settings")]
    public float attackDistance = 2f;     // Distance required to attack the player
    public float attackInterval = 1.5f;   // Time between each attack

    [Header("Debug Settings")]
    public bool showDebugGizmos = true;   // Toggle debug lines

    // Internal variables
    private NavMeshAgent agent;
    private Transform playerTransform;    // Reference to the player
    private int currentPointIndex = 0;
    private float waitTimer;
    private float investigateTimer;
    private float spotTimer = 0f;         // Internal timer for spotting buffer
    private float attackTimer = 0f;       // Internal timer for attacking
    private Vector3 lastKnownPlayerPos;   // Where the player was last seen
    private bool hasReachedSuspectPos = false;
    private Quaternion lookLeft;
    private Quaternion lookRight;
    private bool isLookingLeft = true;
    private float lookTimer = 0f;
    private EnemyState previousState;     // Used to track state changes

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        // Initialize timers and states
        previousState = currentState;
        attackTimer = attackInterval; // Ready to attack immediately
        Debug.Log("Enemy starts in state: " + currentState);

        // Auto-find player by Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // Ensure there are patrol points assigned
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[0].position;
        }
    }

    void Update()
    {
        // Log state changes to the console
        if (currentState != previousState)
        {
            Debug.Log("Enemy state changed to: " + currentState);
            previousState = currentState;
        }

        // Update attack timer
        if (attackTimer < attackInterval)
        {
            attackTimer += Time.deltaTime;
        }

        // Core FSM Switch
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

    void PatrolLogic()
    {
        // Safety check if agent is not on NavMesh
        if (!agent.isOnNavMesh) return;

        agent.isStopped = false; // Ensure agent can move
        agent.speed = moveSpeed;

        // Check if the agent has reached the destination
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;

            // Move to the next point after waiting
            if (waitTimer >= waitTime)
            {
                GoToNextPoint();
                waitTimer = 0f;
            }
        }
    }

    void SuspectLogic()
    {
        agent.isStopped = false; // Ensure agent can move
        agent.speed = suspectSpeed;

        if (!hasReachedSuspectPos)
        {
            // Move to the last known position
            agent.destination = lastKnownPlayerPos;

            // Check if reached
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                hasReachedSuspectPos = true;
                investigateTimer = 0f;
                lookTimer = 0f;

                // Calculate rotation targets
                lookLeft = transform.rotation * Quaternion.Euler(0, -45, 0);
                lookRight = transform.rotation * Quaternion.Euler(0, 45, 0);
                isLookingLeft = true;
            }
        }
        else
        {
            // Reached position, now look around
            investigateTimer += Time.deltaTime;
            lookTimer += Time.deltaTime;

            // Alternate looking left and right
            if (lookTimer >= 1f)
            {
                isLookingLeft = !isLookingLeft;
                lookTimer = 0f;
            }

            Quaternion targetRotation = isLookingLeft ? lookLeft : lookRight;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);

            // Go back to patrol if time is up
            if (investigateTimer >= investigateTime)
            {
                currentState = EnemyState.Patrol;
                GoToNextPoint();
            }
        }
    }

    void ChaseLogic()
    {
        // Safety check if player is missing
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Give up chasing if player is too far
        if (distanceToPlayer > giveUpDistance)
        {
            agent.isStopped = false;
            currentState = EnemyState.Patrol;
            GoToNextPoint();
            return;
        }

        // Attack logic
        if (distanceToPlayer <= attackDistance)
        {
            agent.isStopped = true; // Stop moving to attack

            // Face the player
            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
            dirToPlayer.y = 0; // Keep rotation horizontal
            if (dirToPlayer != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirToPlayer), Time.deltaTime * 5f);
            }

            // Execute attack if timer is ready
            if (attackTimer >= attackInterval)
            {
                Debug.Log("Under attack,-1 health");
                attackTimer = 0f; // Reset timer after attack
            }
        }
        else
        {
            // Continue moving towards player
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            agent.destination = playerTransform.position;
        }
    }

    void LookForPlayer()
    {
        if (playerTransform == null) return;

        Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        bool canSeePlayer = false;

        // Check if player is within view radius
        if (distanceToPlayer < viewRadius)
        {
            // Check if player is within view angle
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                // Check if there are obstacles
                if (!Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distanceToPlayer, obstacleMask))
                {
                    canSeePlayer = true;
                }
            }
        }

        if (canSeePlayer)
        {
            // Immediate chase if player is very close
            if (distanceToPlayer < immediateChaseRadius)
            {
                StartChase();
            }
            else
            {
                // Buffer logic for player further away
                if (currentState == EnemyState.Patrol)
                {
                    StartSuspect(playerTransform.position);
                }
                else if (currentState == EnemyState.Suspect)
                {
                    // Dynamically update target position
                    lastKnownPlayerPos = playerTransform.position;
                    hasReachedSuspectPos = false;

                    spotTimer += Time.deltaTime;
                    if (spotTimer >= suspectToChaseLapse)
                    {
                        StartChase();
                        spotTimer = 0f; // Reset timer
                    }
                }
            }
        }
        else
        {
            // Reset buffer timer if player is not seen
            spotTimer = 0f;
        }
    }

    void StartSuspect(Vector3 triggerPos)
    {
        currentState = EnemyState.Suspect;
        lastKnownPlayerPos = triggerPos;
        hasReachedSuspectPos = false;
        spotTimer = 0f; // Reset timer when starting suspect
    }

    void StartChase()
    {
        currentState = EnemyState.Chase;
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

        // Draw the yellow circle (View Radius)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Draw the red circle (Immediate Chase)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, immediateChaseRadius);

        // Draw the orange circle (Attack Distance)
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        // Calculate and draw Field of View lines
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    }

    // Helper Function
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}