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

        if (Physics.Raycast(gunPoint.position, directionToEnemy, out RaycastHit hitInfo, Mathf.Infinity))
        {
            towerHead.forward = directionToEnemy;

            visuals.PlayAttackVFX(gunPoint.position, hitInfo.point);
            visuals.PlayReloadFX(attackCooldown);

            IDamagable damagable = hitInfo.transform.GetComponent<IDamagable>();

            if (damagable != null)
                damagable.TakeDame(damage);
        }
    }
}
