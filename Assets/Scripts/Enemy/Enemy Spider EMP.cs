using UnityEngine;

public class EnemySpiderEMP : MonoBehaviour
{
    private ObjectPoolManager objectPool;

    [SerializeField] private GameObject empFx;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float empRadius = 2;
    [SerializeField] private float empEffectDuration = 2;

    private Vector3 destination;
    private float shrinkSpeed = 3;
    private bool shouldShrink;

    private void Awake()
    {
        objectPool = ObjectPoolManager.Instance;
    }

    private void Update()
    {
        MoveTowardsTarget();


        if (shouldShrink)
            Shrink();
    }

    private void Shrink()
    {
        transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;

        if (transform.localScale.x <= .01f)
            objectPool.Remove(gameObject);
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) < .1f)
            DeactivateEMP();
    }

    public void SetUpEMP(float duration, Vector3 newTarget, float empDuration)
    {
        empEffectDuration = duration;
        destination = newTarget;

        //Invoke(nameof(DeactivateEMP), empDuration);
    }

    private void DeactivateEMP() => shouldShrink = true;

    void OnTriggerEnter(Collider other)
    {
        Tower tower = other.GetComponent<Tower>();

        if (tower != null)
            tower.DeactivateTower(empEffectDuration, empFx);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, empRadius);
    }
}
