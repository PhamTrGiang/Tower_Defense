using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    private Transform currentEnemy;
    [SerializeField] protected float attackCooldown;
    private float lastTimeAttack;

    [Header("Tower Setting")]
    [SerializeField] protected Transform towerHead;
    [SerializeField] private float rotationSpeed = 10;

    private bool canRotate;

    [SerializeField] private float attackRange = 2.5f;

    [SerializeField] private LayerMask whatIsEnemy;

    protected virtual void Awake() { }

    private void Update()
    {
        if (currentEnemy == null)
        {
            currentEnemy = FindRandomEnemyWithinRange();
            return;
        }

        if (CanAttack())
            Attack();


        if (Vector3.Distance(currentEnemy.position, transform.position) > attackRange)
            currentEnemy = null;

        RotateTowardsEnemy();
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

    private Transform FindRandomEnemyWithinRange()
    {
        List<Transform> posibleTargets = new List<Transform>();
        Collider[] enemiesAround = Physics.OverlapSphere(transform.position, attackRange, whatIsEnemy);

        foreach (Collider enemy in enemiesAround)
        {
            posibleTargets.Add(enemy.transform);
        }

        int randomIndex = Random.Range(0, posibleTargets.Count);

        if (posibleTargets.Count <= 0) return null;

        return posibleTargets[randomIndex];
    }

    public void EnableRotation(bool enabled)
    {
        canRotate = enabled;
    }

    private void RotateTowardsEnemy()
    {
        if (canRotate == false) return;

        if (currentEnemy == null) return;

        Vector3 directionToEnemy = currentEnemy.position - towerHead.position;

        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);

        Vector3 rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;

        towerHead.rotation = Quaternion.Euler(rotation);
    }

    protected Vector3 DirectionToEnemy(Transform startPoint)
    {
        return (currentEnemy.position - startPoint.position).normalized;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}