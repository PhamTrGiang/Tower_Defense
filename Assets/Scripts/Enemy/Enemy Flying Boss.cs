using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyingBoss : EnemyFlying
{
    [Header("Boss Details")]
    [SerializeField] private GameObject bossUnitPrefab;
    [SerializeField] private int amountToCreate = 150;
    [SerializeField] private float cooldown = .05f;
    private float creationTimer;

    private List<Enemy> createEnemies = new List<Enemy>();

    protected override void Start()
    {
        
    }

    protected override void Update()
    {
        base.Update();

        creationTimer -= Time.deltaTime;

        if (creationTimer < 0 && amountToCreate > 0)
        {
            creationTimer = cooldown;
            CreateNewBossUnit();
        }
    }

    private void CreateNewBossUnit()
    {
        amountToCreate--;
        GameObject newUnit = Instantiate(bossUnitPrefab, transform.position, Quaternion.identity);

        EnemyBossUnit bossUnit = newUnit.GetComponent<EnemyBossUnit>();
        bossUnit.SetupEnemy(GetFinalWaypoint(),this,myPortal);

        createEnemies.Add(bossUnit);
    }

    private void EliminateAllUnit()
    {
        foreach (Enemy enemy in createEnemies)
        {
            enemy.Die();
        }
    }

    public override void Die()
    {
        EliminateAllUnit();
        base.Die();
    }
}
