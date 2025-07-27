using System.Transactions;
using UnityEngine;

public class ProjectileCanon : MonoBehaviour
{
    private Rigidbody rb;
    private float damage;

    [SerializeField] private float damageRadius;
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private GameObject ExplosionFx;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetupProjectile(Vector3 newVelocity, float newDamage)
    {
        rb.linearVelocity = newVelocity;
        damage = newDamage;
    }

    private void DamageEnemiesAround()
    {
        Collider[] enemiesAround = Physics.OverlapSphere(transform.position, damageRadius, whatIsEnemy);

        foreach (Collider enemy in enemiesAround)
        {
            IDamagable damagable = enemy.GetComponent<IDamagable>();

            if (damagable != null)
                damagable.TakeDame((int)damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DamageEnemiesAround();
        ExplosionFx.SetActive(true);
        ExplosionFx.transform.parent = null;
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
