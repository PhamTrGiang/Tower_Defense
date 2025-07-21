using UnityEngine;
using UnityEngine.PlayerLoop;

public class TowerCrossbow : Tower
{
    private CrossbowVisuals visuals;

    [Header("Crossbow Details")]
    [SerializeField] private int damage;
    [SerializeField] private Transform gunPoint;

    protected override void Awake()
    {
        base.Awake();

        visuals = GetComponent<CrossbowVisuals>();
    }

    protected override void Attack()
    {

        Vector3 directionToEnemy = DirectionToEnemy(gunPoint);

        if (Physics.Raycast(gunPoint.position, directionToEnemy, out RaycastHit hitInfo, Mathf.Infinity,whatIsTargetable))
        {
            towerHead.forward = directionToEnemy;

            Enemy enemyTarget = null;

            EnemyShield enemyShield = hitInfo.collider.GetComponent<EnemyShield>();
            IDamagable damagable = hitInfo.transform.GetComponent<IDamagable>();

            if (damagable != null && enemyShield == null)
            {
                damagable.TakeDame(damage);
                enemyTarget = currentEnemy;
            }

            if (enemyShield != null)
            {
                damagable = enemyShield.GetComponent<IDamagable>();
                damagable.TakeDame(damage);
            }

            visuals.CreateOnHitFx(hitInfo.point);
            visuals.PlayAttackVFX(gunPoint.position, hitInfo.point, enemyTarget);
            visuals.PlayReloadVFX(attackCooldown);
            AudioManager.Instance?.PlaySFX(attacksSfx,true);
        }
    }
}
