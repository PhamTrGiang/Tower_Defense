using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    Basic, Fast, None,
}

public class Enemy : MonoBehaviour, IDamagable
{
    private GameManager gameManager;
    private EnemyPortal myPortal;
    private NavMeshAgent agent;

    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Transform centerPoint;
    public int healthPoint = 4;

    [Header("Movements")]
    [SerializeField] private float turnSpeed = 10f;

    [SerializeField] private List<Transform> myWaypoints;
    private int nextWaypointIndex;
    private int currentWaypointIndex;

    private float totalDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.avoidancePriority = Mathf.RoundToInt(agent.speed * 10);

        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void SetupEnemy(List<Waypoint> newWaypoint, EnemyPortal myNewPotal)
    {
        myWaypoints = new List<Transform>();

        foreach (var point in newWaypoint)
        {
            myWaypoints.Add(point.transform);
        }

        myPortal = myNewPotal;
    }

    private void Update()
    {
        FaceTarget(agent.steeringTarget);

        if (ShouldChangeWaypoint())
        {
            agent.SetDestination(GetNextWaypoint());
        }

        CollectTotalDistance();
    }

    private bool ShouldChangeWaypoint()
    {
        if (nextWaypointIndex >= myWaypoints.Count) return false;

        if (agent.remainingDistance < .5f) return true;

        Vector3 currentWaypoint = myWaypoints[currentWaypointIndex].position;
        Vector3 nextWayporint = myWaypoints[nextWaypointIndex].position;

        float distanceToNextWaypoint = Vector3.Distance(transform.position, nextWayporint);
        float distanceToBeetwenPoints = Vector3.Distance(currentWaypoint, nextWayporint);

        return distanceToBeetwenPoints > distanceToNextWaypoint;
    }

    public float DistanceAToFinishLine() => totalDistance + agent.remainingDistance;

    private void CollectTotalDistance()
    {
        for (int i = 0; i < myWaypoints.Count - 1; i++)
        {
            float distance = Vector3.Distance(myWaypoints[i].position, myWaypoints[i + 1].position);
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
        if (nextWaypointIndex >= myWaypoints.Count)
            return transform.position;
        Vector3 targertPoint = myWaypoints[nextWaypointIndex].position;

        if (nextWaypointIndex > 0)
        {
            float distance = Vector3.Distance(myWaypoints[nextWaypointIndex].position, myWaypoints[nextWaypointIndex - 1].position);
            totalDistance -= distance;
        }

        nextWaypointIndex++;
        currentWaypointIndex = nextWaypointIndex - 1;
        return targertPoint;

    }

    public Vector3 CenterPoint() => centerPoint.position;

    public EnemyType GetEnemyType() => enemyType;

    public void TakeDame(int damage)
    {
        healthPoint = healthPoint - damage;

        if (healthPoint <= 0)
            Die();
    }

    private void Die()
    {
        myPortal.RemoveActiveEnemy(gameObject);
        gameManager.UpdateCurrency(1);
        Destroy(gameObject);
    }

    public void DestroyEnemy()
    {
        myPortal.RemoveActiveEnemy(gameObject);
        Destroy(gameObject);
    }
}
