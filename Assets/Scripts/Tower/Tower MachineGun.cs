using UnityEngine;

public class TowerMachineGun : Tower
{
    private MachineGunVisuals machineGunVisuals;

    [Header("Machine Gun Details")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float damage;
    [SerializeField] private float projectileSpeed;
    [Space]
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private Transform[] gunPointset;
    private int gunPointIndex;

    protected override void Awake()
    {
        base.Awake();
        machineGunVisuals = GetComponent<MachineGunVisuals>();
    }

    protected override void Attack()
    {
        gunPoint = gunPointset[gunPointIndex];
        Vector3 directionToEnemy = DirectionToEnemy(gunPoint);

        if (Physics.Raycast(gunPoint.position, directionToEnemy, out RaycastHit hitInfo, Mathf.Infinity, whatIsTargetable))
        {
            IDamagable damagable = hitInfo.transform.GetComponent<IDamagable>();

            if (damagable == null)
                return;

            GameObject newProjetile = Instantiate(projectilePrefab, gunPoint.position, gunPoint.rotation);
            newProjetile.GetComponent<ProjectileMachineGun>().SetupProjectile(hitInfo.point, damagable, damage, projectileSpeed);

            machineGunVisuals.RecoilFx(gunPoint);

            base.Attack();
            gunPointIndex = (gunPointIndex + 1) % gunPointset.Length;
        }

    }

    protected override void RotateTowardsEnemy()
    {
        if (currentEnemy == null)
            return;

        Vector3 directionToEnemy = (currentEnemy.CenterPoint() - rotationOffset) - towerHead.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);

        Vector3 rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;
        towerHead.rotation = Quaternion.Euler(rotation);
    }
}
