using UnityEngine;
using UnityEngine.AI;

public class AI_SimpleWalk : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;

    [Header("Movement")]
    public float startWaitTime = 3f;
    public float speedWalk = 3.5f;
    public float speedRun = 6f;

    [Header("Detection")]
    public float viewRadius = 15f;
    public float viewAngle = 120f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [Header("Attack")]
    public float attackDistance = 2f;

    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootRate = 1f;
    public float shootRange = 10f;

    float shootTimer;

    int m_CurrentWaypointIndex = 0;
    float m_WaitTime;

    bool m_IsPatrol = true;
    bool m_PlayerInRange = false;

    Vector3 m_PlayerPosition;
    Transform player;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned!");
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;

        m_WaitTime = startWaitTime;

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }

    void Update()
    {
        EnvironmentView();

        if (m_PlayerInRange)
        {
            m_IsPatrol = false;
        }

        if (m_IsPatrol)
            Patroling();
        else
            Chasing();
    }

    void Shoot()
{
    shootTimer -= Time.deltaTime;

    if (shootTimer <= 0)
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        shootTimer = 1f / shootRate;

        Debug.Log("🔫 Enemy Shooting!");
    }
}

    // ================= PATROL =================
    void Patroling()
    {
        navMeshAgent.speed = speedWalk;

        if (!navMeshAgent.pathPending &&
            navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.2f)
        {
            m_WaitTime -= Time.deltaTime;

            if (m_WaitTime <= 0)
            {
                NextPoint();
                m_WaitTime = startWaitTime;
            }
            else
            {
                navMeshAgent.isStopped = true;
            }
        }
        else
        {
            navMeshAgent.isStopped = false;
        }
    }

    // ================= CHASE =================
    void Chasing()
    {
         float distance = Vector3.Distance(transform.position, player.position);

    // Look at player
    Vector3 direction = (player.position - transform.position).normalized;
    direction.y = 0;
    transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 5f);

    // If in shooting range → stop and shoot
    if (distance <= shootRange)
    {
        navMeshAgent.isStopped = true;

        Shoot();
    }
    else
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedRun;
        navMeshAgent.SetDestination(player.position);
    }

    // Lost player
    if (distance > viewRadius + 5f)
    {
        m_PlayerInRange = false;
        m_IsPatrol = true;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }
    }

    void Attack()
    {
        navMeshAgent.isStopped = true;
        Debug.Log("ATTACKING PLAYER!");
    }

    // ================= WAYPOINT =================
    void NextPoint()
    {
        if (waypoints.Length == 0) return;

        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;

        Debug.Log("➡ Going to waypoint: " + m_CurrentWaypointIndex);

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }

    // ================= DETECTION =================
    void EnvironmentView()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        m_PlayerInRange = false;

        for (int i = 0; i < targets.Length; i++)
        {
            Transform target = targets[i].transform;

            Vector3 dirToPlayer = (target.position - transform.position).normalized;

            float angle = Vector3.Angle(transform.forward, dirToPlayer);

            if (angle < viewAngle / 2)
            {
                float distToPlayer = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToPlayer, distToPlayer, obstacleMask))
                {
                    Debug.Log("PLAYER DETECTED!");

                    m_PlayerInRange = true;
                    m_PlayerPosition = target.position;
                }
            }
        }
    }

    // ================= DEBUG VISUAL =================
    void OnDrawGizmos()
    {
        // View radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Waypoints
        if (waypoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform wp in waypoints)
            {
                if (wp != null)
                    Gizmos.DrawSphere(wp.position, 0.3f);
            }
        }
    }
}