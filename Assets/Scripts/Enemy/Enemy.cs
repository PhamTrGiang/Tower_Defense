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

    protected ObjectPoolManager objectPool;
    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected EnemyPortal myPortal;
    protected GameManager gameManager;

    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Transform centerPoint;
    public float maxHp = 100;
    public float currentHp = 4;
    protected bool isDead;

    [Header("Movements")]
    [SerializeField] private float turnSpeed = 10f;

    [SerializeField] protected Vector3[] myWaypoints;
    protected int nextWaypointIndex;
    protected int currentWaypointIndex;
    protected float totalDistance;
    protected float originalSpeed;

    protected bool canBeHidden = true;
    protected bool isHidden;
    private Coroutine hideCo;
    private Coroutine disableHideCo;
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
        originalSpeed = agent.speed;

        objectPool = ObjectPoolManager.Instance;
    }

    protected virtual void Start()
    {

    }

    public void SetupEnemy(EnemyPortal myNewPotal)
    {
        myPortal = myNewPotal;

        UpdateWaypoints(myPortal.currentWaypoints);
        CollectTotalDistance();
        ResetEnemy();
        BeginMovement();
    }

    private void UpdateWaypoints(Vector3[] newWaypoints)
    {
        myWaypoints = new Vector3[newWaypoints.Length];

        for (int i = 0; i < myWaypoints.Length; i++)
            myWaypoints[i] = newWaypoints[i];
    }

    private void BeginMovement()
    {

        currentWaypointIndex = 0;
        nextWaypointIndex = 0;
        ChangeWaypoint();
    }

    protected void ResetEnemy()
    {
        gameObject.layer = originalLayerIndex;

        visuals.MakeTransperent(false);

        currentHp = maxHp;
        isDead = false;

        agent.speed = originalSpeed;
        agent.enabled = true;
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

    public void SlowSpeed(float slowMultiplier, float duration) => StartCoroutine(SlowEnemyCo(slowMultiplier, duration));

    private IEnumerator SlowEnemyCo(float slowMultiplier, float duration)
    {
        agent.speed = originalSpeed;
        agent.speed = agent.speed * slowMultiplier;

        yield return new WaitForSeconds(duration);

        agent.speed = originalSpeed;

    }

    public void DisableHide(float duration)
    {
        if (disableHideCo != null)
            StopCoroutine(disableHideCo);

        disableHideCo = StartCoroutine(DisableHideCo(duration));
    }

    protected virtual IEnumerator DisableHideCo(float duration)
    {
        canBeHidden = false;

        yield return new WaitForSeconds(duration);

        canBeHidden = true;
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
        if (nextWaypointIndex >= myWaypoints.Length) return false;

        if (agent.remainingDistance < .5f) return true;

        Vector3 currentWaypoint = myWaypoints[currentWaypointIndex];
        Vector3 nextWayporint = myWaypoints[nextWaypointIndex];

        float distanceToNextWaypoint = Vector3.Distance(transform.position, nextWayporint);
        float distanceToBeetwenPoints = Vector3.Distance(currentWaypoint, nextWayporint);

        return distanceToBeetwenPoints > distanceToNextWaypoint;
    }

    public virtual float DistanceAToFinishLine() => totalDistance + agent.remainingDistance;

    private void CollectTotalDistance()
    {
        for (int i = 0; i < myWaypoints.Length - 1; i++)
        {
            float distance = Vector3.Distance(myWaypoints[i], myWaypoints[i + 1]);
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
        if (myWaypoints.Length == 0)
            return transform.position;
        return myWaypoints[myWaypoints.Length - 1];
    }

    private Vector3 GetNextWaypoint()
    {
        if (nextWaypointIndex >= myWaypoints.Length)
            return transform.position;
        Vector3 targertPoint = myWaypoints[nextWaypointIndex];

        if (nextWaypointIndex > 0)
        {
            float distance = Vector3.Distance(myWaypoints[nextWaypointIndex], myWaypoints[nextWaypointIndex - 1]);
            totalDistance -= distance;
        }

        nextWaypointIndex++;
        currentWaypointIndex = nextWaypointIndex - 1;
        return targertPoint;

    }

    public Vector3 CenterPoint() => centerPoint.position;

    public EnemyType GetEnemyType() => enemyType;

    public virtual void TakeDame(float damage)
    {
        currentHp = currentHp - damage;

        if (currentHp <= 0 && isDead == false)
        {
            isDead = true;
            Die();
        }
    }

    public virtual void Die()
    {
        gameManager.UpdateCurrency(10);
        RemoveEnemy();
    }

    public virtual void RemoveEnemy()
    {
        visuals.CreateOnDeadFx();
        objectPool.Remove(gameObject);
        agent.enabled = false;

        if (myPortal != null)
            myPortal.RemoveActiveEnemy(gameObject);
    }

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
        CancelInvoke();
    }
}
