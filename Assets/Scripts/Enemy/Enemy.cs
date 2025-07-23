using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    Basic, Fast, Swarm, Heavy, Stealth, Flying, BossSpider, None
}

public class Enemy : MonoBehaviour, IDamagable
{
    public EnemyVisuals visuals { get; private set; }
    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected EnemyPortal myPortal;
    private GameManager gameManager;

    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Transform centerPoint;
    public int healthPoint = 4;

    [Header("Movements")]
    [SerializeField] private float turnSpeed = 10f;

    [SerializeField] protected List<Transform> myWaypoints;
    protected int nextWaypointIndex;
    private int currentWaypointIndex;

    private float totalDistance;

    protected bool canBeHidden = true;
    protected bool isHidden;
    private Coroutine hideCo;

    private int originalLayerIndex;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.avoidancePriority = Mathf.RoundToInt(agent.speed * 10);

        visuals = GetComponent<EnemyVisuals>();
        originalLayerIndex = gameObject.layer;

        gameManager = FindFirstObjectByType<GameManager>();
    }

    protected virtual void Start()
    {

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

    protected virtual void Update()
    {
        FaceTarget(agent.steeringTarget);

        if (ShouldChangeWaypoint())
        {
            ChangeWaypoint();
        }

        CollectTotalDistance();
    }

    public void HideEnemy(float duration)
    {
        if (canBeHidden == false)
            return;
        if (hideCo != null)
            StopCoroutine(hideCo);

        hideCo = StartCoroutine(HideEnemyCo(duration));
    }

    private IEnumerator HideEnemyCo(float duration)
    {
        gameObject.layer = LayerMask.NameToLayer("Untargetable");
        visuals.MakeTransperent(true);
        isHidden = true;

        yield return new WaitForSeconds(duration);

        gameObject.layer = originalLayerIndex;
        visuals.MakeTransperent(false);
        isHidden = false;
    }

    protected virtual void ChangeWaypoint()
    {
        agent.SetDestination(GetNextWaypoint());
    }

    protected virtual bool ShouldChangeWaypoint()
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

    protected Vector3 GetFinalWaypoint()
    {
        if (myWaypoints.Count == 0)
            return transform.position;
        return myWaypoints[myWaypoints.Count - 1].position;
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

    public virtual void Die()
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
