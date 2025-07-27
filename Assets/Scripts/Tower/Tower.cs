using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    public Enemy currentEnemy;

    protected bool towerActive = true;
    protected Coroutine deactiveatedCo;
    protected GameObject currentEmpFx;

    [SerializeField] private bool dynamicTargetChange;
    [SerializeField] protected float attackCooldown = 1;
    private float lastTimeAttack;

    [Header("Tower Setting")]
    [SerializeField] protected EnemyType enemyPriorityType = EnemyType.None;
    [SerializeField] protected Transform towerHead;
    [SerializeField] protected Transform gunPoint;
    [SerializeField] private float rotationSpeed = 10;

    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] protected LayerMask whatIsTargetable;

    private float targetCheckInterval = .1f;
    private float lastTimeCheckedTarget = .1f;

    [Header("SFX Details")]
    [SerializeField] protected AudioSource attacksSfx;

    protected virtual void Awake() { }

    private void Update()
    {
        if (towerActive == false)
            return;

        LooseTargetIfNeeded();
        UpdateTargetIfNeeded();
        HandleRotation();

        if (CanAttack())
            Attack();
    }

    public void DeactivateTower(float duration, GameObject empFxPrefab)
    {
        if (deactiveatedCo != null)
            StopCoroutine(deactiveatedCo);

        if (currentEmpFx != null)
            Destroy(currentEmpFx);

        currentEmpFx = Instantiate(empFxPrefab, transform.position + new Vector3(0, .5f), Quaternion.identity);
        deactiveatedCo = StartCoroutine(DeactevateTowerCo(duration));
    }

    private IEnumerator DeactevateTowerCo(float duration)
    {
        towerActive = false;

        yield return new WaitForSeconds(duration);

        towerActive = true;
        lastTimeAttack = Time.time;
        Destroy(currentEmpFx);
    }

    private void LooseTargetIfNeeded()
    {
        if (currentEnemy == null)
            return;

        if (Vector3.Distance(currentEnemy.CenterPoint(), transform.position) > attackRange)
            currentEnemy = null;
    }

    private void UpdateTargetIfNeeded()
    {
        if (currentEnemy == null)
        {
            currentEnemy = FindEnemyWithinRange();
            return;
        }

        if (dynamicTargetChange == false)
            return;

        if (Time.time > lastTimeCheckedTarget + targetCheckInterval)
        {
            lastTimeCheckedTarget = Time.time;
            currentEnemy = FindEnemyWithinRange();
        }
    }

    protected virtual void Attack()
    {
        lastTimeAttack += Time.time;
    }

    private bool CanAttack()
    {
        return Time.time > lastTimeAttack + attackCooldown && currentEnemy != null;
    }

    protected virtual Enemy FindEnemyWithinRange()
    {
        List<Enemy> priorityTargets = new List<Enemy>();
        List<Enemy> possibleTargets = new List<Enemy>();

        Collider[] enemiesAround = Physics.OverlapSphere(transform.position, attackRange, whatIsEnemy);

        foreach (Collider enemy in enemiesAround)
        {
            Enemy newEnemy = enemy.GetComponent<Enemy>();
            if (newEnemy == null)
                continue;

            EnemyType newEnemyType = newEnemy.GetEnemyType();

            if (newEnemyType == enemyPriorityType)
                priorityTargets.Add(newEnemy);
            else
                possibleTargets.Add(newEnemy);

            possibleTargets.Add(newEnemy);
        }

        if (priorityTargets.Count > 0)
            return GetMostAdvancedEnemy(priorityTargets);

        if (possibleTargets.Count > 0)
            return GetMostAdvancedEnemy(possibleTargets);

        return null;
    }

    private Enemy GetMostAdvancedEnemy(List<Enemy> targets)
    {
        Enemy mostAdvancedEnemy = null;
        float minRemainingDistance = float.MaxValue;

        foreach (Enemy enemy in targets)
        {
            float remainingDistance = enemy.DistanceAToFinishLine();

            if (remainingDistance < minRemainingDistance)
            {
                minRemainingDistance = remainingDistance;
                mostAdvancedEnemy = enemy;
            }
        }

        return mostAdvancedEnemy;
    }

    protected virtual void HandleRotation()
    {
        RotateTowardsEnemy();
    }

    private void RotateTowardsEnemy()
    {
        if (currentEnemy == null || towerHead == null)
            return;

        Vector3 directionToEnemy = DirectionToEnemy(towerHead);

        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);

        Vector3 rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;

        towerHead.rotation = Quaternion.Euler(rotation);
    }

    protected Vector3 DirectionToEnemy(Transform startPoint)
    {
        return (currentEnemy.CenterPoint() - startPoint.position).normalized;
    }

    public float GetAttackRange() => attackRange;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}