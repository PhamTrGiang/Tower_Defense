using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPortal : MonoBehaviour
{
    [SerializeField] private WaveManager myWaveManager;
    [SerializeField] private float spawnCooldown;
    private float spawnTimer;

    [Space]
    [SerializeField] private ParticleSystem flyPortalFx;
    private Coroutine flyPortalFxCo;


    [Space]
    [SerializeField] private List<Waypoint> waypointList;

    private List<GameObject> enemiesToCreate = new List<GameObject>();
    private List<GameObject> activeEnemy = new List<GameObject>();

    private void Awake()
    {
        CollectWaypoint();
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
        GameObject randomEnemy = GetRandomEnemy();
        GameObject newEnemy = Instantiate(randomEnemy, transform.position, Quaternion.identity, transform);

        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        enemyScript.SetupEnemy(waypointList, this);

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
    }
}
