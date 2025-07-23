using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

[System.Serializable]
public class WaveDetails
{
    public GridBuilder nextGird;
    public EnemyPortal[] newPortal;
    public int basicEnemy;
    public int fastEnemy;
    public int swarmEnemy;
    public int heavyEnemy;
    public int stealthEnemy;
    public int flyingEnemy;
    public int flyingBossEnemy;
    public int spiderBossEnemy;
}

public class WaveManager : MonoBehaviour
{
    private GameManager gameManager;
    private TileAnimator tileAnimator;
    private UI_InGame inGameUI;

    [SerializeField] private GridBuilder currentGrid;
    [SerializeField] private NavMeshSurface flyingNavSurface;
    [SerializeField] private MeshCollider[] flyingNavColliders;

    [Header("Wave Details")]
    [SerializeField] private float timeBetweenWaves = 10;
    [SerializeField] private float waveTimer;
    [SerializeField] private WaveDetails[] levelWaves;
    [SerializeField] private int waveIndex;

    [Header("Level Update Details")]
    [SerializeField] private float yOffset = 5;
    [SerializeField] private float tileDelay = .1f;



    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject basicEnemy;
    [SerializeField] private GameObject fastEnemy;
    [SerializeField] private GameObject swarmEnemy;
    [SerializeField] private GameObject heavyEnemy;
    [SerializeField] private GameObject stealthEnemy;
    [SerializeField] private GameObject flyingEnemy;
    [SerializeField] private GameObject flyingBossEnemy;
    [SerializeField] private GameObject spiderBossEnemy;

    private List<EnemyPortal> enemyPortals;
    private bool waveTimerEnable;
    private bool makingNextWave;
    public bool gameBegin;


    private void Awake()
    {
        enemyPortals = new List<EnemyPortal>(FindObjectsByType<EnemyPortal>(FindObjectsSortMode.None));

        gameManager = FindFirstObjectByType<GameManager>();
        tileAnimator = FindFirstObjectByType<TileAnimator>();
        inGameUI = FindFirstObjectByType<UI_InGame>(FindObjectsInactive.Include);

        flyingNavColliders = GetComponentsInChildren<MeshCollider>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            ActivateWaveManager();

        if (gameBegin == false)
            return;
        HandleWaveTimer();
    }

    [ContextMenu("Activate Wave Manager")]
    public void ActivateWaveManager()
    {
        gameBegin = true;
        EnableWaveTimer(true);
    }

    public void DectivateWaveManager() => gameBegin = false;

    public void CheckIfWaveCompleted()
    {
        if (gameBegin == false)
            return;

        if (AllEnemiesDefeated() == false || makingNextWave)
            return;

        makingNextWave = true;
        waveIndex++;

        if (HasNoMoreWave())
        {
            gameManager.LevelCompleted();
            return;
        }

        if (HasNewLayout())
            AttemptToUpdateLayout();
        else
            EnableWaveTimer(true);

    }

    public void StartNewWave()
    {
        UpdateNavMeshes();
        GiveEnemiesToPortals();
        EnableWaveTimer(false);

        makingNextWave = false;
    }

    private void HandleWaveTimer()
    {
        if (waveTimerEnable == false)
            return;

        waveTimer -= Time.deltaTime;
        inGameUI.UpdateWaveTimerUI(waveTimer);

        if (waveTimer <= 0)
            StartNewWave();

    }

    private void GiveEnemiesToPortals()
    {
        List<GameObject> newEnemies = GetNewEnemies();
        int portalIndex = 0;

        if (newEnemies == null)
        {
            Debug.LogWarning("You have no more wave to setup");
            return;
        }
        for (int i = 0; i < newEnemies.Count; i++)
        {
            GameObject enemyToAdd = newEnemies[i];
            EnemyPortal portalToReciveEnemy = enemyPortals[portalIndex];

            portalToReciveEnemy.AddEnemy(enemyToAdd);

            portalIndex++;

            if (portalIndex >= enemyPortals.Count)
            {
                portalIndex = 0;
            }
        }
    }

    private void AttemptToUpdateLayout() => UpdateLevelLayout(levelWaves[waveIndex]);

    private void UpdateLevelLayout(WaveDetails nextWave)
    {
        GridBuilder nextGrid = nextWave.nextGird;
        List<GameObject> grid = currentGrid.GetTileSetUp();
        List<GameObject> newGrid = nextGrid.GetTileSetUp();

        if (grid.Count != newGrid.Count)
        {
            Debug.Log("Current grid and new grid have different size.");
            return;
        }

        List<TileSlot> tilesToRemove = new List<TileSlot>();
        List<TileSlot> tilesToAdd = new List<TileSlot>();

        for (int i = 0; i < grid.Count; i++)
        {
            TileSlot currentTile = grid[i].GetComponent<TileSlot>();
            TileSlot newTile = newGrid[i].GetComponent<TileSlot>();

            bool shouldBeUpdated = currentTile.GetMesh() != newTile.GetMesh() ||
                                    currentTile.GetMaterial() != newTile.GetMaterial() ||
                                    currentTile.GetAllChildren().Count != newTile.GetAllChildren().Count ||
                                    currentTile.transform.rotation != newTile.transform.rotation;

            if (shouldBeUpdated)
            {
                tilesToRemove.Add(currentTile);
                tilesToAdd.Add(newTile);

                grid[i] = newTile.gameObject;
            }

        }

        StartCoroutine(RebuildLevelCO(tilesToRemove, tilesToAdd, nextWave, tileDelay));
    }

