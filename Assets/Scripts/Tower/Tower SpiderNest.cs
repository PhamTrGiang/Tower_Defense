using System.Collections;
using UnityEngine;

public class TowerSpiderNest : Tower
{
    [Header("Spider-Nest Details")]
    [SerializeField] private float damage;

    [SerializeField] private GameObject spiderPrefab;
    [Range(0, 1)]
    [SerializeField] private float attackTimeMultiplier = .4f;
    private float reloadTimeMultiplier;
    [Space]
    [SerializeField] private Transform[] webSet;
    [SerializeField] private Transform[] attackPoint;
    [SerializeField] private Transform[] attackPointRef;

    private GameObject[] activeSpider;
    private int spiderIndex;
    private Vector3 spiderOffset = new Vector3(0, -.17f, 0);

    protected override void Awake()
    {
        base.Awake();
        InitializeSpiders();

        reloadTimeMultiplier = 1 - attackTimeMultiplier;
    }

    protected override void Update()
    {
        base.Update();
        UpdateAttackPointsPosition();
    }

    protected override bool CanAttack()
    {
        return Time.time > lastTimeAttack + attackCooldown && AtLeanstOneEnemyAround();
    }

    protected override void Attack()
    {
        base.Attack();
        StartCoroutine(AttackCo());
    }

    private IEnumerator AttackCo()
    {
        Transform currentWeb = webSet[spiderIndex];
        Transform currentActtackPoint = attackPoint[spiderIndex];
        float attackTime = (attackCooldown / 4) * attackTimeMultiplier;
        float reloadTime = (attackCooldown / 4) * reloadTimeMultiplier;


        yield return ChangeScaleCo(currentWeb, 1, attackTime);

        activeSpider[spiderIndex].GetComponent<ProjectileSpiderNest>().SetupSpider(damage);

        yield return ChangeScaleCo(currentWeb, .1f, reloadTime);
        activeSpider[spiderIndex] = Instantiate(spiderPrefab, currentActtackPoint.position + spiderOffset, Quaternion.identity, currentActtackPoint);

        spiderIndex = (spiderIndex + 1) % attackPoint.Length;
    }

    private void UpdateAttackPointsPosition()
    {
        for (int i = 0; i < attackPoint.Length; i++)
        {
            attackPoint[i].position = attackPointRef[i].position;
        }
    }

    private void InitializeSpiders()
    {
        activeSpider = new GameObject[attackPoint.Length];
        for (int i = 0; i < attackPoint.Length; i++)
        {
            GameObject newSpider = Instantiate(spiderPrefab, attackPoint[i].position + spiderOffset, Quaternion.identity, attackPoint[i]);
            activeSpider[i] = newSpider;
        }
    }

    private IEnumerator ChangeScaleCo(Transform transform, float newScale, float duration = .25f)
    {
        float time = 0;

        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = new Vector3(1, newScale, 1);

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, time / duration);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}