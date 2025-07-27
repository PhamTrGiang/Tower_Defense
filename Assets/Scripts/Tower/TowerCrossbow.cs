using UnityEngine;

public class TowerCrossbow : Tower
{
    private CrossbowVisuals visuals;

    [Header("Crossbow Details")]
    [SerializeField] private int damage;

    protected override void Awake()
    {
        base.Awake();
        visuals = GetComponent<CrossbowVisuals>();
    }

    protected override void Attack()
    {
        base.Attack();

        Vector3 directionToEnemy = DirectionToEnemy(gunPoint);

        if (Physics.Raycast(gunPoint.position, directionToEnemy, out RaycastHit hitInfo, Mathf.Infinity, whatIsTargetable))
        {
            towerHead.forward = directionToEnemy;

            IDamagable damagable = hitInfo.transform.GetComponent<IDamagable>();
            damagable.TakeDame(damage);

            visuals.CreateOnHitFx(hitInfo.point);
            visuals.PlayAttackVFX(gunPoint.position, hitInfo.point);
            visuals.PlayReloadVFX(attackCooldown);
            AudioManager.Instance?.PlaySFX(attacksSfx, true);
        }
    }
}