    private IEnumerator RebuildLevelCO(List<TileSlot> tilesToRemove, List<TileSlot> tilesToAdd, WaveDetails waveDetails, float delay)
    {
        for (int i = 0; i < tilesToRemove.Count; i++)
        {
            yield return new WaitForSeconds(delay);
            RemoveTile(tilesToRemove[i]);
        }

        for (int i = 0; i < tilesToAdd.Count; i++)
        {
            yield return new WaitForSeconds(delay);
            AddTile(tilesToAdd[i]);
        }

        EnableNewPortals(waveDetails.newPortal);
        EnableWaveTimer(true);
    }

    private void AddTile(TileSlot newTile)
    {
        newTile.gameObject.SetActive(true);
        newTile.transform.position += new Vector3(0, -yOffset, 0);
        newTile.transform.parent = currentGrid.transform;

        Vector3 targetPosition = newTile.transform.position + new Vector3(0, yOffset, 0);

        tileAnimator.MoveTile(newTile.transform, targetPosition);
    }

    private void RemoveTile(TileSlot tileToRemove)
    {
        Vector3 targetPosition = tileToRemove.transform.position + new Vector3(0, -yOffset, 0);
        tileAnimator.MoveTile(tileToRemove.transform, targetPosition);

        Destroy(tileToRemove.gameObject, 1);
    }

    private void EnableWaveTimer(bool enable)
    {
        if (waveTimerEnable == enable)
            return;

        waveTimer = timeBetweenWaves;
        waveTimerEnable = enable;
        inGameUI.EnableWaveTimer(enable);
    }

    private void EnableNewPortals(EnemyPortal[] newPortals)
    {
        foreach (EnemyPortal portal in newPortals)
        {
            portal.AssignWaveManager(this);
            portal.gameObject.SetActive(true);
            enemyPortals.Add(portal);
        }
    }

    private void UpdateNavMeshes()
    {
        foreach (var collider in flyingNavColliders)
        {
            collider.enabled = true;
        }

        flyingNavSurface.BuildNavMesh();

        foreach (var collider in flyingNavColliders)
        {
            collider.enabled = false;
        }

        currentGrid.UpdateNavMesh();
    }

    private List<GameObject> GetNewEnemies()
    {
        if (waveIndex >= levelWaves.Length)
        {
            Debug.LogWarning("You have no more waves");
            return null;
        }

        List<GameObject> newEnemyList = new List<GameObject>();

        for (int i = 0; i < levelWaves[waveIndex].basicEnemy; i++)
        {
            newEnemyList.Add(basicEnemy);
        }

        for (int i = 0; i < levelWaves[waveIndex].fastEnemy; i++)
        {
            newEnemyList.Add(fastEnemy);
        }

        for (int i = 0; i < levelWaves[waveIndex].swarmEnemy; i++)
        {
            newEnemyList.Add(swarmEnemy);
        }
        for (int i = 0; i < levelWaves[waveIndex].heavyEnemy; i++)
        {
            newEnemyList.Add(heavyEnemy);
        }
        for (int i = 0; i < levelWaves[waveIndex].stealthEnemy; i++)
        {
            newEnemyList.Add(stealthEnemy);
        }
        for (int i = 0; i < levelWaves[waveIndex].flyingEnemy; i++)
        {
            newEnemyList.Add(flyingEnemy);
        }
        for (int i = 0; i < levelWaves[waveIndex].flyingBossEnemy; i++)
        {
            newEnemyList.Add(flyingBossEnemy);
        }
        for (int i = 0; i < levelWaves[waveIndex].spiderBossEnemy; i++)
        {
            newEnemyList.Add(spiderBossEnemy);
        }

        return newEnemyList;
    }

    public WaveDetails[] GetLevelWaves() => levelWaves;

    private bool AllEnemiesDefeated()
    {
        foreach (EnemyPortal portal in enemyPortals)
        {
            if (portal.GetActiveEnemies().Count > 0)
                return false;
        }
        return true;
    }

    private bool HasNewLayout() => waveIndex < levelWaves.Length && levelWaves[waveIndex].nextGird != null;

    private bool HasNoMoreWave() => waveIndex >= levelWaves.Length;
}