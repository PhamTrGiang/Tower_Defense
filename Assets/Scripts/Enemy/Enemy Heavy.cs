using UnityEngine;

public class EnemyHeavy : Enemy
{
    [Header("Enemy Details")]
    [SerializeField] private float maxShield = 50;
    [SerializeField] private float currentShield = 50;
    [SerializeField] private EnemyShield shieldObject;

    protected override void OnEnable()
    {
        base.OnEnable();

        currentShield = maxShield;
        EnableShieldIfNeeded();
    }

    private void EnableShieldIfNeeded()
    {
        if (shieldObject != null && currentShield > 0)
            shieldObject.gameObject.SetActive(true);

    }

    public override void TakeDame(float damage)
    {
        if (currentShield > 0)
        {
            currentShield -= damage;
            shieldObject.ActivateShieldImpact();

            if (currentShield <= 0)
                shieldObject.gameObject.SetActive(false);
        }
        else
            base.TakeDame(damage);
    }
}
