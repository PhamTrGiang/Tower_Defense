using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform currentEnemy;

    [Header("Tower Setting")]
    [SerializeField] private Transform towerHead;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private float attackRange = 1.5f;

    [SerializeField] private LayerMask whatIsEnemy;

    private void Update()
    {
        if (currentEnemy == null)
        {
            currentEnemy = FindRandomEnemyWithinRange();
            return;
        }

        if (Vector3.Distance(currentEnemy.position, transform.position) > attackRange)
            currentEnemy = null;

        RotateTowardsEnemy();
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

    private void RotateTowardsEnemy()
    {
        if (currentEnemy == null) return;

        Vector3 directionToEnemy = currentEnemy.position - towerHead.position;

        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);

        Vector3 rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;

        towerHead.rotation = Quaternion.Euler(rotation);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}