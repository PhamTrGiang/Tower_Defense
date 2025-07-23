using System.Collections;
using UnityEngine;

public class SpiderLeg : MonoBehaviour
{
    [SerializeField] private float moveThreshold = .45f;
    private EnemySpiderVisuals spiderVisuals;
    private float legSpeed;
    private bool shouldMove;
    private bool canMove = true;
    private Coroutine moveCo;

    [Header("Leg Setup")]
    [SerializeField] private SpiderLeg oppositeLeg;
    [SerializeField] private SpiderLegReference legRef;
    [SerializeField] private Transform actualTarget;
    [SerializeField] private Transform bottomLeg;
    [SerializeField] private Vector3 placementOffset;
    [SerializeField] private Transform worldTargetReference;

    private void Awake()
    {
        spiderVisuals = GetComponentInParent<EnemySpiderVisuals>();
        worldTargetReference = Instantiate(worldTargetReference, actualTarget.position, Quaternion.identity).transform;
        worldTargetReference.gameObject.name = legRef.gameObject.name + "_world";
    }

    public void UpdateLeg(float legSpeed)
    {
        this.legSpeed = legSpeed;
        actualTarget.position = worldTargetReference.position + placementOffset;
        shouldMove = Vector3.Distance(worldTargetReference.position, legRef.ContactPoint()) > moveThreshold;

        if (bottomLeg != null)
            bottomLeg.forward = Vector3.down;

        if (shouldMove && canMove)
        {
            if (moveCo != null)
                StopCoroutine(moveCo);
            moveCo = StartCoroutine(LegMoveCo());
        }
    }

    private IEnumerator LegMoveCo()
    {
        oppositeLeg.CanMove(false);

        while (Vector3.Distance(worldTargetReference.position, legRef.ContactPoint()) > .01f)
        {
            worldTargetReference.position = Vector3.MoveTowards(worldTargetReference.position, legRef.ContactPoint(), legSpeed * Time.deltaTime);

            yield return null;
        }

        oppositeLeg.CanMove(true);
    }

    public void SpeedUpLeg() => StartCoroutine(SpeedUpLegCo());

    private IEnumerator SpeedUpLegCo()
    {
        legSpeed = spiderVisuals.increasedSpeed;

        yield return new WaitForSeconds(1f);

        legSpeed = spiderVisuals.legSpeed;
    }

    public void CanMove(bool enableMovement) => canMove = enableMovement;
}
