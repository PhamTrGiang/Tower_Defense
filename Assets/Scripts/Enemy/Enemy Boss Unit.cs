using UnityEngine;

public class EnemyBossUnit : Enemy
{
    private Vector3 savedDestination;
    private Vector3 lastKnowBossPosition;
    private EnemyFlyingBoss myBoss;

    protected override void Update()
    {
        base.Update();

        if (myBoss != null)
            lastKnowBossPosition = myBoss.transform.position;
    }

    public void SetupEnemy(Vector3 destination, EnemyFlyingBoss myNewBoss, EnemyPortal myNewPortal)
    {
        ResetEnemy();
        ResetMovementEnemy();

        myBoss = myNewBoss;
        myPortal = myNewPortal;
        myPortal.GetActiveEnemies().Add(gameObject);

        savedDestination = destination;

        InvokeRepeating(nameof(SpapToBossIfNeeded), .1f, .5f);
    }

    private void ResetMovementEnemy()
    {
        rb.useGravity = true;
        rb.isKinematic = false;
        agent.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
            return;

        rb.useGravity = false;
        rb.isKinematic = true;

        agent.enabled = true;
        agent.SetDestination(savedDestination);
    }

    private void SpapToBossIfNeeded()
    {
        if (agent.enabled && agent.isOnNavMesh == false)
        {

            if (Vector3.Distance(transform.position, lastKnowBossPosition) > 3)
            {
                transform.position = lastKnowBossPosition + new Vector3(0, -1, 0);
                ResetMovementEnemy();
            }
        }
    }

    public override float DistanceAToFinishLine()
    {
        return Vector3.Distance(transform.position, GetFinalWaypoint());
    }
}
