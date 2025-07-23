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
        myBoss = myNewBoss;

        myPortal = myNewPortal;
        myPortal.GetActiveEnemies().Add(gameObject);

        savedDestination = destination;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
            return;

        if (Vector3.Distance(transform.position, lastKnowBossPosition) > 3)
            transform.position = lastKnowBossPosition + new Vector3(0, -1, 0);


        rb.useGravity = false;
        rb.isKinematic = true;

        agent.enabled = true;
        agent.SetDestination(savedDestination);
    }
}
