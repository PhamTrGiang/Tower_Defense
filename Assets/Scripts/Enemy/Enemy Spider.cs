using UnityEngine;

public class EnemySpider : Enemy
{
    private EnemySpiderVisuals spiderVisuals;

    [Header("EMP Attack Details")]
    [SerializeField] private GameObject empPrefab;
    [SerializeField] private LayerMask whatIsTower;
    [SerializeField] private float towerCheckRadius = 5;
    [SerializeField] private float empCooldown = 8;
    [SerializeField] private float empEffectDuration = 3;
    [SerializeField] private float empDuration = 5;
    private float empAttackTimer;


    protected override void Awake()
    {
        base.Awake();
        spiderVisuals = GetComponent<EnemySpiderVisuals>();
    }

    protected override void Start()
    {
        base.Start();
        spiderVisuals.BrieflySpeedUpLeg();
        empAttackTimer = empCooldown;

    }

    protected override void Update()
    {
        base.Update();

        empAttackTimer -= Time.deltaTime;

        if (empAttackTimer < 0)
            AttemptToEmp();
    }

    private void AttemptToEmp()
    {
        Transform target = FindRandomTower();

        if (target == null)
            return;
        empAttackTimer = empCooldown;

        GameObject newEmp = objectPool.Get(empPrefab, transform.position + new Vector3(0, .15f), Quaternion.identity);
        newEmp.GetComponent<EnemySpiderEMP>().SetUpEMP(empEffectDuration, target.position,empDuration);
    }

    private Transform FindRandomTower()
    {
        Collider[] towers = Physics.OverlapSphere(transform.position, towerCheckRadius, whatIsTower);

        if (towers.Length > 0)
            return towers[Random.Range(0, towers.Length)].transform.root;

        return null;
    }

    protected override void ChangeWaypoint()
    {
        spiderVisuals.BrieflySpeedUpLeg();
        base.ChangeWaypoint();
    }

    protected override bool ShouldChangeWaypoint()
    {
        if (nextWaypointIndex >= myWaypoints.Length) return false;

        if (agent.remainingDistance < .5f) return true;

        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, towerCheckRadius);
    }
}
