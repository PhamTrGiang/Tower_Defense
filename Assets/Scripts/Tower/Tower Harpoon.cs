using System.Collections;
using UnityEngine;

public class TowerHarpoon : Tower
{
    private HarpoonVisuals harpoonVisuals;

    [Header("Harpoon Details")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileDefaultPosition;
    [SerializeField] private float projectileSpeed = 15;
    private ProjectileHarpoon currentProjectile;

    [Header("Damage Details")]
    [SerializeField] private float initialDamage = 5;
    [SerializeField] private float damageOverTime = 10;
    [SerializeField] private float overTimeEffectDuration = 4;
    [Range(0, 1)]
    [SerializeField] private float slowEffect = .7f;

    private bool reachedTarget;
    private bool busyWithAttack;
    private Coroutine damageOverTimeCo;

    protected override void Awake()
    {
        base.Awake();
        currentProjectile = GetComponentInChildren<ProjectileHarpoon>();
        harpoonVisuals = GetComponent<HarpoonVisuals>();
    }

    protected override void Attack()
    {
        base.Attack();

        if (Physics.Raycast(gunPoint.position, gunPoint.forward, out RaycastHit hitInfo, Mathf.Infinity, whatIsTargetable))
        {
            busyWithAttack = true;
            currentEnemy = hitInfo.collider.GetComponent<Enemy>();
            currentProjectile.SetupProjectile(currentEnemy, projectileSpeed, this);
            harpoonVisuals.EnableChainVisuals(true, currentProjectile.GetConnectionPoint());

            Invoke(nameof(ResetAttackIfMissed), 1);
        }
    }

    public void ActivateAttack()
    {
        reachedTarget = true;
        currentEnemy.GetComponent<EnemyFlying>().AddObservingTower(this);
        currentEnemy.SlowSpeed(slowEffect, overTimeEffectDuration);
        harpoonVisuals.CreateElectrifyVfx(currentEnemy.transform);

        IDamagable damagable = currentEnemy.GetComponent<IDamagable>();
        damagable?.TakeDame(initialDamage);

        damageOverTimeCo = StartCoroutine(DamageOverTimeCo(damagable));
    }

    private IEnumerator DamageOverTimeCo(IDamagable damagable)
    {
        float time = 0;
        float damageFrequency = overTimeEffectDuration / damageOverTime;
        float damagePerTick = damageOverTime / (overTimeEffectDuration / damageFrequency);

        while (time < overTimeEffectDuration)
        {
            damagable?.TakeDame(damagePerTick);
            yield return new WaitForSeconds(damageFrequency);
            time += damageFrequency;
        }

        ResetAttack();
    }

    public void ResetAttack()
    {
        if (damageOverTimeCo != null)
            StopCoroutine(damageOverTimeCo);

        busyWithAttack = false;
        reachedTarget = false;

        currentEnemy = null;
        lastTimeAttack = Time.time;
        harpoonVisuals.EnableChainVisuals(false);
        CreateNewProjectile();
    }

    private void CreateNewProjectile()
    {
        GameObject newProjectile = objectPool.Get(projectilePrefab, projectileDefaultPosition.position, projectileDefaultPosition.rotation, towerHead);
        currentProjectile = newProjectile.GetComponent<ProjectileHarpoon>();
    }

    private void ResetAttackIfMissed()
    {
        if (reachedTarget)
            return;
        Destroy(currentProjectile.gameObject);
        ResetAttack();
    }

    protected override bool CanAttack()
    {
        return base.CanAttack() && busyWithAttack == false;
    }

    protected override void LooseTargetIfNeeded()
    {
        if (busyWithAttack == false)
            base.LooseTargetIfNeeded();
    }
}
