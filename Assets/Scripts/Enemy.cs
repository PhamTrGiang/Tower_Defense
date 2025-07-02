using UnityEngine;
using UnityEngine.AI;


public enum EnemyType
{
    Basic,Fast,None,
}

public class Enemy : MonoBehaviour, IDamagable
{
    private NavMeshAgent agent;

    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Transform centerPoint;
    public int healthPoint = 4;

    [Header("Movements")]
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private Transform[] waypoints;

    private int waypointIndex;
    private float totalDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.avoidancePriority = Mathf.RoundToInt(agent.speed * 10);
    }

    private void Start()
    {
        waypoints = WaypointManager.Instance.GetWaypoints();
        CollectTotalDistance();
    }

    private void Update()
    {
        FaceTarget(agent.steeringTarget);

        if (agent.remainingDistance < .5f)
        {
            agent.SetDestination(GetNextWaypoint());
        }
    }

    public float DistanceAToFinishLine() => totalDistance + agent.remainingDistance;

    private void CollectTotalDistance()
    {
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            float distance = Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
            totalDistance += distance;
        }
    }

    private void FaceTarget(Vector3 target)
    {
        Vector3 directionToTarget = target - transform.position;
        if (directionToTarget.magnitude == 0)
            return;

        directionToTarget.y = 0;

        Quaternion newRotation = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, turnSpeed * Time.deltaTime);
    }

    private Vector3 GetNextWaypoint()
    {
        if (waypointIndex >= waypoints.Length)
            return transform.position;
        Vector3 targertPoint = waypoints[waypointIndex].position;

        if (waypointIndex > 0)
        {
            float distance = Vector3.Distance(waypoints[waypointIndex].position, waypoints[waypointIndex - 1].position);
            totalDistance -= distance;
        }

        waypointIndex++;
        return targertPoint;

    }

    public Vector3 CenterPoint() => centerPoint.position;

    public EnemyType GetEnemyType() => enemyType;

    public void TakeDame(int damage)
    {
        healthPoint = healthPoint - damage;

        if (healthPoint <= 0)
            Destroy(gameObject);
    }
}
