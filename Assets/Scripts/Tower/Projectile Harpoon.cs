using UnityEngine;

public class ProjectileHarpoon : MonoBehaviour
{
    private TowerHarpoon tower;
    private Enemy enemy;
    private float speed;
    private bool isAttached;

    [SerializeField] private Transform connectionPoint;

    private void Update()
    {
        if (enemy == null || isAttached)
            return;

        MoveTowardsEnemy();

        if (Vector3.Distance(transform.position, enemy.transform.position) < .25f)
            AttachToEnemy();
    }

    private void MoveTowardsEnemy()
    {
        transform.position = Vector3.MoveTowards(transform.position, enemy.transform.position, speed * Time.deltaTime);
        transform.forward = enemy.transform.position - transform.position;
    }

    private void AttachToEnemy()
    {
        isAttached = true;
        transform.parent = enemy.transform;
        tower.ActivateAttack();
    }

    public void SetupProjectile(Enemy newEnemy, float newSpeed, TowerHarpoon newTower)
    {
        speed = newSpeed;
        enemy = newEnemy;
        tower = newTower;
    }

    public Transform GetConnectionPoint()
    {
        if (connectionPoint == null)
            return transform;
            
        return connectionPoint;
    }
}
