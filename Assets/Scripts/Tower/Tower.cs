using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    public Enemy currentEnemy;

    protected bool towerActive = true;
    protected Coroutine deactiveatedCo;
    protected GameObject currentEmpFx;

    [SerializeField] protected float attackCooldown;
    private float lastTimeAttack;

    [Header("Tower Setting")]
    [SerializeField] protected EnemyType enemyPriorityType = EnemyType.None;
    [SerializeField] protected Transform towerHead;
    [SerializeField] private float rotationSpeed = 10;

    private bool canRotate;

    [SerializeField] private float attackRange = 2.5f;

    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] protected LayerMask whatIsTargetable;

    [Space]
    [SerializeField] private bool dynamicTargetChange;
    private float targetCheckInterval = .1f;
    private float lastTimeCheckedTarget = .1f;

    [Header("SFX Details")]
    [SerializeField] protected AudioSource attacksSfx;

    protected virtual void Awake()
    {
        EnableRotation(true);
    }

    private void Update()
    {

        if (towerActive == false)
            return;

        UpdateTargetIfNeeded();

        if (currentEnemy == null)
        {
            currentEnemy = FindEnemyWithinRange();
            return;
        }

        if (CanAttack())
            Attack();
        LooseTargetIfNeeded();

        RotateTowardsEnemy();
    }

    public void DeactivevateTower(float duration,GameObject empFxPrefab)
    {
        if (deactiveatedCo != null)
            StopCoroutine(deactiveatedCo);

        if (currentEmpFx != null)
            Destroy(currentEmpFx);

        currentEmpFx = Instantiate(empFxPrefab, transform.position + new Vector3(0, .5f), Quaternion.identity);
        deactiveatedCo = StartCoroutine(DisableTowerCo(duration));  
    }

    private IEnumerator DisableTowerCo(float duration)
    {
        towerActive = false;

        yield return new WaitForSeconds(duration);

        towerActive = true;
        lastTimeAttack = Time.time;
        Destroy(currentEmpFx);
    }

    public float GetAttackRange() => attackRange;

    private void LooseTargetIfNeeded()
    {
        if (Vector3.Distance(currentEnemy.CenterPoint(), transform.position) > attackRange)
            currentEnemy = null;
    }

    private void UpdateTargetIfNeeded()
    {
        if (dynamicTargetChange == false)
            return;

        if (Time.time > lastTimeCheckedTarget + targetCheckInterval)
        {
            lastTimeCheckedTarget = Time.time;
            currentEnemy = FindEnemyWithinRange();
        }
    }

    protected virtual void Attack() { }

    private bool CanAttack()
    {
        if (Time.time > lastTimeAttack + attackCooldown)
        {
            lastTimeAttack = Time.time;
            return true;
        }
        return false;
    }

    private Enemy FindEnemyWithinRange()
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

    public void EnableRotation(bool enabled)
    {
        canRotate = enabled;
    }

    private void RotateTowardsEnemy()
    {
        if (canRotate == false) return;

        if (currentEnemy == null) return;

        Vector3 directionToEnemy = DirectionToEnemy(towerHead);

        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);

        Vector3 rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;

        towerHead.rotation = Quaternion.Euler(rotation);
    }

    protected Vector3 DirectionToEnemy(Transform startPoint)
    {
        return (currentEnemy.CenterPoint() - startPoint.position).normalized;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}