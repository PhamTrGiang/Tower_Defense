using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPortal : MonoBehaviour
{
    private ObjectPoolManager objectPool;

    [SerializeField] private WaveManager myWaveManager;
    [SerializeField] private float spawnCooldown;
    private float spawnTimer;
    private bool canCreateEnemies = true;

    [Space]
    [SerializeField] private ParticleSystem flyPortalFx;
    private Coroutine flyPortalFxCo;

    [Space]
    [SerializeField] private List<Waypoint> waypointList;
    public Vector3[] currentWaypoints { get; private set; }

    private List<GameObject> enemiesToCreate = new List<GameObject>();
    private List<GameObject> activeEnemy = new List<GameObject>();

    private void Awake()
    {
        CollectWaypoint();
    }
    private void Start()
    {
        objectPool = ObjectPoolManager.Instance;
    }


    private void Update()
    {
        if (CanMakeNewEnemy())
            CreateEnemy();
    }

    public void AssignWaveManager(WaveManager newWaveManager) => myWaveManager = newWaveManager;

    private bool CanMakeNewEnemy()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0 && enemiesToCreate.Count > 0)
        {
            spawnTimer = spawnCooldown;
            return true;
        }

        return false;
    }

    private void CreateEnemy()
    {
        if (!canCreateEnemies)
            return;

        GameObject randomEnemy = GetRandomEnemy();
        GameObject newEnemy = objectPool.Get(randomEnemy, transform.position, Quaternion.identity, transform);

        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        enemyScript.SetupEnemy(this);

        PlaceEnemyAtFlyPortal(newEnemy, enemyScript.GetEnemyType());

        activeEnemy.Add(newEnemy);
    }

    private void PlaceEnemyAtFlyPortal(GameObject newEnemy, EnemyType enemyType)
    {
        if (enemyType != EnemyType.Flying)
            return;

        if (flyPortalFxCo != null)
            StopCoroutine(flyPortalFxCo);

        flyPortalFxCo = StartCoroutine(EnableFlyPortalFxCo());

        newEnemy.transform.position = flyPortalFx.transform.position;
    }

    private IEnumerator EnableFlyPortalFxCo()
    {
        flyPortalFx.Play();

        yield return new WaitForSeconds(2);

        flyPortalFx.Stop();
    }

    private GameObject GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemiesToCreate.Count);
        GameObject chooseEnemy = enemiesToCreate[randomIndex];

        enemiesToCreate.Remove(chooseEnemy);

        return chooseEnemy;
    }

    public void AddEnemy(GameObject enemyToAdd) => enemiesToCreate.Add(enemyToAdd);
    public void RemoveActiveEnemy(GameObject enemyToRemove)
    {
        if (activeEnemy.Contains(enemyToRemove))
            activeEnemy.Remove(enemyToRemove);

        myWaveManager.CheckIfWaveCompleted();
    }

    public List<GameObject> GetActiveEnemies() => activeEnemy;
    public void CanCreateNewEnemies(bool canCreate) => canCreateEnemies = canCreate;

    [ContextMenu("Collect Waypoint")]
    private void CollectWaypoint()
    {
        waypointList = new List<Waypoint>();

        foreach (Transform child in transform)
        {
            Waypoint waypoint = child.GetComponent<Waypoint>();

            if (waypoint != null)
            {
                waypointList.Add(waypoint);
            }
        }

        currentWaypoints = new Vector3[waypointList.Count];

        for (int i = 0; i < currentWaypoints.Length; i++)
        {
            currentWaypoints[i] = waypointList[i].transform.position;
        }
    }
}
