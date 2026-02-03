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
    public float viewRadius = 10f;        // How far the enemy can see
    [Range(0, 360)]
    public float viewAngle = 90f;         // Field of view angle
    public LayerMask obstacleMask;        // Layers that block vision (Walls, etc.)

    [Header("Chase Settings")]
    public float chaseSpeed = 6f;         // Speed when chasing
    public float giveUpDistance = 15f;    // Distance to stop chasing

    [Header("Debug Settings")]
    public bool showDebugGizmos = true;   // Toggle this to show/hide lines in Scene view

    // Internal variables
    private NavMeshAgent agent;
    private Transform playerTransform;    // Reference to the player
    private int currentPointIndex = 0;
    private float waitTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        // Auto-find player by Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // Ensure there are patrol points assigned before moving
        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[0].position;
        }
    }

    void Update()
    {
        // Core FSM Switch
        switch (currentState)
        {
            case EnemyState.Patrol:
                LookForPlayer(); // Keep looking while patrolling
                PatrolLogic();
                break;

            case EnemyState.Suspect:
                LookForPlayer();
                // Logic for suspicion will go here
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

        agent.speed = moveSpeed; // Ensure speed is normal

        // Check if the agent has reached the destination
        // !pathPending ensures we don't check distance while the path is still being calculated
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;

            // If have waited long enough, move to the next point
            if (waitTimer >= waitTime)
            {
                GoToNextPoint();
                waitTimer = 0f; // Reset timer
            }
        }
    }

    void ChaseLogic()
    {
        // Safety check if player is missing
        if (playerTransform == null) return;

        agent.speed = chaseSpeed; // Run faster
        agent.destination = playerTransform.position; // Move to player

        // Check distance to see if we should give up
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // If player is too far, go back to Patrol
        if (distanceToPlayer > giveUpDistance)
        {
            currentState = EnemyState.Patrol;
            GoToNextPoint(); // Resume patrol path
        }
    }

    void LookForPlayer()
    {
        if (playerTransform == null) return;

        // 1. Check Distance
        Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < viewRadius)
        {
            // 2. Check Angle
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                // 3. Check Obstacles
                // 
                if (!Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distanceToPlayer, obstacleMask))
                {
                    // No obstacles found, Player is visible
                    currentState = EnemyState.Chase;
                }
            }
        }
    }

    void GoToNextPoint()
    {
        // Safety check
        if (patrolPoints.Length == 0)
            return;

        // Cycle through the waypoints (0 -> 1 -> 2 -> 0)
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;

        // Set the new destination for the NavMeshAgent
        agent.destination = patrolPoints[currentPointIndex].position;
    }


    void OnDrawGizmosSelected()
    {
        if (this == null || transform == null) return;
        // If toggle is off, do nothing
        if (!showDebugGizmos) return;

        // Draw the yellow circle (Max Distance)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Calculate the left and right boundary angles
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        // Draw the red sector lines (Field of View)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    }

    // Helper Function: Converts angle to a direction vector
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}